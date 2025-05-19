using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class Table
    {
        public int TableID { get; set; }

        [Required]
        public int TableNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        public TableStatus Status { get; set; } = TableStatus.Free;

        // Additional properties for UI
        public bool HasActiveOrder { get; set; }
        public int? CurrentOrderID { get; set; }
        public DateTime? LastOrderTime { get; set; }
        public int WaitingMinutes { get; set; }
    }
}