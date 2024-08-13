using System.Text.Json.Serialization;

namespace Mtd.Kiosk.LedUpdater.Realtime.Entitites;

public class Departure
{
	[JsonPropertyName("route")]
	public required string Route { get; set; }
	[JsonPropertyName("time")]
	public required string Time { get; set; }
}
