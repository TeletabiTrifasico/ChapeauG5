namespace ChapeauModel
{
    public class Menu
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string MenuCard { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}