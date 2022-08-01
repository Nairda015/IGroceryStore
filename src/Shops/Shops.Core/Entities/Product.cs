namespace IGroceryStore.Shops.Core.Entities;

public class Product
{
    public ulong Id { get; init; }
    public string Name { get; set; }
    public string BasePrice { get; set; }
    public Promotion Promotion { get; set; }
    public DateOnly LastUpdated { get; set; }    
}