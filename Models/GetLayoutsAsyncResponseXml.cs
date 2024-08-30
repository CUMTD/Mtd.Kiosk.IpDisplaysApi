using System.Xml.Serialization;
namespace Mtd.Kiosk.IpDisplaysApi.Models;

[XmlRoot(ElementName = "Layouts")]
public class GetLayoutsAsyncResponseXml
{

	[XmlElement(ElementName = "Layout")]
	public required List<Layout> Layout { get; set; }
}

[XmlRoot(ElementName = "Layout")]
public class Layout
{

	[XmlAttribute(AttributeName = "recID")]
	public string? RecID { get; set; }

	[XmlAttribute(AttributeName = "parentID")]
	public string? ParentID { get; set; }

	[XmlAttribute(AttributeName = "targetID")]
	public string? TargetID { get; set; }

	[XmlAttribute(AttributeName = "order")]
	public string? Order { get; set; }

	[XmlAttribute(AttributeName = "name")]
	public string? Name { get; set; }

	[XmlAttribute(AttributeName = "defcolor")]
	public string? Defcolor { get; set; }

	[XmlAttribute(AttributeName = "backcolor")]
	public string? Backcolor { get; set; }

	[XmlAttribute(AttributeName = "style")]
	public string? Style { get; set; }

	[XmlAttribute(AttributeName = "duration")]
	public string? Duration { get; set; }

	[XmlAttribute(AttributeName = "top")]
	public string? Top { get; set; }

	[XmlAttribute(AttributeName = "bottom")]
	public string? Bottom { get; set; }

	[XmlAttribute(AttributeName = "left")]
	public string? Left { get; set; }

	[XmlAttribute(AttributeName = "right")]
	public string? Right { get; set; }

	[XmlAttribute(AttributeName = "enabled")]
	public string? Enabled { get; set; }

	[XmlAttribute(AttributeName = "backpict")]
	[XmlIgnore]
	public object? Backpict { get; set; }

	[XmlAttribute(AttributeName = "bkthresh")]
	[XmlIgnore]
	public object? Bkthresh { get; set; }

	[XmlAttribute(AttributeName = "dfthresh")]
	[XmlIgnore]
	public object? Dfthresh { get; set; }
}

