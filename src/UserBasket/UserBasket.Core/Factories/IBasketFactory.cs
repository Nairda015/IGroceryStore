using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.UserBasket.Core.Entities;
using IGroceryStore.UserBasket.Core.ValueObjects;

namespace IGroceryStore.UserBasket.Core.Factories;

internal sealed class BasketFactory : IBasketFactory
{
    private readonly ICurrentUserService _currentUserService;

    public BasketFactory(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Basket Create(BasketName name)
     => new(Guid.NewGuid(), _currentUserService.UserId ?? Guid.NewGuid(), name);
}

internal interface IBasketFactory
{
    public Basket Create(BasketName name);
}