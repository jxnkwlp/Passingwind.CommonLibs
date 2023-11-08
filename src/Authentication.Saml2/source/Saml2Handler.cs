using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.Saml2;

public class Saml2Handler : RemoteAuthenticationHandler<Saml2Options>, IAuthenticationSignOutHandler
{
    private const string RelayStateName = "State";
    private const string CorrelationProperty = ".xsrf";

    private Saml2Configuration? _configuration;

    protected new Saml2Events Events
    {
        get => (Saml2Events)base.Events;
        set => base.Events = value;
    }

    public Saml2Handler(IOptionsMonitor<Saml2Options> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public override async Task<bool> HandleRequestAsync()
    {
        if (Options.RemoteSignOutPath.HasValue && Options.RemoteSignOutPath == Request.Path)
        {
            await HandleSignOutAsync(new AuthenticationProperties());

            return true;
        }

        return await base.HandleRequestAsync();
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        properties ??= new AuthenticationProperties();

        _configuration ??= await Options.ConfigurationManager.GetConfigurationAsync(Context.RequestAborted);

        // Save the original challenge URI so we can redirect back to it when we're done.
        if (string.IsNullOrEmpty(properties.RedirectUri))
        {
            properties.RedirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
        }

        GenerateCorrelationId(properties);

        Saml2AuthnRequest saml2AuthnRequest = new(_configuration)
        {
            ForceAuthn = Options.ForceAuthn,
        };

        Dictionary<string, string> relayStateQuery = new();

        if (!string.IsNullOrEmpty(properties.RedirectUri))
        {
            relayStateQuery[Options.ReturnUrlParameter] = properties.RedirectUri;
        }

        relayStateQuery[RelayStateName] = Options.StateDataFormat.Protect(properties);

        Saml2RedirectBinding binding = new();

        binding.SetRelayStateQuery(relayStateQuery);

        binding = binding.Bind(saml2AuthnRequest);

        RedirectContext redirectContext = new(Context, Scheme, Options, properties)
        {
            Saml2AuthnRequest = saml2AuthnRequest,
            RedirectBinding = binding,
        };

        await Events.RedirectToIdentityProvider(redirectContext);

        if (redirectContext.Handled)
        {
            return;
        }

        binding = redirectContext.RedirectBinding;

        Response.Redirect(binding.RedirectLocation.OriginalString);
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        var target = ResolveTarget(Options.ForwardSignOut);
        return (target != null)
            ? Context.SignOutAsync(target, properties)
            : HandleSignOutAsync(properties ?? new AuthenticationProperties());
    }

    protected virtual async Task HandleSignOutAsync(AuthenticationProperties? properties)
    {
        _configuration ??= await Options.ConfigurationManager.GetConfigurationAsync(Context.RequestAborted);

        Saml2StatusCodes status;
        var requestBinding = new Saml2PostBinding();
        var logoutRequest = new Saml2LogoutRequest(_configuration, Context.User);

        try
        {
            requestBinding.Unbind(Request.ToGenericHttpRequest(), logoutRequest);
            status = Saml2StatusCodes.Success;

            await Context.SignOutAsync(Options.SignOutScheme);
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Saml2 single logout error");
            status = Saml2StatusCodes.RequestDenied;
        }

        var responseBinding = new Saml2PostBinding
        {
            RelayState = requestBinding.RelayState
        };

        var saml2LogoutResponse = new Saml2LogoutResponse(_configuration)
        {
            InResponseToAsString = logoutRequest.IdAsString,
            Status = status,
        };
        responseBinding = responseBinding.Bind(saml2LogoutResponse);

        Response.Headers.CacheControl = "no-cache, no-store";
        Response.ContentType = "text/html";
        await Response.WriteAsync(responseBinding.PostContent);
    }

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        _configuration ??= await Options.ConfigurationManager.GetConfigurationAsync(Context.RequestAborted);

        Saml2AuthnResponse saml2AuthnResponse = new(_configuration);

        AuthenticationProperties? properties = null;

        try
        {
            Dictionary<string, string> relayStateQuery;
            if (Request.Method == HttpMethods.Get)
            {
                Saml2RedirectBinding binding = new();

                var saml2Request = Request.ToGenericHttpRequest();

                binding.ReadSamlResponse(saml2Request, saml2AuthnResponse);

                relayStateQuery = binding.GetRelayStateQuery();
            }
            else if (Request.Method == HttpMethods.Post)
            {
                Saml2PostBinding binding = new();

                var saml2Request = Request.ToGenericHttpRequest();

                binding.ReadSamlResponse(saml2Request, saml2AuthnResponse);

                relayStateQuery = binding.GetRelayStateQuery();
            }
            else
            {
                throw new AuthenticationException($"Saml2 response method '{Request.Method}' not support");
            }

            if (!relayStateQuery.ContainsKey(RelayStateName))
            {
                throw new AuthenticationException("Saml2 response missing relay state ");
            }

            string state = relayStateQuery[RelayStateName];

            properties = Options.StateDataFormat.Unprotect(state);

            MessageReceivedContext messageReceivedContext = new(Context, Scheme, Options, properties)
            {
                Saml2AuthnResponse = saml2AuthnResponse
            };
            await Events.MessageReceived(messageReceivedContext);
            if (messageReceivedContext.Result != null)
            {
                return messageReceivedContext.Result;
            }

            saml2AuthnResponse = messageReceivedContext.Saml2AuthnResponse;
            properties = messageReceivedContext.Properties!; // Provides a new instance if not set.

            // If state did flow from the challenge then validate it. See AllowUnsolicitedLogins above.
            if (properties.Items.TryGetValue(CorrelationProperty, out string? correlationId)
                && !ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.", properties);
            }

            if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                return HandleRequestResult.Fail($"Saml2 response status: {saml2AuthnResponse.Status}", properties);
            }

            ClaimsPrincipal? principal = new(saml2AuthnResponse.ClaimsIdentity);

            SecurityTokenReceivedContext securityTokenReceivedContext = new(Context, Scheme, Options, properties)
            {
                Saml2AuthnResponse = saml2AuthnResponse
            };
            await Events.SecurityTokenReceived(securityTokenReceivedContext);
            if (securityTokenReceivedContext.Result != null)
            {
                return securityTokenReceivedContext.Result;
            }

            SecurityTokenValidatedContext securityTokenValidatedContext = new(Context, Scheme, Options, principal, properties)
            {
                Saml2AuthnResponse = saml2AuthnResponse,
            };

            await Events.SecurityTokenValidated(securityTokenValidatedContext);
            if (securityTokenValidatedContext.Result != null)
            {
                return securityTokenValidatedContext.Result;
            }

            // Flow possible changes
            principal = securityTokenValidatedContext.Principal!;
            properties = securityTokenValidatedContext.Properties;

            return HandleRequestResult.Success(new AuthenticationTicket(principal, properties, Scheme.Name));
        }
        catch (System.Exception ex)
        {
            Logger.LogError(ex, "Exception occurred while processing message");

            AuthenticationFailedContext authenticationFailedContext = new(Context, Scheme, Options)
            {
                Saml2AuthnResponse = saml2AuthnResponse,
                Exception = ex
            };

            await Events.AuthenticationFailed(authenticationFailedContext);

            return authenticationFailedContext.Result ?? HandleRequestResult.Fail(ex, properties);
        }
    }
}
