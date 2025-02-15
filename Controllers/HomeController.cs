using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_MVC_Project.Controllers
{
    /// <summary>
    /// ���������� ������� ��������.
    /// �������� �� ����������� ��������� �������� ����������.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// ����������� HomeController.
        /// </summary>
        /// <param name="configuration">������������ ����������.</param>
        public HomeController(IConfiguration configuration) : base(configuration) { }

        /// <summary>
        /// ���������� ������� ��������.
        /// </summary>
        /// <returns>������������� ������� ��������.</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}