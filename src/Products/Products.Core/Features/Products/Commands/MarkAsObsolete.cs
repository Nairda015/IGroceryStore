﻿using IGroceryStore.Products.Exceptions;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Products.Commands;

internal record MarkAsObsolete(MarkAsObsolete.MarkAsObsoleteBody Body) : IHttpCommand
{
    internal record MarkAsObsoleteBody(ProductId Id);
}

public class MarkAsObsoleteEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<MarkAsObsolete>("api/products/mark-as-obsolete/{id}").WithTags(Constants.SwaggerTags.Products);
}

internal class MarkAsObsoleteHandler : ICommandHandler<MarkAsObsolete, IResult>
{
    private readonly ProductsDbContext _productsDbContext;

    public MarkAsObsoleteHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<IResult> HandleAsync(MarkAsObsolete command, CancellationToken cancellationToken = default)
    {
        var product = await _productsDbContext.Products.FirstOrDefaultAsync(x => x.Id == command.Body.Id, cancellationToken);
        if (product == null) throw new ProductNotFoundException(command.Body.Id);

        product.MarkAsObsolete();
        _productsDbContext.Update(product);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}
