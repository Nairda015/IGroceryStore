using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

internal record GetUsers : IHttpQuery;
internal record UsersReadModel(IEnumerable<UserReadModel> Users, int Count);


public class GetUsersEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetUsers>("/users")
            .Produces<UsersReadModel>()
            .Produces(401)
            .RequireAuthorization()
            .WithTags(SwaggerTags.Users);
}

internal class GetUsersHandler : IQueryHandler<GetUsers, IResult>
{
    private readonly UsersDbContext _dbContext;

    public GetUsersHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> HandleAsync(GetUsers query, CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Select(x => new UserReadModel(x.Id, x.FirstName, x.LastName, x.Email))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new UsersReadModel(users, users.Count);
        return Results.Ok(result);
    }
}