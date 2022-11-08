namespace IGroceryStore.Shops.ValueObjects;

public record Adress
{
    public required string City { get; init; }
    public required string Street { get; init; }
    public required string HouseNumber { get; init; }
    public required Coordinates Coordinates { get; init; }
}

public record struct Coordinates(decimal Latitude, decimal Longitude);
