using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{
    public class DrinkItem
    {
        public int DrinkItemID { get; set; }

        [Required]
        public int DrinkMenuItemID { get; set; }

        public bool IsAlcoholic { get; set; } = false;

        [Required]
        public string DrinkCategory { get; set; }

        public MenuItem MenuItem { get; set; }
    }
}
