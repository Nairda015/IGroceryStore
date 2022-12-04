﻿using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Shared.ValueObjects;

public class ProductName
{
    private ProductName()
    {
    }

    public ProductName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidProductNameException();

        Value = value;
    }

    public string Value { get; }

    public static implicit operator ProductName(string productName)
    {
        return new(productName);
    }

    public static implicit operator string(ProductName productName)
    {
        return productName.Value;
    }
}