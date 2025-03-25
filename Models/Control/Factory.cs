using System.Collections.Generic;

namespace AspNetCore_MVC_Project.Models.Control
{
    /// <summary>
    /// Модель компании, к которой могут принадлежать пользователи.
    /// Каждая компания может иметь множество пользователей и доступных модулей.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// Уникальный идентификатор компании
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название компании
        /// Должно быть уникальным в системе
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Навигационное свойство для списка пользователей, принадлежащих компании
        /// Один ко многим: одна компания - много пользователей
        /// </summary>
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// Навигационное свойство для списка модулей, доступных компании
        /// Один ко многим: одна компания - много модулей
        /// </summary>
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
