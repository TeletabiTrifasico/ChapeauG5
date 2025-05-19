using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class Payment
    {
        public int PaymentID { get; set; }

        [Required]
        public int InvoiceID { get; set; }

        [MaxLength(200)]
        public string Feedback { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        // VAT percentage using enum
        [Required]
        public VATCategory VatPercentage { get; set; } = VATCategory.low;

        [Range(0, double.MaxValue)]
        public decimal TipAmount { get; set; } = 0;


        public Invoice Invoice { get; set; }


        public decimal TotalAmount => Amount + TipAmount;

        public decimal VATRate => VatPercentage == VATCategory.high ? 21m : 9m;
    }
}
