using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class Invoice
    {
        public int InvoiceID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalVAT { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalTipAmount { get; set; } = 0;

        [Required]
        public DateTime DateCreatedAt { get; set; } = DateTime.Now;

        public Order Order { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}
