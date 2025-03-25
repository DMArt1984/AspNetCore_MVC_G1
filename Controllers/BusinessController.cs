using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Data;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// Контроллер для работы с бизнес-данными.
    /// Отвечает за доступ пользователей к данным таблицы Job.
    /// </summary>
    [Authorize] // Ограничивает доступ только для авторизованных пользователей
    public class BusinessController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор контроллера Business.
        /// </summary>
        /// <param name="userManager">Менеджер пользователей Identity.</param>
        /// <param name="context">Основной контекст базы данных.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        public BusinessController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
            : base(configuration) // Вызывает конструктор BaseController
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Получает контекст базы данных компании.
        /// Проверяет, есть ли у пользователя доступ к модулю "Business".
        /// Доступ разрешается, если у компании назначен модуль, у которого:
        /// - NameController равен "Business"
        /// - Если NameArea задан (не пустой), он также должен равняться "Business"
        /// </summary>
        /// <returns>Контекст базы данных компании или null, если доступ запрещен.</returns>
        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);
            if (user?.FactoryId == null)
                return null;

            // Находим компанию пользователя по FactoryId
            var company = await _context.Factories.FindAsync(user.FactoryId);
            if (company == null)
                return null;

            // Проверяем, есть ли у компании доступ к модулю "Business"
            bool hasAccess = await _context.Purchases
                .Include(p => p.OptionBlock) // Загружаем связанные данные OptionBlock
                .AnyAsync(p => p.FactoryId == user.FactoryId &&
                               p.OptionBlock.NameController == "Business" &&
                               (string.IsNullOrEmpty(p.OptionBlock.NameArea) || p.OptionBlock.NameArea == "Business"));

            if (!hasAccess)
                return null;

            // Формируем строку подключения и создаем контекст для базы данных компании.
            // Здесь используется название компании для формирования уникальной строки подключения.
            string connectionString = GetCompanyConnectionString($"w{company.Title.Replace(" ", "_")}");
            return new CompanyDbContext(connectionString);
        }

        /// <summary>
        /// Отображает список данных из таблицы Job.
        /// Доступен только пользователям, у которых есть доступ к модулю "Business".
        /// </summary>
        /// <returns>Страница с таблицей данных Job.</returns>
        public async Task<IActionResult> Index()
        {
            // Получаем контекст базы данных компании
            var dbContext = await GetCompanyDbContext();
            if (dbContext == null)
            {
                // Если доступа нет – перенаправляем пользователя на главную страницу
                return RedirectToAction("Index", "Home");
            }

            // Загружаем все записи из таблицы Job
            var jobs = await dbContext.Jobs.ToListAsync();
            return View(jobs);
        }
    }
}
