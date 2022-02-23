using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class CategoryId
{
    public Guid Value { get; }

    public CategoryId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidCategoryIdException();
        Value = value;
    }
    
    public static implicit operator Guid(CategoryId id) => id.Value;
    public static implicit operator CategoryId(Guid id) => new(id);
}

public class InvalidCategoryIdException : GroceryStoreException
{
    public InvalidCategoryIdException() : base("Invalid Category Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}