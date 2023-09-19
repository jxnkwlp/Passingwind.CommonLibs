using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// An <see cref="AuthenticationHandler{TOptions}"/> that can perform ApiKey based authentication.
/// </summary>
public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
{
    /// <summary>
    /// 
    /// </summary>
    protected IApiKeyProvider ApiKeyProvider { get; }

    /// <summary>
    /// 
    /// </summary>
    protected new ApiKeyEvents Events { get => (ApiKeyEvents)base.Events!; set => base.Events = value; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    /// <param name="clock"></param>
    /// <param name="apiKeyProvider"></param>
    public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiKeyProvider apiKeyProvider) : base(options, logger, encoder, clock)
    {
        ApiKeyProvider = apiKeyProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override Task<object> CreateEventsAsync()
    {
        return Task.FromResult<object>(new ApiKeyEvents());
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var messageReceivedContext = new ApiKeyMessageReceivedContext(Context, Scheme, Options);

        await Events.MessageReceivedAsync(messageReceivedContext).ConfigureAwait(false);

        if (messageReceivedContext.Result != null)
            return messageReceivedContext.Result;

        var request = Request;

        var isApiKeyAvailable = CanHandle(request);

        if (!isApiKeyAvailable)
        {
            return AuthenticateResult.NoResult();
        }

        string? token = GetToken(request);

        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.NoResult();
        }
        else
        {
            Logger.LogDebug("The Api Key '{token}' was found in the request.", token);
        }

        try
        {
            var validationResult = await ApiKeyProvider.ValidateAsync(token).ConfigureAwait(false);

            if (validationResult.Exception != null)
            {
                Logger.LogError(validationResult.Exception, "The API Key verification failed by {0}.", ApiKeyProvider.GetType().Name);

                var authenticationFailedContext = new ApiKeyAuthenticationFailedContext(Context, Scheme, Options) { Exception = validationResult.Exception };

                await Events.AuthenticationFailedAsync(authenticationFailedContext).ConfigureAwait(false);

                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                return AuthenticateResult.Fail(authenticationFailedContext.Exception);
            }
            else
            {
                Logger.LogInformation("The API Key verification successful by {0}.", ApiKeyProvider.GetType().Name);
            }

            var identity = validationResult.Identity;

            var tokenValidatedContext = new ApiKeyTokenValidatedContext(Context, Scheme, Options)
            {
                Token = token,
                Principal = new ClaimsPrincipal(identity),
            };

            await Events.TokenValidatedAsync(tokenValidatedContext).ConfigureAwait(false);

            if (tokenValidatedContext.Result != null)
                return tokenValidatedContext.Result;

            if (Options.SaveToken)
            {
                tokenValidatedContext.Properties.StoreTokens(new[] { new AuthenticationToken() { Name = "token", Value = token } });
            }

            tokenValidatedContext.Success();

            return tokenValidatedContext.Result!;
        }
        catch (System.Exception exception)
        {
            Logger.LogError("Exception occurred while processing message.", exception);

            var authenticationFailedContext = new ApiKeyAuthenticationFailedContext(Context, Scheme, Options) { Exception = exception };

            await Events.AuthenticationFailedAsync(authenticationFailedContext).ConfigureAwait(false);

            if (authenticationFailedContext.Result != null)
            {
                return authenticationFailedContext.Result;
            }

            throw;
        }
    }

    /// <inheritdoc />
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await HandleAuthenticateOnceSafeAsync().ConfigureAwait(false);

        var eventContext = new ApiKeyChallengeContext(Context, Scheme, Options, properties)
        {
            AuthenticateFailure = authResult?.Failure
        };

        await Events.ChallengeAsync(eventContext).ConfigureAwait(false);

        if (eventContext.Handled)
        {
            return;
        }

        Response.StatusCode = 401;

        if (string.IsNullOrEmpty(eventContext.Error))
        {
            Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
        }
        else
        {
            // WWW-Authenticate: ApiKey realm="example", charset="UTF-8", in="authorization_header", in="header_or_query_params", key_name="mykey";

            var builder = new StringBuilder(Options.Challenge);

            if (!string.IsNullOrWhiteSpace(Options.Realm))
            {
                builder.Append($"realm=\"{Options.Realm}\"");
            }
            if (Options.Challenge.IndexOf(' ') > 0)
            {
                // Only add a comma after the first param, if any
                builder.Append(',');
            }
            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                builder.Append(" error=\"");
                builder.Append(eventContext.Error);
                builder.Append('\"');
            }

            // TODO

            Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        var forbiddenContext = new ApiKeyForbiddenContext(Context, Scheme, Options);

        return Events.ForbiddenAsync(forbiddenContext);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual string? GetToken(HttpRequest request)
    {
        if (!string.IsNullOrWhiteSpace(Options.QueryStringName) && request.Query.TryGetValue(Options.QueryStringName, out var queryToken))
            return queryToken;

        if (!string.IsNullOrWhiteSpace(Options.HeaderName) && request.Headers.TryGetValue(Options.HeaderName, out var headerToken))
            return headerToken;

        if (!string.IsNullOrWhiteSpace(Options.AuthenticationSchemeName) && request.Headers.ContainsKey(HeaderNames.Authorization) && AuthenticationHeaderValue.TryParse(request.Headers[HeaderNames.Authorization], out var headerValue))
            return headerValue.Parameter;

        return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected virtual bool CanHandle(HttpRequest request)
    {
        return (!string.IsNullOrWhiteSpace(Options.HeaderName) && request.Headers.ContainsKey(Options.HeaderName))
          || (!string.IsNullOrWhiteSpace(Options.QueryStringName) && request.Query.ContainsKey(Options.QueryStringName))
          || (!string.IsNullOrWhiteSpace(Options.AuthenticationSchemeName) && AuthenticationHeaderValue.TryParse(request.Headers[HeaderNames.Authorization], out var headerValue) && headerValue.Scheme.Equals(Options.AuthenticationSchemeName, System.StringComparison.OrdinalIgnoreCase));
    }
}