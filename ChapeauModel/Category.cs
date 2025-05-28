using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauG5.ChapeauModel
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name; // Optional, helps with debugging
    }
}
