using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.DTO;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public QuantityDto Quantity { get; set; }
    public string BarCode { get; set; }
    public bool IsObsolete { get; set; }
    public string CountryName { get; set; }
    public string BrandName { get; set; }
    public string CategoryName { get; set; }
    public IEnumerable<AllergensDto> Allergens { get; set; }
    //TODO: Add img
}