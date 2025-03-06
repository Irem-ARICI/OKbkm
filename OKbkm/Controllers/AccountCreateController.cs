using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OKbkm.Controllers
{
    public class AccountCreateController : Controller
    {
        private static List<AccountCreate> accounts = new List<AccountCreate>(); // Hesapları saklayan liste
        private static List<Register> users = new List<Register> // Kullanıcıları saklayan liste
        {
            new Register { id = 1, TC = "12345678901", NameUsername = "Ahmet Yılmaz", Mail = "ahmet@example.com", Password = "1234", PhoneNumber = "555-1234", Address = "İstanbul" },
            new Register { id = 2, TC = "98765432109", NameUsername = "Merve Aksoy", Mail = "merve@example.com", Password = "5678", PhoneNumber = "555-5678", Address = "Ankara" }
        };

        [HttpGet]
        public IActionResult Index()
        {
            return View(accounts);
        }

        [HttpPost]
        public IActionResult Create(AccountCreate model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcının TC numarasına göre NameUsername'ı bul
                var user = users.FirstOrDefault(u => u.TC == model.TC);
                if (user != null)
                {
                    model.AccountNo = GenerateAccountNumber(); // 10 basamaklı hesap numarası oluştur
                    accounts.Add(model);
                    ViewBag.NameUsername = user.NameUsername; // NameUsername'ı ViewBag'e ekleyerek Index.cshtml'de kullanacağız
                }
                return RedirectToAction("Index");
            }
            return View("Index", accounts);
        }

        private int GenerateAccountNumber()
        {
            Random random = new Random();
            return random.Next(1000000000, 1999999999); // 10 basamaklı hesap no oluştur
        }
    }
}






//using Microsoft.AspNetCore.Mvc;
//using OKbkm.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace OKbkm.Controllers
//{
//    public class AccountCreateController : Controller
//    {
//        private static List<AccountCreate> accounts = new List<AccountCreate>();

//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View(accounts);
//        }

//        [HttpPost]
//        public IActionResult Create(AccountCreate model)
//        {
//            if (ModelState.IsValid)
//            {
//                model.AccountNo = GenerateAccountNumber(); // 10 basamaklı hesap numarası oluştur
//                accounts.Add(model);
//                return RedirectToAction("Index");
//            }
//            return View("Index", accounts);
//        }

//        private int GenerateAccountNumber()
//        {
//            Random random = new Random();
//            return random.Next(1000000000, 1999999999); // 10 basamaklı hesap no oluştur
//        }
//    }
//}
