using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Baskets.Core.Factories;

internal sealed class BasketFactory : IBasketFactory
{
    private readonly ICurrentUserService _currentUserService;

    public BasketFactory(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Basket Create(BasketName name)
    {
        var userId = _currentUserService.UserId ?? throw new InvalidUserIdException();
        return new Basket
        {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            Name = name
        };
    }
}

internal interface IBasketFactory
{
    public Basket Create(BasketName name);
}
