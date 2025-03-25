using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Data;
using Microsoft.AspNetCore.Authorization; // Пространство имен, где определен CompanyDbContext

namespace AspNetCore_MVC_Project.Areas.BOX.Controllers
{
    [Area("BOX")] // Обязательно
    [AllowAnonymous] // Разрешаем анонимный доступ ко всему контроллеру
    public class ToiRController : Controller
    {

        //public ToiRController()
        //{
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}
