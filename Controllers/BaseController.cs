using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// Базовый контроллер, от которого наследуются другие контроллеры.
    /// Содержит методы, общие для всех контроллеров, такие как получение строки подключения.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Конфигурация приложения, используется для доступа к appsettings.json.
        /// </summary>
        protected readonly IConfiguration _configuration;

        /// <summary>
        /// Конструктор базового контроллера.
        /// </summary>
        /// <param name="configuration">Интерфейс IConfiguration для доступа к настройкам приложения.</param>
        public BaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Метод получает строку подключения для базы данных компании.
        /// </summary>
        /// <param name="databaseName">Название базы данных компании.</param>
        /// <returns>Форматированная строка подключения к базе данных.</returns>
        protected string GetCompanyConnectionString(string databaseName)
        {
            // Получаем шаблон строки подключения из appsettings.json
            string template = _configuration.GetConnectionString("CompanyDatabaseTemplate");

            // Заменяем {0} на фактическое название базы данных компании
            return string.Format(template, databaseName);
        }
    }
}
