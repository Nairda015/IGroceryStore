namespace IGroceryStore.Products.Core.ReadModels;

public class QuantityReadModel
{
    public QuantityReadModel(float amount, string unit)
    {
        Amount = amount;
        Unit = unit;
    }
    public float Amount { get; }
    public string Unit { get; }
}