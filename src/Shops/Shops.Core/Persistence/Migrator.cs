// using IGroceryStore.Shared.Abstraction.Queries;
//
// namespace IGroceryStore.Shops.Persistence;
//
// internal record MigrateTable : IHttpQuery;
//
// public class MigrateTableEndpoint : IEndpoint2
// {
//     public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
//         builder.Shops.MapGet<MigrateTable>("migrate");
// }
//
// internal class MigrateTableHandler //: IHandler<MigrateTable>
// {
//
//     
// }
//
// public interface IEndpoint2
// {
//     void RegisterEndpoint(IGroceryStoreRouteBuilder builder);
// }

// public interface IHandler<in TQuery> where TQuery : IHttpQuery
// {
//     Task<IResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
// }
//
// public interface IHttpQuery { }
//
// public static class EndpointRouteBuilderExtensions
// {
//     public static RouteHandlerBuilder MapGet<TQuery, THandler>(this IEndpointRouteBuilder endpoints, string template) 
//         where TQuery : IHttpQuery
//         where THandler : IHandler<TQuery> =>
//         endpoints.MapGet(template, async (
//                 [FromServices] THandler handler,
//                 [AsParameters] TQuery query,
//                 CancellationToken cancellationToken) =>
//             await handler.HandleAsync(query, cancellationToken));
// }
