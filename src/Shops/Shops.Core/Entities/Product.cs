namespace IGroceryStore.Shops.Entities;

public class Product
{
    public required ulong Id { get; init; }
    public required string Name { get; set; }
    public Promotion? Promotion { get; set; }
    public required DateOnly LastUpdated { get; set; }
    public decimal? LowestPrice => _basePriceAtShop.MinBy(x => x.Value).Value;
    private readonly Dictionary<ulong, decimal> _basePriceAtShop = new();
    public Dictionary<ulong, decimal> BasePriceAtShop => _basePriceAtShop;
    
    public void UpdatePrice(ulong shopId, decimal price)
    {
        if (_basePriceAtShop.ContainsKey(shopId))
        {
            _basePriceAtShop[shopId] = price;
        }
        else
        {
            _basePriceAtShop.Add(shopId, price);
        }
    }
    
    public bool DoesPriceChange(ulong shopId, decimal price)
    {
        if (!_basePriceAtShop.ContainsKey(shopId)) return true;
        return _basePriceAtShop[shopId] != price;
    }
    
    public bool IsPriceLower(ulong shopId, decimal price)
    {
        if (!_basePriceAtShop.ContainsKey(shopId)) return true;
        
        return _basePriceAtShop[shopId] > price;
    }
}
