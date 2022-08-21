using Bogus;
using IGroceryStore.Users.Core.Features.Users;

namespace Users.IntegrationTests;

internal sealed class RegisterFaker : Faker<Register>
{
    public RegisterFaker()
    {
        CustomInstantiator(ResolveConstructor);
    }
   
    private Register ResolveConstructor(Faker faker)
    {
        var password = Guid.NewGuid().ToString();
        
        var body = new Register.RegisterBody(
            faker.Person.Email,
            password,
            password,
            faker.Person.FirstName,
            faker.Person.LastName);
        
        return new Register(body);
    }
}