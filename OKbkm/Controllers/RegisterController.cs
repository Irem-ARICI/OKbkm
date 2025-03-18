using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;

using System.Linq;

namespace OKbkm.Controllers
{
    public class RegisterController : Controller
    {
        private readonly Context _context;

        public RegisterController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Register model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcının zaten kayıtlı olup olmadığını kontrol et
                var existingUser = _context.Registers.FirstOrDefault(u => u.TC == model.TC);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu TC kimlik numarası ile zaten kayıtlı bir kullanıcı var.");
                    return View(model);
                }

                //// Yeni kullanıcıyı veritabanına ekle
                //_context.Registers.Add(model);
                //_context.SaveChanges();
                // Yeni kullanıcıyı veritabanına ekle
                _context.Registers.Add(new Register
                {
                    TC = model.TC,
                    NameUsername = model.NameUsername,
                    Mail = model.Mail,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                });
                _context.SaveChanges();


                return RedirectToAction("Welcome");
            }
            return View(model);
        }

        public IActionResult Welcome()
        {
            return Content("Kaydınız başarıyla tamamlandı. Hoş geldiniz!");
        }
    }
}




//using Microsoft.AspNetCore.Mvc;
//using OKbkm.Models;

//namespace OKbkm.Controllers
//{
//    public class RegisterController : Controller
//    {
//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Index(Register model)
//        {
//            if (ModelState.IsValid)
//            {
//                // Burada kayıt işlemi yapılabilir (veritabanına ekleme vs.)
//                return RedirectToAction("Welcome");
//            }
//            return View(model);
//        }

//        public IActionResult Welcome()
//        {
//            return Content("Kaydınız başarıyla tamamlandı. Hoş geldiniz!");
//        }
//    }
//}
