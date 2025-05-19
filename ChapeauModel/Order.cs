using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required]
        public int TableID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public DateTime DateCreatedAt { get; set; } = DateTime.Now;

        public bool IsDone { get; set; } = false;


        public Table Table { get; set; }
        public Employee Employee { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


        public Invoice Invoice { get; set; }


        public decimal TotalAmountBeforeVAT
        {
            get
            {
                return OrderItems.Sum(item => item.TotalPrice);
            }
        }

        public decimal TotalAmountWithVAT
        {
            get
            {
                return OrderItems.Sum(item => item.TotalPriceWithVAT);
            }
        }

        public decimal TotalVAT
        {
            get
            {
                return TotalAmountWithVAT - TotalAmountBeforeVAT;
            }
        }

        public int WaitingMinutes => (int)(DateTime.Now - DateCreatedAt).TotalMinutes;

        // Map old status property to IsDone
        public OrderStatus Status => IsDone ? OrderStatus.Finished : OrderStatus.Active;
    }
}
