using System;
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
    /// <inheritdoc />
    protected IApiKeyProvider ApiKeyProvider { get; }

    /// <inheritdoc />
    protected new ApiKeyEvents Events { get => (ApiKeyEvents)base.Events!; set => base.Events = value; }

#if NET8_0_OR_GREATER
    /// <inheritdoc />
    [Obsolete("ISystemClock is obsolete, use TimeProvider on AuthenticationSchemeOptions instead.")]
    public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiKeyProvider apiKeyProvider) : base(options, logger, encoder, clock)
    {
        ApiKeyProvider = apiKeyProvider;
    }
    /// <inheritdoc />
    public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, IApiKeyProvider apiKeyProvider) : base(options, logger, encoder)
    {
        ApiKeyProvider = apiKeyProvider;
    }
#else
    /// <inheritdoc />
    public ApiKeyHandler(
        IOptionsMonitor<ApiKeyOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyProvider apiKeyProvider) : base(options, logger, encoder, clock)
    {
        ApiKeyProvider = apiKeyProvider;
    }
#endif

    /// <inheritdoc />
    protected override Task<object> CreateEventsAsync()
    {
        return Task.FromResult<object>(new ApiKeyEvents());
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        ApiKeyMessageReceivedContext messageReceivedContext = new(Context, Scheme, Options);

        await Events.MessageReceivedAsync(messageReceivedContext).ConfigureAwait(false);

        if (messageReceivedContext.Result != null)
        {
            return messageReceivedContext.Result;
        }

        HttpRequest request = Request;

        bool isApiKeyAvailable = CanHandle(request);

        if (!isApiKeyAvailable)
        {
            return AuthenticateResult.NoResult();
        }

        string? token = GetToken(request);

        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.NoResult();
        }

        Logger.LogDebug("The Api Key '{token}' was found in the request.", token);

        try
        {
            ApiKeyValidationResult validationResult = await ApiKeyProvider.ValidateAsync(token).ConfigureAwait(false);

            if (validationResult.Exception != null)
            {
                Logger.LogError(validationResult.Exception, "The API Key verification failed by {Name}.", ApiKeyProvider.GetType().Name);

                ApiKeyAuthenticationFailedContext authenticationFailedContext = new(Context, Scheme, Options) { Exception = validationResult.Exception };

                await Events.AuthenticationFailedAsync(authenticationFailedContext).ConfigureAwait(false);

                return authenticationFailedContext.Result ?? AuthenticateResult.Fail(authenticationFailedContext.Exception);
            }

            Logger.LogInformation("The API Key verification successful by {Name}.", ApiKeyProvider.GetType().Name);

            ClaimsIdentity identity = validationResult.Identity;

            ApiKeyTokenValidatedContext tokenValidatedContext = new(Context, Scheme, Options)
            {
                Token = token,
                Principal = new ClaimsPrincipal(identity),
            };

            await Events.TokenValidatedAsync(tokenValidatedContext).ConfigureAwait(false);

            if (tokenValidatedContext.Result != null)
            {
                return tokenValidatedContext.Result;
            }

            if (Options.SaveToken)
            {
                tokenValidatedContext.Properties.StoreTokens(new[] { new AuthenticationToken() { Name = "token", Value = token } });
            }

            tokenValidatedContext.Success();

            return tokenValidatedContext.Result!;
        }
        catch (System.Exception exception)
        {
            Logger.LogError(exception, "Exception occurred while processing message.");

            ApiKeyAuthenticationFailedContext authenticationFailedContext = new(Context, Scheme, Options) { Exception = exception };

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
        AuthenticateResult authResult = await HandleAuthenticateOnceSafeAsync().ConfigureAwait(false);

        ApiKeyChallengeContext eventContext = new(Context, Scheme, Options, properties)
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

            StringBuilder builder = new(Options.Challenge);

            if (!string.IsNullOrWhiteSpace(Options.Realm))
            {
                builder.Append("realm=\"").Append(Options.Realm).Append('\"');
            }
            if (Options.Challenge.IndexOf(' ') > 0)
            {
                // Only add a comma after the first param, if any
                builder.Append(',');
            }
            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                builder.Append(" error=\"")
                    .Append(eventContext.Error)
                    .Append('\"');
            }

            // TODO

            Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
        }
    }

    /// <inheritdoc />
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        ApiKeyForbiddenContext forbiddenContext = new(Context, Scheme, Options);

        return Events.ForbiddenAsync(forbiddenContext);
    }

    /// <inheritdoc />
    protected virtual string? GetToken(HttpRequest request)
    {
        if (!string.IsNullOrWhiteSpace(Options.QueryStringName) && request.Query.TryGetValue(Options.QueryStringName, out Microsoft.Extensions.Primitives.StringValues queryToken))
        {
            return (string?)queryToken;
        }

        if (!string.IsNullOrWhiteSpace(Options.HeaderName) && request.Headers.TryGetValue(Options.HeaderName, out Microsoft.Extensions.Primitives.StringValues headerToken))
        {
            return (string?)headerToken;
        }

        if (!string.IsNullOrWhiteSpace(Options.HeaderAuthenticationSchemeName) && request.Headers.TryGetValue(HeaderNames.Authorization, out Microsoft.Extensions.Primitives.StringValues value) && AuthenticationHeaderValue.TryParse(value, out AuthenticationHeaderValue? headerValue))
        {
            return headerValue.Parameter;
        }

        return string.Empty;
    }

    /// <inheritdoc />
    protected virtual bool CanHandle(HttpRequest request)
    {
        return (!string.IsNullOrWhiteSpace(Options.HeaderName) && request.Headers.ContainsKey(Options.HeaderName))
          || (!string.IsNullOrWhiteSpace(Options.QueryStringName) && request.Query.ContainsKey(Options.QueryStringName))
          || (!string.IsNullOrWhiteSpace(Options.HeaderAuthenticationSchemeName) && AuthenticationHeaderValue.TryParse(request.Headers[HeaderNames.Authorization], out AuthenticationHeaderValue? headerValue) && headerValue.Scheme.Equals(Options.HeaderAuthenticationSchemeName, System.StringComparison.OrdinalIgnoreCase));
    }
}
