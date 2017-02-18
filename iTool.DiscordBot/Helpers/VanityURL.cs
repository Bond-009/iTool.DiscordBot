using System.Xml.Serialization;

[XmlRootAttribute("response")]
public class VanityURL
{
    [XmlElementAttribute("steamid", IsNullable=true)]
    public long? SteamID64 { get; set; }

    [XmlElementAttribute("success")]
    public int Succes { get; set; }
}
