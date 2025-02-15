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
    /// ���������� ��� ������ � KPI-�������.
    /// �������� �� ������ ������������� � ������ ������� Statistic.
    /// </summary>
    [Authorize] // ������������ ������ ������ ��� �������������� �������������
    public class KPIController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// ����������� ����������� KPI.
        /// </summary>
        /// <param name="userManager">�������� ������������� Identity.</param>
        /// <param name="context">�������� �������� ���� ������.</param>
        /// <param name="configuration">������������ ����������.</param>
        public KPIController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
            : base(configuration) // �������� ����������� BaseController
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// �������� �������� ���� ������ ��������.
        /// ���������, ���� �� � ������������ ������ � ������ "KPI".
        /// </summary>
        /// <returns>�������� ���� ������ �������� ��� null, ���� ������ ��������.</returns>
        private async Task<CompanyDbContext> GetCompanyDbContext()
        {
            // �������� �������� ������������
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return null;

            // ������� �������� ������������
            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null) return null;

            // ���������, ���� �� � �������� ������ � ������ "KPI"
            bool hasAccess = await _context.BuyModules.AnyAsync(bm => bm.CompanyId == user.CompanyId && bm.NameController == "KPI");
            if (!hasAccess) return null;

            // ��������� ������ ����������� � ������� �������� ��� ���� ������ ��������
            string connectionString = GetCompanyConnectionString($"w{company.Name.Replace(" ", "_")}");
            return new CompanyDbContext(connectionString);
        }

        /// <summary>
        /// ���������� ������ ������ �� ������� Statistic.
        /// �������� ������ �������������, � ������� ���� ������ � ������ "KPI".
        /// </summary>
        /// <returns>�������� � �������� ������ Statistic.</returns>
        public async Task<IActionResult> Index()
        {
            // �������� �������� ���� ������ ��������
            var dbContext = await GetCompanyDbContext();
            if (dbContext == null) return RedirectToAction("Index", "Home"); // ���� ������� ��� � �������������� �� ������� ��������

            // ��������� ��� ������ �� ������� Statistic
            var statistics = await dbContext.Statistics.ToListAsync();
            return View(statistics);
        }
    }
}
