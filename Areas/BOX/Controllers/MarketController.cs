using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore_MVC_Project.Areas.BOX.Controllers
{
    [Area("BOX")] // Обязательно
    [AllowAnonymous] // Разрешаем анонимный доступ ко всему контроллеру
    public class MarketController : Controller
    {
        public MarketController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
