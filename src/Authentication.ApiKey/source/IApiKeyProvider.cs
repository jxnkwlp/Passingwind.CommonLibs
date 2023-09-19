using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.AspNetCore.Authentication.ApiKey;

/// <summary>
/// 
/// </summary>
public interface IApiKeyProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiKeyValidationResult> ValidateAsync(string apiKey, CancellationToken cancellationToken = default);
}
