using System.Xml.Serialization;

namespace Mtd.Kiosk.IpDisplaysApi.Models;

/*
 * 
<Layout recID="805306403" parentID="0" targetID="501" order="1" name="TwoLineMessage" defcolor="000000" backcolor="000000" style="0" duration="60" top="0" bottom="32" left="0" right="256" enabled="1" backpict="" bkthresh="" dfthresh=""/>

 */

[XmlRoot("Layout")]
public class GetLayoutByNameResponseXml
{
	[XmlAttribute("recID")]
	public string? RecID { get; set; }
	[XmlAttribute("parentID")]
	public string? ParentID { get; set; }
	[XmlAttribute("targetID")]
	public string? TargetID { get; set; }
	[XmlAttribute("order")]
	public string? Order { get; set; }
	[XmlAttribute("name")]
	public string? Name { get; set; }
	[XmlAttribute("defcolor")]
	public string? DefColor { get; set; }
	[XmlAttribute("backcolor")]
	public string? BackColor { get; set; }
	[XmlAttribute("style")]
	public string? Style { get; set; }
	[XmlAttribute("duration")]
	public string? Duration { get; set; }
	[XmlAttribute("top")]
	public string? Top { get; set; }
	[XmlAttribute("bottom")]
	public string? Bottom { get; set; }
	[XmlAttribute("left")]
	public string? Left { get; set; }
	[XmlAttribute("right")]
	public string? Right { get; set; }
	[XmlAttribute("enabled")]
	public string? Enabled { get; set; }
	[XmlAttribute("backpict")]
	public string? BackPict { get; set; }
	[XmlAttribute("bkthresh")]
	public string? BkThresh { get; set; }
	[XmlAttribute("dfthresh")]
	public string? DfThresh { get; set; }
}

