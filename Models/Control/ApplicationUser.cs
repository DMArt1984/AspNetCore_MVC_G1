using Microsoft.AspNetCore.Identity;

namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// Расширенная модель пользователя для системы аутентификации.
    /// Наследуется от IdentityUser, добавляя связь с компанией.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Идентификатор компании, к которой принадлежит пользователь.
        /// Может быть null, если пользователь не привязан к конкретной компании.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Навигационное свойство для связи с сущностью Company.
        /// Позволяет получить данные о компании, к которой принадлежит пользователь.
        /// </summary>
        public virtual Company Company { get; set; }
    }
}
