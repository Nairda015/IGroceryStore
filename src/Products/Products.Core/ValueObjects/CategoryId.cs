using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class CategoryId
{
    public ulong Value { get; }
    private CategoryId() : this(0)
    {}

    public CategoryId(ulong value)
    {
        //TODO: Validate
        //if (value ) throw new InvalidCategoryIdException();
        Value = value;
    }
    
    public static implicit operator ulong(CategoryId id) => id.Value;
    public static implicit operator CategoryId(ulong id) => new(id);
}

public class InvalidCategoryIdException : GroceryStoreException
{
    public InvalidCategoryIdException() : base("Invalid Category Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}