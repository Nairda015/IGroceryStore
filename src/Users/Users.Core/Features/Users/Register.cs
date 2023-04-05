﻿using FluentValidation;
using FluentValidation.Results;
using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.Filters;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Factories;
using IGroceryStore.Users.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace IGroceryStore.Users.Features.Users;

internal record Register(Register.RegisterBody Body) : IHttpCommand
{
    public record RegisterBody(string Email,
        string Password,
        string ConfirmPassword,
        string FirstName,
        string LastName);
}

public class RegisterUserEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Users.MapPost<Register, RegisterHandler>("register")
            .Produces(202)
            .Produces(400)
            .Produces<ValidationResult>(400)
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<Register>>()
            .WithName(nameof(Register));
}

internal class RegisterHandler : IHttpCommandHandler<Register>
{
    private readonly IUserFactory _factory;
    private readonly UsersDbContext _context;
    private readonly IBus _bus;

    public RegisterHandler(IUserFactory factory, UsersDbContext context, IBus bus)
    {
        _factory = factory;
        _context = context;
        _bus = bus;
    }

    public async Task<IResult> HandleAsync(Register command, CancellationToken cancellationToken = default)
    {
        var (email, password, _, firstName, lastName) = command.Body;

        var user = _factory.Create(Guid.NewGuid(), firstName, lastName, email, password);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _bus.Publish(new UserCreated(user.Id, firstName, lastName), cancellationToken: cancellationToken);
        return Results.AcceptedAtRoute(nameof(GetUser), new { Id = user.Id.Value });
    }
}

internal class CreateProductValidator : AbstractValidator<Register>
{
    public CreateProductValidator(IValidator<Register.RegisterBody> bodyValidator)
    {
        RuleFor(x => x.Body)
            .SetValidator(bodyValidator);
    }
}

internal class RegisterBodyValidator : AbstractValidator<Register.RegisterBody>
{
    public RegisterBodyValidator(UsersDbContext usersDbContext)
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .CustomAsync(async (email, context, cancellationToken) =>
            {
                var user = await usersDbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
                if (user is null) return;
                context.AddFailure(new ValidationFailure(nameof(Register.RegisterBody.Email), "Email already exists"));
            });

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();
    }
}
