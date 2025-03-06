using Microsoft.AspNetCore.Mvc;
using OKbkm.Models;

namespace OKbkm.Controllers
{
    public class RegisterController : Controller
    {
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
                // Burada kayıt işlemi yapılabilir (veritabanına ekleme vs.)
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
