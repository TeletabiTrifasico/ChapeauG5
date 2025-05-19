using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauModels
{ 
    public class MenuCategory
    {
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public MenuCard MenuCard { get; set; }
    }
}
