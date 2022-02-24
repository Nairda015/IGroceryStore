namespace IGroceryStore.Products.Core.DTO;

public class QuantityDto
{
    public QuantityDto(float amount, string unit)
    {
        Amount = amount;
        Unit = unit;
    }
    public float Amount { get; }
    public string Unit { get; }
}