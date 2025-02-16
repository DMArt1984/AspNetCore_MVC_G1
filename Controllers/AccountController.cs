using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.ViewModels;
using AspNetCore_MVC_Project.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AspNetCore_MVC_Project.Models.Control;
using AspNetCore_MVC_Project.Models.Factory;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// Контроллер для управления аутентификацией и регистрацией пользователей.
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        /// Конструктор AccountController.
        /// </summary>
        /// <param name="signInManager">Менеджер входа в систему.</param>
        /// <param name="userManager">Менеджер пользователей.</param>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <param name="logger">Логгер для отслеживания событий.</param>
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AccountController> logger)
            : base(configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Получает контекст базы данных компании.
        /// </summary>
        /// <param name="companyId">Идентификатор компании.</param>
        /// <returns>Контекст базы данных компании.</returns>
        private async Task<CompanyDbContext> GetCompanyDbContext(int companyId)
        {
            var company = await _context.Factories.FindAsync(companyId);
            if (company == null) return null;

            string databaseName = $"w{company.Title.Replace(" ", "_")}";
            string connectionString = _configuration.GetConnectionString("CompanyDatabaseTemplate").Replace("{0}", databaseName);
            return new CompanyDbContext(connectionString);
        }


        /// <summary>
        /// Возвращает страницу входа.
        /// </summary>
        public IActionResult Login() => View();

        /// <summary>
        /// Обрабатывает попытку входа пользователя в систему.
        /// </summary>
        /// <param name="model">Данные входа (логин, пароль).</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь {Email} успешно вошел в систему", model.Email);

                // Получаем пользователя
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user?.FactoryId != null)
                {
                    var dbContext = await GetCompanyDbContext(user.FactoryId.Value);
                    if (dbContext != null)
                    {
                        dbContext.Database.EnsureCreated(); // Автоматически создаёт таблицы, если их нет
                        // ✅ Вместо EnsureCreated() используем Migrate() для обновления схемы
                        //dbContext.Database.Migrate();
                        _logger.LogInformation("Синхронизация базы компании {DatabaseName} завершена", $"w{user.Factory.Title}");

                        // Проверяем, есть ли таблица Mark
                        //if (!await dbContext.Marks.AnyAsync())
                        //{
                        //    _logger.LogInformation("Создаётся таблица Mark для компании {DatabaseName}", $"w{user.Company.Name}");

                        //    // Добавляем записи в таблицу Mark
                        //    dbContext.Marks.Add(new Mark { Title = "Creator", Value = user.UserName });
                        //    dbContext.Marks.Add(new Mark { Title = "Service", Value = "0" });

                        //    await dbContext.SaveChangesAsync();
                        //    _logger.LogInformation("Записи Mark созданы для компании {DatabaseName}", $"w{user.Company.Name}");
                        //}
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }


        /// <summary>
        /// Возвращает страницу регистрации.
        /// </summary>
        public IActionResult Register() => View();

        /// <summary>
        /// Обрабатывает регистрацию нового пользователя.
        /// </summary>
        /// <param name="model">Данные регистрации (email, пароль, компания, модули).</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Попытка регистрации пользователя {Email}", model.Email);

            // Проверяем, существует ли компания
            var company = await _context.Factories.FirstOrDefaultAsync(c => c.Title == model.CompanyName);
            bool isNewCompany = false; // Флаг новой компании

            if (company == null)
            {
                company = new Factory { Title = model.CompanyName };
                _context.Factories.Add(company);
                await _context.SaveChangesAsync();
                isNewCompany = true;
                _logger.LogInformation("Создана новая компания: {CompanyName}", model.CompanyName);
            }

            // Создаем пользователя
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FactoryId = company.Id };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь {Email} успешно создан", model.Email);

                // Добавляем доступные модули (Business, KPI)
                if (model.SelectedModules != null && model.SelectedModules.Any())
                {
                    var optionBlocks = await _context.OptionBlocks
                        .Where(o => model.SelectedModules.Contains(o.NameController))
                        .ToListAsync(); // Получаем все OptionBlock по именам контроллеров

                    if (!optionBlocks.Any())
                    {
                        _logger.LogWarning("Не найдены соответствующие OptionBlocks для выбранных модулей.");
                        return BadRequest("Некоторые или все выбранные модули не существуют.");
                    }

                    var buyModules = optionBlocks.Select(optionBlock => new Purchase
                    {
                        OptionBlockId = optionBlock.Id, // Используем связь с OptionBlock
                        FactoryId = company.Id
                    }).ToList();

                    _context.Purchases.AddRange(buyModules);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Добавлены модули для компании {CompanyName}", company.Title);
                }

                // Создание базы данных компании при первой регистрации
                if (isNewCompany)
                {
                    var dbContext = await GetCompanyDbContext(company.Id);
                    if (dbContext != null)
                    {
                        dbContext.Database.EnsureCreated(); // Создаём базу

                        // Добавляем записи в таблицу Mark
                        dbContext.Marks.Add(new Mark { Title = "Creator", Value = user.UserName });
                        dbContext.Marks.Add(new Mark { Title = "Service", Value = "0" });

                        await dbContext.SaveChangesAsync();
                        _logger.LogInformation("База данных {DatabaseName} успешно создана", $"w{company.Title.Replace(" ", "_")}");
                    }
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Ошибка при создании пользователя: {Error}", error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Обрабатывает выход пользователя из системы.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Пользователь вышел из системы");
            return RedirectToAction("Index", "Home");
        }
    }
}

