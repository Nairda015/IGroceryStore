using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Services;

namespace IGroceryStore.Baskets.Core.Factories;

internal sealed class BasketFactory : IBasketFactory
{
    private readonly ICurrentUserService _currentUserService;

    public BasketFactory(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Basket Create(BasketName name)
     => new(Guid.NewGuid(), _currentUserService.UserId, name);
}

internal interface IBasketFactory
{
    public Basket Create(BasketName name);
}