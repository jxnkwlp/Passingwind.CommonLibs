using Microsoft.AspNetCore.Authentication;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// Inherited from <see cref="AuthenticationSchemeOptions"/> to allow extra option properties for 'ApiKey' authentication.
/// </summary>
public class ApiKeyOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string? HeaderName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? QueryStringName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? AuthenticationSchemeName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Realm { get; set; }

    /// <summary>
    /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
    /// </summary>
    public string Challenge { get; set; } = ApiKeyDefaults.AuthenticationScheme;

    /// <summary>
    /// Defines whether the apikey token should be stored in the
    /// <see cref="AuthenticationProperties"/> after a successful authorization.
    /// </summary>
    public bool SaveToken { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public new ApiKeyEvents Events
    {
        get { return (ApiKeyEvents)base.Events!; }
        set { base.Events = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public ApiKeyOptions()
    {
        HeaderName = ApiKeyDefaults.HeaderName;
        QueryStringName = ApiKeyDefaults.QueryStringName;
        AuthenticationSchemeName = ApiKeyDefaults.AuthenticationSchemeName;
    }
}
