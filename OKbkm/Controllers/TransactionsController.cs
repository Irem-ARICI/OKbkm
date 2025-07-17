using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using OKbkm.Services;
using System.Linq;

namespace OKbkm.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly Context _context;
        private readonly KafkaProducerService _kafka;

        public TransactionsController(Context context, KafkaProducerService kafka)
        {
            _context = context;
            _kafka = kafka;
        }

        [HttpGet]
        public IActionResult Index(string selectedAccountNo = null)
        {
            var userTC = HttpContext.Session.GetString("UserTC");
            if (string.IsNullOrEmpty(userTC))
                return RedirectToAction("Index", "Login");

            var accounts = _context.Accounts
                .Where(a => a.TC == userTC)
                .ToList();

            ViewBag.Accounts = accounts;

            var selectedAccount = !string.IsNullOrEmpty(selectedAccountNo)
                ? accounts.FirstOrDefault(a => a.AccountNo == selectedAccountNo)
                : accounts.FirstOrDefault();

            ViewBag.SelectedBalance = selectedAccount?.Balance ?? 0;
            ViewBag.SelectedAccountNo = selectedAccount?.AccountNo;

            return View();
        }

        [HttpGet]
        public IActionResult Deposit(string selectedAccountNo)
        {
            ViewBag.SelectedAccountNo = selectedAccountNo;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(decimal amount, string selectedAccountNo)
        {
            var userTC = HttpContext.Session.GetString("UserTC");
            var account = _context.Accounts.FirstOrDefault(a => a.TC == userTC && a.AccountNo == selectedAccountNo);

            if (account == null)
                return RedirectToAction("Index", "Login");

            account.Balance += amount;
            _context.Update(account);

            var history = new TransactionHistory
            {
                AccountNo = account.AccountNo,
                TransactionAmount = amount,
                BalanceAfter = account.Balance,
                TransactionType = "Deposit",
                TransactionDate = DateTime.UtcNow
            };
            _context.Add(history);

            _context.SaveChanges();

            //// 🔸 Kafka mesajı
            //await _kafka.ProduceAsync("deposit-topic", new
            //{
            //    UserTC = userTC,
            //    AccountNo = account.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = account.Balance,
            //    TransactionType = "Deposit",
            //    Date = DateTime.UtcNow
            //});

            try
            {
                await _kafka.ProduceAsync("deposit-topic", new
                {
                    UserTC = userTC,
                    AccountNo = account.AccountNo,
                    Amount = amount,
                    BalanceAfter = account.Balance,
                    TransactionType = "Deposit",
                    Date = DateTime.UtcNow
                });
            }
            //catch (Exception ex)
            //{
            //    // 🔴 Basit loglama (gelişmiş log için Serilog veya ILogger kullanılabilir)
            //    Console.WriteLine($"Kafka'ya mesaj gönderilemedi: {ex.Message}");
            //}
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine("🚨 Kafka ProduceException:");
                Console.WriteLine($"• Reason: {ex.Error.Reason}");
                Console.WriteLine($"• IsFatal: {ex.Error.IsFatal}");
                Console.WriteLine($"• Code: {ex.Error.Code}");
                //Console.WriteLine($"• Broker: {ex.BrokerMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Genel hata: {ex}");
            }

            TempData["Success"] = "Para başarıyla yatırıldı.";
            return RedirectToAction("Index", new { selectedAccountNo });
        }

        [HttpGet]
        public IActionResult Withdraw(string selectedAccountNo)
        {
            if (string.IsNullOrEmpty(selectedAccountNo))
                return RedirectToAction("Index");

            ViewBag.SelectedAccountNo = selectedAccountNo;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(decimal amount, string selectedAccountNo)
        {
            var userTC = HttpContext.Session.GetString("UserTC");

            if (string.IsNullOrEmpty(userTC) || string.IsNullOrEmpty(selectedAccountNo))
                return RedirectToAction("Index", "Login");

            var account = _context.Accounts.FirstOrDefault(a => a.TC == userTC && a.AccountNo == selectedAccountNo);

            if (account == null)
                return RedirectToAction("Index", "Login");

            if (account.Balance < amount)
            {
                ModelState.AddModelError("", "Yetersiz bakiye.");
                ViewBag.SelectedAccountNo = selectedAccountNo;
                return View();
            }

            account.Balance -= amount;
            _context.Update(account);

            var history = new TransactionHistory
            {
                AccountNo = account.AccountNo,
                TransactionAmount = amount,
                BalanceAfter = account.Balance,
                TransactionType = "Withdraw",
                TransactionDate = DateTime.UtcNow
            };

            _context.Add(history);
            _context.SaveChanges();

            //// 🔸 Kafka mesajı Withdraw işlemi için
            //await _kafka.ProduceAsync("withdraw-topic", new
            //{
            //    UserTC = userTC,
            //    AccountNo = account.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = account.Balance,
            //    TransactionType = "Withdraw",
            //    Date = DateTime.UtcNow
            //});

            // Kafka mesajı Withdraw işlemi için
            try
            {
                await _kafka.ProduceAsync("withdraw-topic", new
                {
                    UserTC = userTC,
                    AccountNo = account.AccountNo,
                    Amount = amount,
                    BalanceAfter = account.Balance,
                    TransactionType = "Withdraw",
                    Date = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kafka'ya Withdraw mesajı gönderilemedi: {ex.Message}");
            }

            TempData["Success"] = "Para başarıyla çekildi.";
            return RedirectToAction("Index", new { selectedAccountNo });
        }

        [HttpGet]
        public IActionResult Transfer(string selectedAccountNo)
        {
            var userTC = HttpContext.Session.GetString("UserTC");

            if (string.IsNullOrEmpty(userTC))
                return RedirectToAction("Index", "Login");

            var accounts = _context.Accounts
                .Where(a => a.TC == userTC)
                .ToList();

            ViewBag.Accounts = accounts;

            // Eğer URL'den gelen bir selectedAccountNo varsa onu kullan, yoksa ilk hesabı seç
            var selectedAccount = !string.IsNullOrEmpty(selectedAccountNo)
                ? accounts.FirstOrDefault(a => a.AccountNo == selectedAccountNo)
                : accounts.FirstOrDefault();

            ViewBag.SelectedAccountNo = selectedAccount?.AccountNo;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Transfer(string receiverAccountNo, decimal amount, string selectedAccountNo)
        {
            var userTC = HttpContext.Session.GetString("UserTC");

            if (string.IsNullOrEmpty(userTC) || string.IsNullOrEmpty(selectedAccountNo))
            {
                TempData["Error"] = "Geçersiz hesap seçimi.";
                return RedirectToAction("Index");
            }

            var senderAccount = _context.Accounts.FirstOrDefault(a => a.TC == userTC && a.AccountNo == selectedAccountNo);
            var receiverAccount = _context.Accounts.FirstOrDefault(a => a.AccountNo == receiverAccountNo);

            if (senderAccount == null)
            {
                ModelState.AddModelError("", "Gönderen hesap bulunamadı.");
                return View();
            }

            if (receiverAccount == null)
            {
                ModelState.AddModelError("", "Alıcı hesap bulunamadı.");
                return View();
            }

            if (senderAccount.Balance < amount)
            {
                ModelState.AddModelError("", "Yetersiz bakiye.");
                return View();
            }

            senderAccount.Balance -= amount;
            receiverAccount.Balance += amount;

            _context.Update(senderAccount);
            _context.Update(receiverAccount);

            // Gönderen için işlem geçmişi
            var senderHistory = new TransactionHistory
            {
                AccountNo = senderAccount.AccountNo,
                TransactionAmount = amount,
                BalanceAfter = senderAccount.Balance,
                TransactionType = "Transfer - Gönderilen",
                TransactionDate = DateTime.UtcNow
            };
            _context.Add(senderHistory);

            // Alıcı için işlem geçmişi
            var receiverHistory = new TransactionHistory
            {
                AccountNo = receiverAccount.AccountNo,
                TransactionAmount = amount,
                BalanceAfter = receiverAccount.Balance,
                TransactionType = "Transfer - Alınan",
                TransactionDate = DateTime.UtcNow
            };
            _context.Add(receiverHistory);

            _context.SaveChanges();

            //// 🔸 Kafka mesajı Transfer işlemi için
            //await _kafka.ProduceAsync("transfer-topic", new
            //{
            //    UserTC = userTC,
            //    AccountNo = senderAccount.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = senderAccount.Balance,
            //    TransactionType = "Transfer - Gönderilen",
            //    Date = DateTime.UtcNow
            //});

            //await _kafka.ProduceAsync("transfer-topic", new
            //{
            //    UserTC = userTC,
            //    AccountNo = receiverAccount.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = receiverAccount.Balance,
            //    TransactionType = "Transfer - Alınan",
            //    Date = DateTime.UtcNow
            //});

            // Gönderici Kafka mesajı
            try
            {
                await _kafka.ProduceAsync("transfer-topic", new
                {
                    UserTC = userTC,
                    AccountNo = senderAccount.AccountNo,
                    Amount = amount,
                    BalanceAfter = senderAccount.Balance,
                    TransactionType = "Transfer - Gönderilen",
                    Date = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kafka'ya Transfer-Gönderilen mesajı gönderilemedi: {ex.Message}");
            }

            // Alıcı Kafka mesajı
            try
            {
                await _kafka.ProduceAsync("transfer-topic", new
                {
                    AccountNo = receiverAccount.AccountNo,
                    Amount = amount,
                    BalanceAfter = receiverAccount.Balance,
                    TransactionType = "Transfer - Alınan",
                    Date = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kafka'ya Transfer-Alınan mesajı gönderilemedi: {ex.Message}");
            }

            TempData["Success"] = "Para başarıyla transfer edildi.";
            return RedirectToAction("Index", new { selectedAccountNo });
        }


    }
}
