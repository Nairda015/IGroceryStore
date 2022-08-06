﻿using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

internal sealed record BarCode
{
    public BarCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidBarCodeException();
        
        Value = value;
    }

    public string Value { get; }

    public static implicit operator BarCode(string barcode) => new(barcode);
    
    public static implicit operator string(BarCode barCode) => barCode.Value;
}

public class InvalidBarCodeException : GroceryStoreException
{
    public InvalidBarCodeException() : base("Invalid bar code")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}