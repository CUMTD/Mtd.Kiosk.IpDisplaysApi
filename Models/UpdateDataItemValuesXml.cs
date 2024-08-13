using System.Xml.Serialization;

namespace Mtd.Kiosk.LedUpdater.IpDisplaysApi.Models;

/*
 <DataItems>
<DataItem name="name1">42</DataItem>
<DataItem name="name2">21</DataItem>
</DataItems>
 */

[XmlRoot("DataItems")]
public class UpdateDataItemValuesXml
{
	[XmlElement("DataItem")]
	public List<DataItem> DataItems { get; set; }

	public UpdateDataItemValuesXml()
	{
		DataItems = new List<DataItem>();
	}

	public UpdateDataItemValuesXml(List<DataItem> dataItems)
	{
		DataItems = dataItems;
	}
}

public class DataItem
{
	[XmlAttribute("name")]
	public required string Name { get; set; }
	[XmlText]
	public required string Value { get; set; }
}
