using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
