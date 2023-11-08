using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.Saml2;

/// <summary>
/// Extension methods to configure Saml2 authentication.
/// </summary>
public static class Saml2Extensions
{
    /// <summary>
    /// Enables Saml2 authentication using the default scheme
    /// </summary>
    /// <param name="builder"></param>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder)
    {
        return AddSaml2(builder, Saml2Defaults.AuthenticationScheme, configureOptions: (_) => { });
    }

    /// <summary>
    /// Enables Saml2 authentication
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme"></param>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, string authenticationScheme)
    {
        return AddSaml2(builder, authenticationScheme, configureOptions: (_) => { });
    }

    /// <summary>
    /// Enables Saml2 authentication
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, Action<Saml2Options> configureOptions)
    {
        return AddSaml2(builder, Saml2Defaults.AuthenticationScheme, configureOptions: configureOptions);
    }

    /// <summary>
    /// Enables Saml2 authentication
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme"></param>
    /// <param name="configureOptions"></param>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, string authenticationScheme, Action<Saml2Options> configureOptions)
    {
        return AddSaml2(builder, authenticationScheme, configureOptions: configureOptions);
    }

    /// <summary>
    /// Enables Saml2 authentication
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme"></param>
    /// <param name="displayName"></param>
    /// <param name="configureOptions"></param>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<Saml2Options> configureOptions)
    {
        // builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<Saml2Options>, Saml2OptionsConfigureOptions>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<Saml2Options>, Saml2PostConfigureOptions>());

        builder.AddRemoteScheme<Saml2Options, Saml2Handler>(authenticationScheme, displayName, configureOptions);
        builder.Services.AddTransient<Saml2Handler>();

        return builder;
    }
}
