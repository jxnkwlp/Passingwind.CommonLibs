using System.Threading;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;

namespace Passingwind.AspNetCore.Authentication.Saml2.Configuration;

/// <summary>
/// 
/// </summary>
public class StaticConfigurationManager : IConfigurationManager
{
    private readonly Saml2Configuration _saml2Configuration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="saml2Configuration"></param>
    public StaticConfigurationManager(Saml2Configuration saml2Configuration)
    {
        _saml2Configuration = saml2Configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Saml2Configuration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_saml2Configuration);
    }
}
