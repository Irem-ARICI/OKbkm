using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using System;
using System.Linq;

namespace OKbkm.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly Context _context;

        public TransactionsController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Transactions model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int accountNo = int.Parse(model.AccountNo);
            var account = _context.Accounts.FirstOrDefault(a => a.AccountNo == accountNo);

            if (account == null)
            {
                ModelState.AddModelError("", "Hesap bulunamadı.");
                return View(model);
            }

            if (model.TransactionType == "Deposit")
            {
                account.Balance += model.Total;
                model.ReceiverAccountNo = null; // sadece transferde kullanılır
            }
            else if (model.TransactionType == "Withdrawal")
            {
                if (account.Balance < model.Total)
                {
                    ModelState.AddModelError("", "Yetersiz bakiye.");
                    return View(model);
                }

                account.Balance -= model.Total;
                model.ReceiverAccountNo = null;
            }

                else if (model.TransactionType == "Transfer")
                {
                    if (string.IsNullOrEmpty(model.ReceiverAccountNo))
                    {
                        ModelState.AddModelError("", "Alıcı hesap numarası zorunludur.");
                        return View(model);
                    }

                    // ✅ 1. string -> int dönüşüm
                    int receiverNo = int.Parse(model.ReceiverAccountNo);

                    // ✅ 2. receiverAccount'u tek seferde tanımla
                    var receiverAccount = _context.Accounts.FirstOrDefault(a => a.AccountNo == receiverNo);

                    if (receiverAccount == null)
                    {
                        ModelState.AddModelError("", "Alıcı hesap bulunamadı.");
                        return View(model);
                    }

                    if (account.Balance < model.Total)
                    {
                        ModelState.AddModelError("", "Yetersiz bakiye.");
                        return View(model);
                    }

                    account.Balance -= model.Total;
                    receiverAccount.Balance += model.Total;

                    _context.Update(receiverAccount); // ✅ Güncelle
                }

                else
                {
                    ModelState.AddModelError("", "Geçersiz işlem türü.");
                    return View(model);
                }

                _context.Update(account);

                model.TransactionDate = DateTime.UtcNow;
                _context.Transactions.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "İşlem başarıyla kaydedildi.";
                return RedirectToAction("Index");
            }



        }
    } 