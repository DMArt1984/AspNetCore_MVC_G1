using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.ViewModels;
using AspNetCore_MVC_Project.Data;
using System.Linq;
using System.Threading.Tasks;

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
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь {Email} успешно вошел в систему", model.Email);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
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
            var company = _context.Companies.FirstOrDefault(c => c.Name == model.CompanyName);
            if (company == null)
            {
                company = new Company { Name = model.CompanyName };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Создана новая компания: {CompanyName}", model.CompanyName);
            }

            // Создаем пользователя
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, CompanyId = company.Id };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь {Email} успешно создан", model.Email);

                // Добавляем доступные модули (Business, KPI)
                if (model.SelectedModules != null && model.SelectedModules.Any())
                {
                    foreach (var module in model.SelectedModules)
                    {
                        var buyModule = new BuyModule { NameController = module, CompanyId = company.Id };
                        _context.BuyModules.Add(buyModule);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Добавлены модули для компании {CompanyName}", company.Name);
                }

                // Создаем базу данных для компании
                string databaseName = $"w{company.Name.Replace(" ", "_")}";
                string connectionString = GetCompanyConnectionString(databaseName);
                _logger.LogInformation("Попытка создания базы данных: {DatabaseName}", databaseName);

                try
                {
                    using (var dbContext = new CompanyDbContext(connectionString))
                    {
                        dbContext.Database.EnsureCreated();
                    }
                    _logger.LogInformation("База данных {DatabaseName} успешно создана", databaseName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании базы данных {DatabaseName}", databaseName);
                    ModelState.AddModelError("", "Ошибка при создании базы компании. Обратитесь к администратору.");
                    return View(model);
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

