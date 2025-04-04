using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_MVC_Project.Models.Control;
using AspNetCore_MVC_Project.Models.Factory;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// API-контроллер для управления пользователями, компаниями, разрешениями, Jobs и Statistics.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")] // Доступ только для администраторов
    public class AdminApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Конструктор AdminApiController.
        /// </summary>
        /// <param name="userManager">Менеджер пользователей Identity.</param>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        public AdminApiController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        // ============================== Вспомогательный метод для получения CompanyDbContext ==============================

        /// <summary>
        /// Получает контекст базы данных компании по ее идентификатору.
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

        // ============================== Работа с пользователями ==============================

        /// <summary>
        /// Получает список всех пользователей с их компаниями.
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Factory) // Загружаем данные о компании
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    Company = u.Factory != null ? u.Factory.Title : "Нет компании"
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Создает нового пользователя.
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Пользователь с таким Email уже существует." });

            var company = await _context.Factories.FindAsync(model.CompanyId);
            if (company == null)
                return BadRequest(new { message = "Компания не найдена." });

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FactoryId = company.Id };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new { message = "Ошибка при создании пользователя." });

            return Ok(new { message = "Пользователь успешно создан." });
        }

        /// <summary>
        /// Удаляет пользователя по ID.
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "Пользователь не найден." });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return BadRequest(new { message = "Ошибка при удалении пользователя." });

            return Ok(new { message = "Пользователь успешно удален." });
        }

        // ============================== Работа с компаниями ==============================

        /// <summary>
        /// Получает список всех компаний.
        /// </summary>
        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _context.Factories
                .Select(c => new { c.Id, c.Title })
                .ToListAsync();

            return Ok(companies);
        }

        /// <summary>
        /// Создает новую компанию.
        /// </summary>
        [HttpPost("companies")]
        public async Task<IActionResult> CreateCompany([FromBody] Factory company)
        {
            if (string.IsNullOrWhiteSpace(company.Title))
                return BadRequest(new { message = "Название компании не может быть пустым." });

            _context.Factories.Add(company);
            await _context.SaveChangesAsync();

            // Создание базы данных компании
            var dbContext = await GetCompanyDbContext(company.Id);
            if (dbContext == null)
                return BadRequest(new { message = "Ошибка при создании базы компании." });

            dbContext.Database.EnsureCreated(); // Создаем базу

            // Добавляем записи в таблицу Mark
            var user = await _userManager.GetUserAsync(User);
            var creatorName = user != null ? user.UserName : "Unknown";

            dbContext.Marks.Add(new Mark { Title = "Creator", Value = creatorName });
            dbContext.Marks.Add(new Mark { Title = "Service", Value = "0" });

            await dbContext.SaveChangesAsync(); // Сохраняем изменения

            return CreatedAtAction(nameof(GetCompanies), new { id = company.Id }, company);
        }

        /// <summary>
        /// Удаляет компанию по ID.
        /// </summary>
        [HttpDelete("companies/{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Factories.FindAsync(id);
            if (company == null) return NotFound(new { message = "Компания не найдена." });

            _context.Factories.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Компания успешно удалена." });
        }

        // ============================== Работа с разрешениями (модулями) ==============================

        /// <summary>
        /// Получает список разрешений компании.
        /// </summary>
        [HttpGet("options/{companyId}")]
        public async Task<IActionResult> GetOptions(int companyId)
        {
            var modules = await _context.Purchases
                .Where(bm => bm.FactoryId == companyId)
                .Select(bm => bm.OptionBlock.NameController) // Получаем NameController из OptionBlock
                .ToListAsync();

            return Ok(modules);
        }

        // ============================== Работа с таблицей Jobs ==============================

        /// <summary>
        /// Получает список всех записей Jobs для компании.
        /// </summary>
        [HttpGet("jobs/{companyId}")]
        public async Task<IActionResult> GetJobs(int companyId)
        {
            var dbContext = await GetCompanyDbContext(companyId);
            if (dbContext == null) return NotFound(new { message = "Компания не найдена." });

            var jobs = await dbContext.Jobs.ToListAsync();
            return Ok(jobs);
        }

        // ============================== Работа с таблицей Statistics ==============================

        /// <summary>
        /// Получает список всех записей Statistics для компании.
        /// </summary>
        [HttpGet("statistics/{companyId}")]
        public async Task<IActionResult> GetStatistics(int companyId)
        {
            var dbContext = await GetCompanyDbContext(companyId);
            if (dbContext == null) return NotFound(new { message = "Компания не найдена." });

            var statistics = await dbContext.Statistics.ToListAsync();
            return Ok(statistics);
        }
    }

    /// <summary>
    /// DTO-модель для создания пользователя через API.
    /// </summary>
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int CompanyId { get; set; }
    }
}

