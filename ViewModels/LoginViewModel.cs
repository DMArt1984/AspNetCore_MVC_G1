using System.ComponentModel.DataAnnotations;

namespace AspNetCore_MVC_Project.ViewModels
{
    /// <summary>
    /// ������ ������������� ��� ����� � �������.
    /// ������������ � ����� ����������� �������������.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Email ������������, ������������ ��� �����.
        /// ������������ ���� � ��������� �� ���������� email.
        /// </summary>
        [Required(ErrorMessage = "Email ����������.")]
        [EmailAddress(ErrorMessage = "������� ���������� email.")]
        public string Email { get; set; }

        /// <summary>
        /// ������ ������������.
        /// ������������ ���� � ������� ������.
        /// </summary>
        [Required(ErrorMessage = "������ ����������.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// ��������� ������������ � ������� (�������).
        /// ��������� ��������� ������ ������������ ����� �������� ��������.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}

