using System.Xml.Serialization;

[XmlRootAttribute("achievement")]
public class Achievement
{
    [XmlElementAttribute("name")]
    public string Name { get; set; }

    [XmlElementAttribute("achieved")]
    public int Achieved { get; set; }
}
