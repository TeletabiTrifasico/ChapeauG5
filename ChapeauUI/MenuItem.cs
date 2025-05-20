using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauUI
{
    public class MenuItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public override string ToString() => $"{Name} - ${Price:F2}";
    }
}
