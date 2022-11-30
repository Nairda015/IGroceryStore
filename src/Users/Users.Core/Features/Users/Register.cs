using FluentValidation;
using FluentValidation.Results;
using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.Filters;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Entities;
using IGroceryStore.Users.Persistence.Mongo;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
    private readonly IUserRepository _repository;
    private readonly IBus _bus;

    public RegisterHandler(IUserRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task<IResult> HandleAsync(Register command, CancellationToken cancellationToken)
    {
        var (email, password, _, firstName, lastName) = command.Body;

        var user = new User(password)
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };
        
        await _repository.AddAsync(user, cancellationToken);
        await _bus.Publish(new UserCreated(user.Id, firstName, lastName), cancellationToken);
        return Results.AcceptedAtRoute(nameof(GetUser),new {Id = user.Id.Value});
    }
}

internal class CreateProductValidator : AbstractValidator<Register>
{
    public CreateProductValidator(IUserRepository repository)
    {
        RuleFor(x => x.Body.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.Body.ConfirmPassword)
            .Equal(x => x.Body.Password);

        RuleFor(x => x.Body.Email)
            .NotEmpty()
            .EmailAddress()
            .CustomAsync(async (email, context, cancellationToken) =>
            {
                if (!await repository.ExistByEmailAsync(email, cancellationToken)) return;
                context.AddFailure(new ValidationFailure(nameof(Register.Body.Email), "Email already exists"));
            });

        RuleFor(x => x.Body.FirstName)
            .NotEmpty();

        RuleFor(x => x.Body.LastName)
            .NotEmpty();
    }
}
