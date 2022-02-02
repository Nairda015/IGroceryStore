using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Persistence;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.ReadModels;
using IGroceryStore.Users.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Features.Users;

public record GetUser(UserId Id) : IQuery<UserReadModel>;

public class GetUserController : ApiControllerBase
{
    private readonly IQueryDispatcher _dispatcher;

    public GetUserController(IQueryDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpGet("/users/{userId:guid}")]
    public async Task<UserReadModel> Get(Guid userId)
    {
        var result = await _dispatcher.QueryAsync(new GetUser(userId));
        return result;
    }
}


public class GetUserHandler : IQueryHandler<GetUser, UserReadModel>
{
    private readonly UsersDbContext _dbContext;

    public GetUserHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserReadModel> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);
        if (user is null) throw new UserNotFoundException(query.Id);
        
        return new UserReadModel(user.Id, user.FirstName, user.LastName, user.Email);
    }
}