using IpDisplaysSoapService;
using System.ServiceModel;
using System.Xml;

namespace Mtd.Led.Soap
{
	public class IpDisplaysSoapConfig
	{
		private readonly Uri _uri;
		private readonly TimeSpan _timeout;

		public IpDisplaysSoapConfig(string ip, TimeSpan timeout)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(ip);

			_uri = new Uri($"http://{ip}/soap1.wsdl");
			_timeout = timeout;
		}
		public SignSvrSoapPortClient GetSoapClient()
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
	}
}
