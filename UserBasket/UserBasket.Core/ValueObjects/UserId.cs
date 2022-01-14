namespace IGroceryStore.UserBasket.Core.ValueObjects;

public record UserId
{
    public Guid Value { get; set; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new NotImplementedException();
        }
        
        Value = value;
    }
    
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);
    
}