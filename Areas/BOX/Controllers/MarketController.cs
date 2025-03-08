using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_MVC_Project.Areas.BOX.Controllers
{
    public class MarketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
