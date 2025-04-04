﻿using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AspNetCore_MVC_Project.Models.Control;

namespace AspNetCore_MVC_Project.ViewModels
{
    /// <summary>
    /// Модель представления для регистрации нового пользователя.
    /// Используется в форме регистрации.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Email пользователя, используемый для регистрации.
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
        /// Подтверждение пароля.
        /// Должно совпадать с основным паролем.
        /// </summary>
        [Required(ErrorMessage = "Подтверждение пароля обязательно.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Название компании, к которой будет привязан пользователь.
        /// Обязательное поле.
        /// </summary>
        [Required(ErrorMessage = "Название компании обязательно.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Список идентификаторов модулей, выбранных пользователем при регистрации.
        /// Каждый идентификатор соответствует записи в OptionBlock.Id.
        /// </summary>
        [Display(Name = "Select Modules")]
        public List<int> SelectedModules { get; set; } = new List<int>();

        /// <summary>
        /// Опциональное свойство для отображения списка доступных модулей (OptionBlock) в представлении.
        /// Если нужно динамически вывести чекбоксы или другой интерфейс выбора модулей.
        /// </summary>
        public List<OptionBlock> AvailableModules { get; set; } = new List<OptionBlock>();
    }
}
