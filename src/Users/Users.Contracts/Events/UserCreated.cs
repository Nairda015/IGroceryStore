namespace IGroceryStore.Users.Contracts.Events;

public record UserCreated(Guid UserId, string FirstName, string LastName);