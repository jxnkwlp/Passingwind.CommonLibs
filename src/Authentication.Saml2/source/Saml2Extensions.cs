using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.Saml2;

/// <summary>
/// 
/// </summary>
public static class Saml2Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, Action<Saml2Options>? configureOptions = null)
    {
        return builder.AddSaml2(Saml2Defaults.AuthenticationScheme, Saml2Defaults.AuthenticationScheme, configureOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="scheme"></param>
    /// <param name="displayName"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSaml2(this AuthenticationBuilder builder, string scheme, string? displayName = null, Action<Saml2Options>? configureOptions = null)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<Saml2Options>, Saml2PostConfigureOptions>());

        builder.AddScheme<Saml2Options, Saml2Handler>(scheme, displayName, configureOptions);

        builder.Services.AddTransient<Saml2Handler>();

        return builder;
    }
}
