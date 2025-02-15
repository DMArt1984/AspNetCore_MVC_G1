using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// Контроллер главной страницы.
    /// Отвечает за отображение стартовой страницы приложения.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Конструктор HomeController.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        public HomeController(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// Отображает главную страницу.
        /// </summary>
        /// <returns>Представление главной страницы.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}