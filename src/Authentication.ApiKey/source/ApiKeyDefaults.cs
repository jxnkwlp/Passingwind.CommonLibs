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
    /// 
    /// </summary>
    public const string HeaderName = "X-ApiKey";

    /// <summary>
    /// 
    /// </summary>
    public const string QueryStringName = "x-apikey";

    /// <summary>
    /// 
    /// </summary>
    public const string AuthenticationSchemeName = "ApiKey";
}
