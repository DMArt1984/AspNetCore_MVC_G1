using Microsoft.AspNetCore.Identity;

namespace AspNetCore_MVC_Project.Models
{
    /// <summary>
    /// ����������� ������ ������������ ��� ������� ��������������.
    /// ����������� �� IdentityUser, �������� ����� � ���������.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// ������������� ��������, � ������� ����������� ������������.
        /// ����� ���� null, ���� ������������ �� �������� � ���������� ��������.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// ������������� �������� ��� ����� � ��������� Company.
        /// ��������� �������� ������ � ��������, � ������� ����������� ������������.
        /// </summary>
        public virtual Company Company { get; set; }
    }
}
