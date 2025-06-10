using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using System.Collections.Generic;
using System.Linq;

namespace OKbkm.Controllers
{
    public class TransactionHistoryController : Controller
    {
        private readonly Context _context;

        public TransactionHistoryController(Context context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var userTC = HttpContext.Session.GetString("UserTC");

            if (string.IsNullOrEmpty(userTC))
                return RedirectToAction("Index", "Login");

            var accounts = _context.Accounts
                                   .Where(a => a.TC == userTC)
                                   .ToList();

            ViewBag.Accounts = accounts;
            ViewBag.AccountNo = null;

            return View(new List<TransactionHistory>()); // <- boş liste döner
        }

        [HttpPost]
        public IActionResult Index(string selectedAccountNo)
        {
            if (string.IsNullOrEmpty(selectedAccountNo))
                return RedirectToAction("Index");

            var histories = _context.TransactionHistories
                .Where(t => t.AccountNo == selectedAccountNo)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            var accounts = _context.Accounts
                                   .Where(a => a.TC == HttpContext.Session.GetString("UserTC"))
                                   .ToList();

            ViewBag.AccountNo = selectedAccountNo;
            ViewBag.Accounts = accounts;

            return View(histories);
        }

    }
}
