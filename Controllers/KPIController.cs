using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// Контроллер для работы с KPI-данными.
    /// Отвечает за доступ пользователей к данным таблицы Statistic.
    /// </summary>
    [Authorize] // Ограничивает доступ только для авторизованных пользователей
    public class KPIController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор контроллера KPI.
        /// </summary>
        /// <param name="userManager">Менеджер пользователей Identity.</param>
        /// <param name="context">Основной контекст базы данных.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        public KPIController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
            : base(configuration) // Вызывает конструктор BaseController
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Получает контекст базы данных компании.
        /// Проверяет, есть ли у пользователя доступ к модулю "KPI".
        /// </summary>
        /// <returns>Контекст базы данных компании или null, если доступ запрещен.</returns>
        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return null;

            // Находим компанию пользователя
            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null) return null;

            // Проверяем, есть ли у компании доступ к модулю "KPI"
            bool hasAccess = await _context.BuyModules.AnyAsync(bm => bm.CompanyId == user.CompanyId && bm.NameController == "KPI");
            if (!hasAccess) return null;

            // Формируем строку подключения и создаем контекст для базы данных компании
            string connectionString = GetCompanyConnectionString($"w{company.Name.Replace(" ", "_")}");
            return new CompanyDbContext(connectionString);
        }

        /// <summary>
        /// Отображает список данных из таблицы Statistic.
        /// Доступен только пользователям, у которых есть доступ к модулю "KPI".
        /// </summary>
        /// <returns>Страница с таблицей данных Statistic.</returns>
        public async Task<IActionResult> Index()
        {
            // Получаем контекст базы данных компании
            var dbContext = await GetCompanyDbContext();
            if (dbContext == null) return RedirectToAction("Index", "Home"); // Если доступа нет – перенаправляем на главную страницу

            // Загружаем все записи из таблицы Statistic
            var statistics = await dbContext.Statistics.ToListAsync();
            return View(statistics);
        }
    }
}
