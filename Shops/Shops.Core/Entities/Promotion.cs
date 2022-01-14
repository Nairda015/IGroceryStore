namespace IGroceryStore.Shops.Core.Entities;

public abstract class Promotion
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public abstract bool CheckIfPromotionIsApplicable();
    public abstract double CalculateDiscount();
}