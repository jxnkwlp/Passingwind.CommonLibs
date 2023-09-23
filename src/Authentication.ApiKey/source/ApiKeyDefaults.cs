namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
///  Default value for ApiKey authentication
/// </summary>
public static class ApiKeyDefaults
{
    /// <summary>
    /// Default value for AuthenticationScheme 
    /// </summary>
    public const string AuthenticationScheme = "ApiKey";

    /// <summary>
    /// Default value for header name
    /// </summary>
    public const string HeaderName = "X-ApiKey";

    /// <summary>
    /// Default value for query name
    /// </summary>
    public const string QueryStringName = "x-apikey";

    /// <summary>
    /// Default value for header authentication name
    /// </summary>
    public const string HeaderAuthenticationSchemeName = "ApiKey";
}
