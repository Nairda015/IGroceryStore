using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

public record GetUsers() : IQuery<UsersReadModel>;

public class GetUsersController : UsersControllerBase
{
    private readonly IQueryDispatcher _dispatcher;

    public GetUsersController(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpGet("/users")]
    [Authorize]
    public async Task<UsersReadModel> Get()
    {
        var result = await _dispatcher.QueryAsync(new GetUsers());
        return result;
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