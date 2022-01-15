using IGroceryStore.Products.Core.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public record BarCode
{
    public BarCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidBarCodeException();
        
        Value = value;
    }

    public string Value { get; }

    public static implicit operator BarCode(string barcode) => new(barcode);
    
    public static implicit operator string(BarCode barCode) => barCode.Value;
}