using IGroceryStore.Products.Contracts.Events;
using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Products.Commands;

internal record UpdateDetails(UpdateDetails.UpdateDetailsBody DetailsBody, ulong Id) : IHttpCommand
{
    internal record UpdateDetailsBody(string Name,
        string Description
        );
}

public class UpdateDetailsEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
        => endpoints.MapPut<UpdateDetails>("api/products")
        .WithTags(SwaggerTags.Products)
        .Produces(204)
        .Produces(400);
}

internal class UpdateDetailsHandler : ICommandHandler<UpdateDetails, IResult>
{
    private readonly ProductsDbContext _context;
    private readonly IBus _bus;

    public UpdateDetailsHandler(ProductsDbContext context, IBus bus)
    {
        _context = context;
        _bus = bus;
    }

    public async Task<IResult> HandleAsync(UpdateDetails command, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id.Equals(command.Id), cancellationToken);

        if (product is null) throw new ProductNotFoundException(command.Id);

        var (name, description) = command.DetailsBody;

        product.Name = name;
        product.Description = description;
        _context.Update(product);
        await _context.SaveChangesAsync(cancellationToken);

        await _bus.Publish(new ProductUpdated(command.Id, name), cancellationToken);

        return Results.NoContent();
    }
}
