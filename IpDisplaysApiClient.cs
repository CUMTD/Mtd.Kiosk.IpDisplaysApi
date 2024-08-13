using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.Kiosk.LedUpdater.IpDisplaysApi.Models;

namespace Mtd.Kiosk.LedUpdater.IpDisplaysApi;

// TODO: Use NuGet package instead of adding service reference

public class IPDisplaysApiClient
{

	private readonly Uri _uri;
	private readonly TimeSpan _timeout;
	private readonly ILogger<IPDisplaysApiClient> _logger;

	private const int START_TIMER = 96;
	private const int STOP_TIMER = 94;
	private const int PAUSE_TIMER = 95;

	#region Constructors

	internal IPDisplaysApiClient(string ip, IOptions<IpDisplaysApiClientConfig> config, ILogger<IPDisplaysApiClient> logger)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(ip);
		ArgumentNullException.ThrowIfNull(config, nameof(config));
		ArgumentNullException.ThrowIfNull(logger, nameof(logger));

		_uri = new Uri($"http://{ip}/soap1.wsdl");
		_timeout = TimeSpan.FromMilliseconds(config.Value.TimeoutMiliseconds);
		_logger = logger;
	}

	#endregion Constructors

	#region Helpers

	private SignSvrSoapPortClient GetSoapClient()
	{
		var binding = new BasicHttpBinding
		{
			MaxBufferSize = int.MaxValue,
			ReaderQuotas = XmlDictionaryReaderQuotas.Max,
			MaxReceivedMessageSize = int.MaxValue,
			AllowCookies = true,
			CloseTimeout = _timeout,
			OpenTimeout = _timeout,
			ReceiveTimeout = _timeout,
			SendTimeout = _timeout
		};
		var endpointAddress = new EndpointAddress(_uri);
		return new SignSvrSoapPortClient(binding, endpointAddress);
	}

	/// <summary>
	/// Serializes a dictionary of data items into an XML string compatible with the ipDisplays API
	/// </summary>
	/// <param name="dataItems">A dictionary of dataItem names and their new values</param>
	/// <returns></returns>
	private string SerializeUpdateDataItemsXmlString(Dictionary<string, string> dataItems)
	{
		var XMLdataItems = new UpdateDataItemValuesXml();

		foreach (var item in dataItems)
		{
			XMLdataItems.DataItems.Add(new DataItem { Name = item.Key, Value = item.Value });
		}

		var serializer = new XmlSerializer(typeof(UpdateDataItemValuesXml));

		try
		{
			using var textWriter = new StringWriter();
			serializer.Serialize(textWriter, XMLdataItems);
			var xml = textWriter.ToString();
			_logger.LogTrace("Serialized {xml} data items for sign.", xml);
			return xml;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to serialize data items for sign update.");
			throw;
		}
	}

	/// <summary>
	/// Deserializes a GetLayoutByNameResponse into a GetLayoutByNameResponseXml object.
	/// </summary>
	/// <param name="response"></param>
	/// <returns></returns>
	private GetLayoutByNameResponseXml? DeserializeGetLayoutByNameResponse(GetLayoutByNameResponse response)
	{
		var serializer = new XmlSerializer(typeof(GetLayoutByNameResponseXml));

		try
		{
			using var reader = new StringReader(response.layoutInfoXml);

			if (serializer.Deserialize(reader) is GetLayoutByNameResponseXml layout)
			{
				return layout;
			}

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to deserialize GetLayoutByName response xml.");
		}

		return null;

	}

	/// <summary>
	/// Enables a single layout and disables all other layouts.
	/// </summary>
	/// <param name="client"></param>
	/// <param name="layoutToEnable">The name of the layout to enable.</param>
	/// <returns></returns>
	private async Task<bool> SetSingleLayoutAsync(SignSvrSoapPortClient client, string layoutToEnable)
	{
		// get all layouts on the sign
		var layouts = await client.GetLayoutsAsync(new GetLayoutsRequest(0));

		// deserialize into a GetLayoutsAsyncResponseXml object
		var serializer = new XmlSerializer(typeof(GetLayoutsAsyncResponseXml));
		using var reader = new StringReader(layouts.layoutInfoXml);
		if (serializer.Deserialize(reader) is not GetLayoutsAsyncResponseXml response)
		{
			_logger.LogError("Failed to deserialize GetLayoutsAsync response.");
			return false;
		}

		// disable all enabled layouts and enable the target layout
		foreach (var layout in response.Layout)
		{
			if (layout.Enabled == "1")
			{
				_ = await client.SetLayoutStateAsync(layout.Name, 0);
				_logger.LogTrace("Disabled {layoutName} layout on sign.", layout.Name);
			}
		}

		// enable the target layout

		var result = await client.SetLayoutStateAsync(layoutToEnable, 1);
		_logger.LogTrace("Enabled {layoutToEnable} layout on sign.", layoutToEnable);

		return true;
	}

	#endregion Helpers

	#region Api Methods

	/// <summary>
	/// Refreshes the "Time_Since_Last_Update" data item on the sign. Failing to refresh this data item will cause the sign to time out and go blank.
	/// </summary>
	/// <returns></returns>
	public async Task<bool> RefreshTimer()
	{
		using var client = GetSoapClient();

		// must be done in this order
		var stop = await client.SendCommandAsync(STOP_TIMER, "Time_Since_Last_Update");
		var set = await client.UpdateDataItemValueByNameAsync("Time_Since_Last_Update", DateTime.Now.ToString("M/d HH:mm:ss"));
		var start = await client.SendCommandAsync(START_TIMER, "Time_Since_Last_Update");

		var success = stop.Result == 1 && set.Result == 1 && start.Result == 1;

		if (success)
		{
			_logger.LogTrace("Refreshed Timer on Sign");
		}
		else
		{
			_logger.LogError("Failed to Refresh Timer on Sign");
		}

		return success;
	}

	/// <summary>
	/// Ensures that a layout is enabled. If the layout is not enabled, it will be enabled. This approach prevents unnecessary setLayout calls, which can cause flickering on the sign.
	/// </summary>
	/// <param name="layoutName"></param>
	/// <returns></returns>
	public async Task<bool> EnsureLayoutEnabled(string layoutName)
	{
		using var client = GetSoapClient();

		var layout = await client.GetLayoutByNameAsync(new GetLayoutByNameRequest(layoutName, 0));

		if (layout.layoutInfoXml.Contains("enabled=\"1\""))
		{
			// layout is already enabled, no need to do anything
			_logger.LogTrace("{layoutName} is already enabled.", layoutName);
			return true;
		}
		else
		{
			var result = await SetSingleLayoutAsync(client, layoutName);
		}

		return true;

	}

	/// <summary>
	/// Updates a single data item on the sign.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public async Task<bool> UpdateDataItem(string name, string value)
	{
		using var client = GetSoapClient();
		try
		{
			_ = await client.UpdateDataItemValueByNameAsync(name, value);
			_logger.LogTrace("Updated {name} to {value}", name, value);
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "{name} Failed to Execute", nameof(UpdateDataItem));
		}
		return false;
	}

	/// <summary>
	/// Updates multiple data items on the sign at once.
	/// </summary>
	/// <param name="dataItems">A dictionary of dataItem names mapped to their new values.</param>
	/// <returns></returns>
	public async Task<bool> UpdateDataItems(Dictionary<string, string> dataItems)
	{
		using var client = GetSoapClient();

		var xml = SerializeUpdateDataItemsXmlString(dataItems);

		_ = await client.UpdateDataItemValuesAsync(xml);

		return true;
	}
	#endregion Api Methods

}
