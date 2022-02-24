using System;
using FluentAssertions;
using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.Exceptions;
using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Exceptions;
using Moq;
using Xunit;

namespace Baskets.UnitTests;

public class BasketFactoryTests
{
    private static readonly Guid UserId = new("{B0B8F8E8-B8E8-4B8E-B8E8-B8E8B8E8B8E8}"); 
    private static readonly Mock<ICurrentUserService> CurrentUserService = new();
    
    [Fact]
    public void Create_ReturnsBasket()
    {
        CurrentUserService.Setup(x => x.UserId)
            .Returns(UserId);
        
        var sut = new BasketFactory(CurrentUserService.Object);
        var basket = sut.Create("Codzienne");

        basket.Should().BeOfType<Basket>();
        basket.Name.Value.Should().Be("Codzienne");
        basket.OwnerId.Value.Should().Be(UserId);
    }

[Theory]
[InlineData("00000000-0000-0000-0000-000000000000")]
[InlineData(null)]

    public void Create_ShouldThrow_WhenUserIdIsInvalid(Guid userId)
    {
        
        CurrentUserService.Setup(x => x.UserId)
            .Returns(userId);
        
        var sut = new BasketFactory(CurrentUserService.Object);
        
        Action action = () => sut.Create("Codzienne");

        action.Should().Throw<InvalidUserIdException>()
            .WithMessage("Invalid User Id");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_ShouldThrow_WhenBasketNameIsInvalid(string basketName)
    {
        CurrentUserService.Setup(x => x.UserId)
            .Returns(UserId);
        
        var sut = new BasketFactory(CurrentUserService.Object);
        
        Action action = () => sut.Create(basketName);
        
        action.Should().Throw<InvalidBasketNameException>()
            .WithMessage($"Invalid basket name: {basketName}");
    }
}