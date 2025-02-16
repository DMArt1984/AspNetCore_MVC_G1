using System.Collections.Generic;

namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// ������ ��������, �������������� �����������, � ������� ����� ������������ ������������.
    /// ������ �������� ����� ����� ��������� ������������� � ��������� �������.
    /// </summary>
    public class Company
    {
        /// <summary>
        /// ���������� ������������� ��������.
        /// ������������� ������������ ����� ������.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// �������� ��������.
        /// ������ ���� ���������� � �������.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ������������� �������� ��� ������ �������������, ������������� ��������.
        /// ���� �� ������: ���� �������� - ����� �������������.
        /// </summary>
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// ������������� �������� ��� ������ �������, ��������� ��������.
        /// ���� �� ������: ���� �������� - ����� �������.
        /// </summary>
        public virtual ICollection<Purchase> BuyModules { get; set; } = new List<Purchase>();
    }
}
