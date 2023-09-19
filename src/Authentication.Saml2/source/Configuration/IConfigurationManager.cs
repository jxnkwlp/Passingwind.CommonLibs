using System.Threading;
using System.Threading.Tasks;
using ITfoxtec.Identity.Saml2;

namespace Passingwind.AspNetCore.Authentication.Saml2.Configuration;

/// <summary>
/// 
/// </summary>
public interface IConfigurationManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Saml2Configuration> GetConfigurationAsync(CancellationToken cancellationToken = default);
}
