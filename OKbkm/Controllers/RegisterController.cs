using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;
using OKbkm.Data; // ApplicationDbContext'i kullanabilmek için
using System.Linq;

namespace OKbkm.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
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
                var existingUser = _context.Users.FirstOrDefault(u => u.TC == model.TC);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu TC kimlik numarası ile zaten kayıtlı bir kullanıcı var.");
                    return View(model);
                }

                // Yeni kullanıcıyı veritabanına ekle
                _context.Users.Add(model);
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
