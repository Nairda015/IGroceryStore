namespace IGroceryStore.UserBasket.Core.ValueObjects;

public record BasketName
{
    public string Value { get; }

    public BasketName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new NotImplementedException();
        Value = name;
    }
    
    public static implicit operator string(BasketName basketName) => basketName.Value;
    public static implicit operator BasketName(string name) => new(name);
}