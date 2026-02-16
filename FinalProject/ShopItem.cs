public class ShopItem
{
    public string Name { get; }
    public int Price { get; }

    public ShopItem(string name, int price)
    {
        Name = name;
        Price = price;
    }
}