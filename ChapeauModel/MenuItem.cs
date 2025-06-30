using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChapeauModel
{
    public enum CourseType
    {
        Starter,
        Main,
        Dessert,
        Drink
    }

    public class MenuCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string MenuCard { get; set; }
    }

    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Menu CategoryId { get; set; }
        public int VatPercentage { get; set; }
        public bool IsActive { get; set; }
        public CourseType CourseType { get; set; }
        public bool IsAlcoholic { get; set; }

        // For display in order view
        public string DisplayPrice => $"€{Price:0.00}";
        public string DisplayInfo => $"{Name} - {DisplayPrice}";

        
    }
}
