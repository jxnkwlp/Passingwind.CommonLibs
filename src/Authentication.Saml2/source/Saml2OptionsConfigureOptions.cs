using System;
using Microsoft.Extensions.Options;

namespace Passingwind.AspNetCore.Authentication.Saml2;

public class Saml2OptionsConfigureOptions : IConfigureNamedOptions<Saml2Options>
{
    public void Configure(string name, Saml2Options options)
    {
        throw new NotImplementedException();
    }

    public void Configure(Saml2Options options)
    {
        throw new NotImplementedException();
    }
}