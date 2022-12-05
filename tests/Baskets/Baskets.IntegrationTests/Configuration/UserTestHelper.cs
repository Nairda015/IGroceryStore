using IGroceryStore.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IGroceryStore.Baskets.IntegrationTests.Configuration;

// public static class UserTestHelper
// {
//     internal static async Task RemoveUserById<T>(this WebApplicationFactory<T> apiFactory, Guid id) where T: class, IApiMarker
//     {
//         using var scope = apiFactory.Services.CreateScope();
//         var collection = scope.ServiceProvider.GetRequiredService<IMongoCollection<UserDbModel>>();
//
//         var user = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();
//
//         user.Should().NotBeNull();
//         if (user is not null) await collection.DeleteOneAsync(x => x.Id == id);
//     }
// }
