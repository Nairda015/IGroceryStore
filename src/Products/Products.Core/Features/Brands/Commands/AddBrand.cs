﻿using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Brands.Commands
{
    internal record AddBrand(AddBrand.AddBrandBody Body) : IHttpCommand
    {
        internal record AddBrandBody(string Name);
    }

    public class AddBrandEndpoint : IEndpoint
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddBrand>("api/brands")
            .WithTags(SwaggerTags.Products)
            .Produces(201);
    }

    internal class AddBrandHandler : ICommandHandler<AddBrand, IResult>
    {
        private readonly ProductsDbContext _productsDbContext;
        private readonly ISnowflakeService _snowflakeService;

        public AddBrandHandler(ProductsDbContext productsDbContext, ISnowflakeService snowflakeService)
        {
            _productsDbContext = productsDbContext;
            _snowflakeService = snowflakeService;
        }

        public async Task<IResult> HandleAsync(AddBrand command, CancellationToken cancellationToken = default)
        {
            var brand = new Brand
            {
                Id = _snowflakeService.GenerateId(),
                Name = command.Body.Name
            };

            _productsDbContext.Brands.Add(brand);
            await _productsDbContext.SaveChangesAsync(cancellationToken);
            return Results.CreatedAtRoute($"api/brands/{brand.Id}");
        }
    }

}