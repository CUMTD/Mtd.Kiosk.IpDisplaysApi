using Microsoft.Extensions.Logging;
using Mtd.Kiosk.IpDisplaysApi.Models;
using Mtd.Kiosk.LedUpdater.IpDisplaysApi;
using System.ComponentModel.DataAnnotations;

namespace Mtd.Kiosk.IpDisplaysApi;
public class LedSign
{
	private readonly string _kioskId;
	private readonly string _kioskName;
	private readonly IPDisplaysApiClient _client;
	private readonly ILogger _logger;

	public LedSign(string kioskId, string kioskName, string stopId, IPDisplaysApiClient client, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(client);
		ArgumentNullException.ThrowIfNull(logger);

		_kioskId = kioskId;
		_kioskName = $"{kioskName} ({stopId})";

		_client = client;
		_logger = logger;
	}

	public Task<bool> UpdateSign(Departure departure) => UpdateSign(departure, null);

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

		_logger.LogDebug("{kioskId} updated with TwoLineDepartures: {TopDeparture} and {BottomDeparture}", _kioskId, topDeparture, bottomDeparture);

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

		_logger.LogDebug("{kioskId} updated with OneLineMessage: {TopMessage} and departure: {BottomDeparture}", _kioskId, topMessage, bottomDeparture);

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

		_logger.LogDebug("{kioskId} updated with TwoLineMessage: {TopMessage} and {BottomMessage}", _kioskId, topMessage, bottomMessage);

		return result;
	}

	public async Task<bool> BlankScreen()
	{
		var result = await UpdateSign(string.Empty, string.Empty);

		_logger.LogInformation("{kioskId} blanked.", _kioskId);

		return result;
	}

	public async Task<bool> UpdateBrightness([Range(1, 127)] int newBrightness)
	{
		var result = await _client.UpdateSignBrightness(newBrightness);

		_logger.LogInformation("{kioskId} brightness updated to {brightness}", _kioskId, newBrightness);

		return result;
	}
}

