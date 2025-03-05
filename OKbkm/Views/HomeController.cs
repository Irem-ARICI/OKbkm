using Microsoft.AspNetCore.Mvc;

namespace OKbkm.Views
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
