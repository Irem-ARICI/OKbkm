using Microsoft.AspNetCore.Mvc;

namespace OKbkm.Controllers
{
    public class TransactionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
