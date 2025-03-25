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

            var userAccounts = _context.AccountCreates
                .Where(a => a.TC == userTC)
                .ToList();

            return View(userAccounts);
        }

        [HttpPost]
        public IActionResult Create(AccountCreate model)
        {
            var userTC = HttpContext.Session.GetString("UserTC");

            if (string.IsNullOrEmpty(userTC))
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                model.TC = userTC;
                model.AccountNo = GenerateAccountNumber();
                model.Balance = 0.00m;

                _context.AccountCreates.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            var userAccounts = _context.AccountCreates
                .Where(a => a.TC == userTC)
                .ToList();

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View("Index", userAccounts);
        }

        private int GenerateAccountNumber()
        {
            return new Random().Next(1000000000, 1999999999);
        }
    }
}
