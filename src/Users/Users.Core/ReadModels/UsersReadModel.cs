namespace IGroceryStore.Users.Core.ReadModels;

public record UsersReadModel(IEnumerable<UserReadModel> Users , int Count);