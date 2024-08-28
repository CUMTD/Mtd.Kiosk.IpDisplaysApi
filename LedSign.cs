using Microsoft.Extensions.Logging;
using Mtd.Kiosk.LedUpdater.Realtime.Entitites;
using System.ComponentModel.DataAnnotations;

namespace Mtd.Kiosk.LedUpdater.IpDisplaysApi;
public class LedSign
{
	private readonly IPDisplaysApiClient _client;
	private readonly ILogger _logger;

	public LedSign(IPDisplaysApiClient client, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(client);
		ArgumentNullException.ThrowIfNull(logger);

		_client = client;
		_logger = logger;
	}

	public Task<bool> UpdateSign(Departure departure)
	{
		return UpdateSign(departure, null);
	}

	public async Task<bool> UpdateSign(Departure topDeparture, Departure? bottomDeparture)
	{
		await _client.RefreshTimer();

		var dataItems = new Dictionary<string, string>
		{
			{ "Top_Left", topDeparture.Route },
			{ "Top_Right", topDeparture.Time },
			{ "Bottom_Left", bottomDeparture?.Route ?? string.Empty },
			{ "Bottom_Right", bottomDeparture?.Time ?? string.Empty }
		};

		await _client.UpdateDataItems(dataItems);

		var result = await _client.EnsureLayoutEnabled("TwoLineDepartures");

		_logger.LogInformation("Sign updated with TwoLineDepartures: {TopDeparture} and {BottomDeparture}", topDeparture, bottomDeparture);

		return result;
	}

	public async Task<bool> UpdateSign(string topMessage, Departure? bottomDeparture)
	{
		await _client.RefreshTimer();

		var dataItems = new Dictionary<string, string>
		{
			{ "Top_Center", topMessage },
			{ "Bottom_Left", bottomDeparture?.Route ?? string.Empty },
			{ "Bottom_Right", bottomDeparture?.Time ?? string.Empty }
		};

		await _client.UpdateDataItems(dataItems);

		var result = await _client.EnsureLayoutEnabled("OneLineMessage");

		_logger.LogInformation("Sign updated with OneLineMessage: {TopMessage} and departure: {BottomDeparture}", topMessage, bottomDeparture);

		return result;
	}

	public async Task<bool> UpdateSign(string topMessage, string bottomMessage)
	{
		await _client.RefreshTimer();

		var dataItems = new Dictionary<string, string>
		{
			{ "Top_Center", topMessage },
			{ "Bottom_Center", bottomMessage }
		};

		await _client.UpdateDataItems(dataItems);

		var result = await _client.EnsureLayoutEnabled("TwoLineMessage");

		_logger.LogInformation("Sign updated with TwoLineMessage: {TopMessage} and {BottomMessage}", topMessage, bottomMessage);

		return result;
	}

	public async Task<bool> BlankScreen()
	{
		var result = await UpdateSign(string.Empty, string.Empty);

		_logger.LogInformation("Sign blanked.");

		return result;
	}

	public async Task<bool> UpdateBrightness([Range(1, 127)] int newBrightness)
	{
		var result = await _client.UpdateSignBrightness(newBrightness);

		_logger.LogInformation("Sign brightness updated to {Brightness}", newBrightness);

		return result;
	}
}

