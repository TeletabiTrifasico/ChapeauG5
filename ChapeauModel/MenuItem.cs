using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauModel
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        

        public int CategoryID { get; set; } // Assuming CategoryID is used for foreign key reference
        public string Category { get; set; }
        public bool IsActive { get; set; }
    }
}
