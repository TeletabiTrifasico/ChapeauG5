using ChapeauModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauModel
{
    public class Menu
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public MenuCard MenuCard { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
    public enum MenuCard
    {
        Food,
        Drinks,
        Desserts
    }
}
