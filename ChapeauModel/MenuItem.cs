using ChapeauG5.ChapeauEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauG5.ChapeauModels
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int CategoryID { get; set; }


        [Required]
        public VATCategory VAT_Percentage { get; set; } = VATCategory.low;

        public bool IsActive { get; set; } = true;


        public CourseType? CourseType { get; set; }


        public MenuCategory Category { get; set; }

        // Calculated properties for UI
        public bool IsOutOfStock => Stock <= 0;
        public bool IsLowStock => Stock > 0 && Stock <= 10;

        // Calculate VAT amount based on enum value
        public decimal VAT_Rate => VAT_Percentage == VATCategory.high ? 21m : 9m;
        public decimal PriceWithVAT => Price * (1 + VAT_Rate / 100);

        // To check if this is a drink item
        public bool IsDrink => Category?.MenuCard == MenuCard.Drinks;
        public bool IsAlcoholic { get; set; } = false;
    }

}
