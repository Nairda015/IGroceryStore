using System.Diagnostics;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Entities;
using IGroceryStore.Users.Persistence.Mongo.DbModels;
using IGroceryStore.Users.ValueObjects;
using MongoDB.Driver;

namespace IGroceryStore.Users.Persistence.Mongo;

internal interface IUserRepository
{
    Task<User?> GetAsync(UserId id, CancellationToken cancellationToken);
    Task<User?> GetAsync(Email email, CancellationToken cancellationToken);
    Task<bool> ExistByEmailAsync(string email,  CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
}

internal class UsersRepository : IUserRepository
{
    private readonly IMongoCollection<UserDbModel> _usersCollection;

    public UsersRepository(IMongoCollection<UserDbModel> usersCollection)
    {
        _usersCollection = usersCollection;
    }

    public async Task<User?> GetAsync(UserId id, CancellationToken cancellationToken)
    {
        var model = await _usersCollection
            .Find(x => x.Id == id.Value.ToString())
            .FirstOrDefaultAsync();
        
        throw new NotImplementedException();
    }

    public Task<User?> GetAsync(Email email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var documentsCount = await _usersCollection
            .Find(x => x.Email == email)
            .CountDocumentsAsync(cancellationToken);
        return documentsCount > 0;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        if (await ExistByEmailAsync(user.Email, cancellationToken)) 
            throw new UnreachableException("This should be validated on handler level");

        // var userDbModel = new UserDbModel
        // {
        //     Id = user.Id.ToString(),
        //     FirstName = user.FirstName,
        //     LastName = user.LastName,
        //     Email = user.Email,
        //     
        // };
        throw new NotImplementedException();

        //await _usersCollection.InsertOneAsync(userDbModel);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
