using System.Xml.Serialization;

[XmlRootAttribute("stat")]
public class Stat
{
    [XmlElementAttribute("name")]
    public string Name { get; set; }

    [XmlElementAttribute("value")]
    public int Value { get; set; }
}
