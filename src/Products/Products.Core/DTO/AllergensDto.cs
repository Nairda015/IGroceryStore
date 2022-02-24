namespace IGroceryStore.Products.Core.DTO;

public class AllergensDto
{
    public AllergensDto(string name, string code)
    {
        Name = name;
        Code = code;
    }
    public string Name { get; set; }
    public string Code { get; set; }
}