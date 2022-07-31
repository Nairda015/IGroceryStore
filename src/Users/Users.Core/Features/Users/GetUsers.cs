using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

public record GetUsers : IQuery<UsersReadModel>;

public class GetUsersEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users",
            [Authorize] async ([FromServices] IQueryDispatcher dispatcher) =>
                Results.Ok(await dispatcher.QueryAsync(new GetUsers())));
    }
}


public class GetUsersHandler : IQueryHandler<GetUsers, UsersReadModel>
{
    private readonly UsersDbContext _dbContext;

    public GetUsersHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UsersReadModel> HandleAsync(GetUsers query, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Select(x => new UserReadModel(x.Id, x.FirstName, x.LastName, x.Email))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

            return new UsersReadModel(users, users.Count);
    }
}