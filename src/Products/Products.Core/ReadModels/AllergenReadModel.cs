namespace IGroceryStore.Products.Core.ReadModels;

public record AllergenReadModel(string Name, string Code);

public record AllergenReadModelWithId(ulong Id, string Name, string Code) : AllergenReadModel(Name, Code);