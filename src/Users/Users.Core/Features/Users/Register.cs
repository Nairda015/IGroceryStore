using FluentValidation;
using FluentValidation.Results;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Validation;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Core.Factories;
using IGroceryStore.Users.Core.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ValidationResult = MassTransit.ValidationResult;

namespace IGroceryStore.Users.Core.Features.Users;

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
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<Register>("users/register")
            .Produces(202)
            .Produces(400)
            .Produces<ValidationResult>(400)
            .AddEndpointFilter<ValidationFilter<Register>>()
            .WithName(nameof(Register))
            .WithTags(SwaggerTags.Users);
}

internal class RegisterHandler : ICommandHandler<Register, IResult>
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
        return Results.AcceptedAtRoute(nameof(GetUser),new {Id = user.Id.Value});
    }
}

internal class CreateProductValidator : AbstractValidator<Register>
{
    public CreateProductValidator(UsersDbContext usersDbContext)
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
                var user = await usersDbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
                if (user is null) return;
                context.AddFailure(new ValidationFailure(nameof(Register.Body.Email), "Email already exists"));
            });

        RuleFor(x => x.Body.FirstName)
            .NotEmpty();

        RuleFor(x => x.Body.LastName)
            .NotEmpty();
    }
}