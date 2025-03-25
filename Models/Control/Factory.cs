using System.Collections.Generic;

namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// ������ ��������, � ������� ����� ������������ ������������.
    /// ������ �������� ����� ����� ��������� ������������� � ��������� �������.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// ���������� ������������� ��������
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// �������� ��������
        /// ������ ���� ���������� � �������
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ������������� �������� ��� ������ �������������, ������������� ��������
        /// ���� �� ������: ���� �������� - ����� �������������
        /// </summary>
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// ������������� �������� ��� ������ �������, ��������� ��������
        /// ���� �� ������: ���� �������� - ����� �������
        /// </summary>
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
