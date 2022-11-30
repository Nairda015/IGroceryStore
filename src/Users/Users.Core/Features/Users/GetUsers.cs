using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Users.Persistence.Contexts;
using IGroceryStore.Users.ReadModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Features.Users;

internal record GetUsers : IHttpQuery;
internal record UsersReadModel(IEnumerable<UserReadModel> Users, int Count);


public class GetUsersEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Users.MapGet<GetUsers, GetUsersHandler>("")
            .Produces<UsersReadModel>()
            .Produces(401);
}

internal class GetUsersHandler : IHttpQueryHandler<GetUsers>
{
    private readonly UsersDbContext _dbContext;

    public GetUsersHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> HandleAsync(GetUsers query, CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .Select(x => new UserReadModel(x.Id, x.FirstName, x.LastName, x.Email))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new UsersReadModel(users, users.Count);
        return Results.Ok(result);
    }
}
