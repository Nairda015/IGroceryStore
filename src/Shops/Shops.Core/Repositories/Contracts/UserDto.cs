using System.Text.Json.Serialization;

namespace IGroceryStore.Shops.Repositories.Contracts;

public class UserDto
{
    [JsonPropertyName("pk")] public string Pk => Id;
    [JsonPropertyName("sk")] public string Sk => Id;
    [JsonPropertyName("id")] public required string Id { get; init; }
    [JsonPropertyName("firstName")] public required string FirstName { get; init; }
    [JsonPropertyName("lastName")] public required string LastName { get; init; }
    [JsonPropertyName("trustLevel")] public uint TrustLevel { get; init; }
}
