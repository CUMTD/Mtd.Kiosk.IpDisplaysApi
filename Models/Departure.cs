using System.Text.Json.Serialization;

namespace Mtd.Kiosk.IpDisplaysApi.Models;

public class Departure
{
	[JsonPropertyName("route")]
	public required string Route { get; set; }
	[JsonPropertyName("time")]
	public required string Time { get; set; }

	public override string ToString() => $"{Route} in {Time}";
}
