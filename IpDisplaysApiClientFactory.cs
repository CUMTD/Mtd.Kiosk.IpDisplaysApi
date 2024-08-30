using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mtd.Kiosk.IpDisplaysApi;

public class IpDisplaysApiClientFactory
{
	private readonly IOptions<IpDisplaysApiClientConfig> _config;
	private readonly ILogger<IPDisplaysApiClient> _logger;

	public IpDisplaysApiClientFactory(IOptions<IpDisplaysApiClientConfig> config, ILogger<IPDisplaysApiClient> logger)
	{
		ArgumentNullException.ThrowIfNull(config?.Value);
		ArgumentNullException.ThrowIfNull(logger);

		_config = config;
		_logger = logger;
	}

	public IPDisplaysApiClient CreateClient(string ipAddress) => new(ipAddress, null, _config, _logger);
	public IPDisplaysApiClient CreateClient(string ipAddress, string kioskId) => new(ipAddress, kioskId, _config, _logger);
}
