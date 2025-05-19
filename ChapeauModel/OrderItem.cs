using ChapeauG5.ChapeauEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauG5.ChapeauModels
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int MenuItemID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [MaxLength(200)]
        public string Comment { get; set; }

        [Required]
        public DateTime DateCreatedAt { get; set; } = DateTime.Now;

        public OrderItemStatus Status { get; set; } = OrderItemStatus.Ordered;


        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }


        public decimal TotalPrice => (MenuItem?.Price ?? 0) * Quantity;

        // Update VAT calculation
        public decimal TotalPriceWithVAT
        {
            get
            {
                if (MenuItem == null) return 0;
                decimal vatRate = MenuItem.VAT_Percentage == VATCategory.high ? 21m : 9m;
                return TotalPrice * (1 + vatRate / 100);
            }
        }

        public int WaitingMinutes => (int)(DateTime.Now - DateCreatedAt).TotalMinutes;
    }
}
