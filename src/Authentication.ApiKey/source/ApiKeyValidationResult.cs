using System;
using System.Security.Claims;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <inheritdoc />
public class ApiKeyValidationResult
{
    /// <inheritdoc />
    public ClaimsIdentity Identity { get; protected set; } = null!;

    /// <inheritdoc />
    public Exception Exception { get; set; } = null!;

    /// <inheritdoc />
    public static ApiKeyValidationResult Failed(Exception exception) => new ApiKeyValidationResult() { Exception = exception };

    /// <inheritdoc />
    public static ApiKeyValidationResult Success(ClaimsIdentity identity) => new ApiKeyValidationResult() { Identity = identity };
}