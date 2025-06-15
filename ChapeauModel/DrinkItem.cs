namespace ChapeauModel
{
    public class DrinkItem
    {
        public int DrinkItemId { get; set; }
        public string Name { get; set; }
        public MenuItem MenuItemId { get; set; }
        public bool IsAlcoholic { get; set; }
        public DrinkCategory DrinkCategory { get; set; }
    }
    public enum DrinkCategory
        {
            SoftDrinks,
            Beers,
            Wines,
            Spirits,
            CoffeeAndTea
        }
}