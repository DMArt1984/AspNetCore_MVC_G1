using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore_MVC_Project.Models;
using AspNetCore_MVC_Project.ViewModels;
using AspNetCore_MVC_Project.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, 
                                 UserManager<ApplicationUser> userManager, 
                                 ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded) return RedirectToAction("Index", "Home");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверяем, существует ли компания
                var company = _context.Companies.FirstOrDefault(c => c.Name == model.CompanyName);
                if (company == null)
                {
                    // Создаем новую компанию
                    company = new Company { Name = model.CompanyName };
                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();
                }

                // Создаем пользователя и привязываем к компании
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, CompanyId = company.Id };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Добавляем модули пользователя в таблицу BuyModules
                    foreach (var module in model.SelectedModules)
                    {
                        _context.BuyModules.Add(new BuyModule { NameController = module, CompanyId = company.Id });
                    }
                    await _context.SaveChangesAsync();

                    // Создаем новую базу данных для компании, если её нет
                    string databaseName = $"w{company.Name.Replace(" ", "_")}";
                    string masterConnectionString = "Server=DESKTOP-PVGO5SO\\MSSQLSERVER01;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

                    using (var connection = new SqlConnection(masterConnectionString))
                    {
                        connection.Open();
                        var createDbCommand = new SqlCommand($@"
                            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}') 
                            BEGIN
                                CREATE DATABASE {databaseName};
                            END", connection);
                        createDbCommand.ExecuteNonQuery();
                    }

                    // Создаем контекст базы данных с правильной строкой подключения
                    string newDbConnectionString = $"Server=DESKTOP-PVGO5SO\\MSSQLSERVER01;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;";

                    using (var dbContext = new CompanyDbContext(newDbConnectionString))
                    {
                        dbContext.Database.EnsureCreated();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
