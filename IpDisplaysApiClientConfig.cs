using System.ComponentModel.DataAnnotations;

namespace Mtd.Kiosk.IpDisplaysApi;
public class IpDisplaysApiClientConfig
{
	public const string CONFIG_SECTION_NAME = "IPDisplaysApiClient";

	[Required, Range(1, int.MaxValue)]
	public required int TimeoutMiliseconds { get; set; }
}
