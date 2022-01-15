namespace IGroceryStore.Stores.Core.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string BasePrice { get; set; }
    public Promotion Promotion { get; set; }
    
}