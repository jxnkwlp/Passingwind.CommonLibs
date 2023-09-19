# AspNetCore.Authentication.ApiKey

ASP.NET Core authentication handler for the ApiKey protocol

## Quickstart

``` cs
builder.Services
    .AddAuthentication()
    // api ApiKey scheme
    .AddApiKey<TestApiKeyProvider>();

// configure this if you default scheme is not 'ApiKey'
// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.ForwardDefaultSelector = (s) =>
//     {
//         var authorization = (string?)s.Request.Headers.Authorization;
//         if (authorization?.StartsWith(ApiKeyDefaults.AuthenticationSchemeName) == true)
//             return ApiKeyDefaults.AuthenticationScheme;
// 
//         // you default scheme
//         return IdentityConstants.ApplicationScheme;
//     };
// });
```

TestApiKeyProvider.cs

```cs 
public class TestApiKeyProvider : IApiKeyProvider
{ 
    public async Task<ApiKeyValidationResult> ValidateAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        // verification apiKey
        ...

        // if success
        return ApiKeyValidationResult.Success(...);

        // if fail
        return ApiKeyValidationResult.Failed(new Exception("invalid api key"));
    }
}
```