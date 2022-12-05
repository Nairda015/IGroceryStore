using System.Text.Json.Serialization;

namespace IGroceryStore.Shared.Contracts;

public class UserCreated : IMessage
{
    [JsonPropertyName("id")] 
    public required Guid UserId { get; init; }
    
    [JsonPropertyName("firstName")] 
    public required string FirstName { get; init; }
    
    [JsonPropertyName("lastName")] 
    public required string LastName { get; init; }

    [Newtonsoft.Json.JsonIgnore]
    public string MessageTypeName => nameof(UserCreated);

    public void Deconstruct(out Guid userId, out string firstName, out string lastName)
    {
        userId = this.UserId;
        firstName = this.FirstName;
        lastName = this.LastName;
    }
}
