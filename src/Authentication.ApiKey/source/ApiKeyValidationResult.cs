using System;
using System.Security.Claims;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// 
/// </summary>
public class ApiKeyValidationResult
{
    /// <summary>
    /// 
    /// </summary>
    public ClaimsIdentity Identity { get; protected set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Exception Exception { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static ApiKeyValidationResult Failed(Exception exception) => new ApiKeyValidationResult() { Exception = exception };
    /// <summary>
    /// 
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static ApiKeyValidationResult Success(ClaimsIdentity identity) => new ApiKeyValidationResult() { Identity = identity };
}