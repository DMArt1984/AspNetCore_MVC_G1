using System.ComponentModel.DataAnnotations;

namespace AspNetCore_MVC_Project.ViewModels
{
    /// <summary>
    /// Модель представления для входа в систему.
    /// Используется в форме авторизации пользователей.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Email пользователя, используемый для входа.
        /// Обязательное поле с проверкой на корректный email.
        /// </summary>
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// Обязательное поле с скрытым вводом.
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Запомнить пользователя в системе (чекбокс).
        /// Позволяет сохранять сессию пользователя после закрытия браузера.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}

