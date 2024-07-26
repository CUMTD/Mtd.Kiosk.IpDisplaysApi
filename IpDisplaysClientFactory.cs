namespace Mtd.Led.Soap
{
	public class IpDisplaysClientFactory
	{
		public IpDisplaysSoapConfig CreateClient(string ipAddress, TimeSpan timeout)
		{
			return new IpDisplaysSoapConfig(ipAddress, timeout);
		}
	}
}
