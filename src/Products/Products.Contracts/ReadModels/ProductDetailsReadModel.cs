namespace IGroceryStore.Products.Contracts.ReadModels;

public class ProductDetailsReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public QuantityReadModel Quantity { get; set; }
    public string? BarCode { get; set; }
    public bool IsObsolete { get; set; }
    public string CountryName { get; set; }
    public string BrandName { get; set; }
    public string CategoryName { get; set; }
    public IEnumerable<AllergenReadModel> Allergens { get; set; } = new List<AllergenReadModel>();
    //TODO: Add img
}

public class ProductReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string BrandName { get; set; }
    public QuantityReadModel Quantity { get; set; }
}