namespace IGroceryStore.Shops.Core.Common;

public record DateRange(DateOnly Start, DateOnly End)
{
    public bool Contains(DateOnly date) => date >= Start && date <= End;
}