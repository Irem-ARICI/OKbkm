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

            // Kafka'ya mesaj gönder
            await _kafka.SendMessageAsync("deposit-topic", new TransactionEvent
            {
                AccountNo = account.AccountNo,
                Amount = amount,
                BalanceAfter = account.Balance,
                Type = "Deposit",
                Timestamp = DateTime.UtcNow
            });

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

            //await _kafka.SendMessageAsync("withdraw-topic", new TransactionEvent
            //{
            //    AccountNo = account.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = account.Balance,
            //    Type = "Withdraw",
            //    Timestamp = DateTime.UtcNow
            //});
            await SendKafkaTransaction("withdraw-topic", "Withdraw", account.AccountNo, amount, account.Balance);


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

            // Kafka'ya mesaj gönder (Gönderici)
            //await _kafka.SendMessageAsync("transfer-topic", new TransactionEvent
            //{
            //    AccountNo = senderAccount.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = senderAccount.Balance,
            //    Type = "Transfer-Sent",
            //    Timestamp = DateTime.UtcNow
            //});

            // Kafka'ya mesaj gönder (Alıcı)
            //await _kafka.SendMessageAsync("transfer-topic", new TransactionEvent
            //{
            //    AccountNo = receiverAccount.AccountNo,
            //    Amount = amount,
            //    BalanceAfter = receiverAccount.Balance,
            //    Type = "Transfer-Received",
            //    Timestamp = DateTime.UtcNow
            //});
            await SendKafkaTransaction("transfer-topic", "Transfer-Sent", senderAccount.AccountNo, amount, senderAccount.Balance);
            await SendKafkaTransaction("transfer-topic", "Transfer-Received", receiverAccount.AccountNo, amount, receiverAccount.Balance);

            TempData["Success"] = "Para başarıyla transfer edildi.";
            return RedirectToAction("Index", new { selectedAccountNo });
        }

        // KAFKA mesajı gönderen ortak metot:
        private async Task SendKafkaTransaction(string topic, string type, string accountNo, decimal amount, decimal balance)
        {
            await _kafka.SendMessageAsync(topic, new TransactionEvent
            {
                AccountNo = accountNo,
                Amount = amount,
                BalanceAfter = balance,
                Type = type,
                Timestamp = DateTime.UtcNow
            });
        }
        //private async Task SendKafkaTransaction(string topic, string type, string accountNo, decimal amount, decimal balanceAfter)
        //{
        //    var transactionEvent = new TransactionEvent
        //    {
        //        AccountNo = accountNo,
        //        Amount = amount,
        //        BalanceAfter = balanceAfter,
        //        Type = type,
        //        Timestamp = DateTime.UtcNow
        //    };

        //    var message = JsonConvert.SerializeObject(transactionEvent);

        //    await _kafkaProducer.SendAsync(topic, message);
        //}


    }
}
