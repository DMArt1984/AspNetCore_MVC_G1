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
    [Authorize]
    public class KPIController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public KPIController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return null;

            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null) return null;

            // Проверка доступа к контроллеру KPI в BuyModules
            bool hasAccess = await _context.BuyModules.AnyAsync(bm => bm.CompanyId == user.CompanyId && bm.NameController == "KPI");
            if (!hasAccess) return null;

            string connectionString = $"Server=DESKTOP-PVGO5SO\\MSSQLSERVER01;Database=w{company.Name.Replace(" ", "_")};Trusted_Connection=True;TrustServerCertificate=True;";
            return new CompanyDbContext(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            var dbContext = await GetCompanyDbContext();
            if (dbContext == null) return RedirectToAction("Index", "Home");

            var statistics = await dbContext.Statistics.ToListAsync();
            return View(statistics);
        }
    }
}
