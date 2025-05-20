using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauUI
{
    public class EmployeeDummy
    {
        public string Name { get; set; }
        public string Role { get; set; }

        public override string ToString() => $"{Name} ({Role})";
    }
}
