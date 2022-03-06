namespace IGroceryStore.Products.Core.ReadModels;

public class AllergenReadModel
{
    public AllergenReadModel(string name, string code)
    {
        Name = name;
        Code = code;
    }
    public string Name { get; set; }
    public string Code { get; set; }
}