﻿using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class PasswordDoesNotMatchException : GroceryStoreException
{
    public PasswordDoesNotMatchException() : base("Password does not match")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
