using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_MVC_Project.Areas.BOX.Data
{
    public class MarketItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string Category { get; set; }

        // Количество доступных единиц товара
        public int Stock { get; set; }
    }
}
