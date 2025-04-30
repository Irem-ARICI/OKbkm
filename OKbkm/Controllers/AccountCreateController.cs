using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace OKbkm.Controllers
{
    public class AccountCreateController : Controller
    {
        private readonly Context _context;

        public AccountCreateController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userTC = HttpContext.Session.GetString("UserTC");
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userTC))
                return RedirectToAction("Index", "Login");

            ViewBag.UserName = userName;

            // ✅ Account tablosunu kullanıyoruz artık!
            var userAccounts = _context.Accounts
                .Where(a => a.TC == userTC)
                .ToList();

            return View(userAccounts);
        }

        [HttpPost]
        public IActionResult Index([Bind("CardType")] Account model)    //         public IActionResult Index([Bind("CardType")] Account model)
        {
            var userTC = HttpContext.Session.GetString("UserTC");
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userTC))
                return RedirectToAction("Index", "Login");

            // Aynı TC + KartTipi var mı kontrol et
            var hasSameCard = _context.Accounts.Any(a => a.TC == userTC && a.CardType == model.CardType);
            if (hasSameCard)
            {
                ViewBag.UserName = userName;
                ViewBag.Error = "Aynı kart tipinden birden fazla hesap oluşturamazsınız.";

                var existingAccounts = _context.Accounts
                    .Where(a => a.TC == userTC)
                    .ToList();

                return View(existingAccounts);
            }

            //if (ModelState.IsValid)
            //{
            model.TC = userTC;
            model.AccountNo = GenerateAccountNumber().ToString();
                model.Balance = 0.00m;

                _context.Accounts.Add(model);
                _context.SaveChanges();
            //}

            ViewBag.UserName = userName;

            var userAccounts = _context.Accounts
                .Where(a => a.TC == userTC)
                .ToList();

            //return View(userAccounts);
             return RedirectToAction("Index");
        }

        //private int GenerateAccountNumber()
        //{
        //    Random rnd = new Random();
        //    return rnd.Next(100000000, 999999999);
        //}

        private string GenerateAccountNumber()
        {
            Random rnd = new Random();
            return string.Join("", Enumerable.Range(0, 12).Select(i => rnd.Next(0, 10)));
        }


    }
}