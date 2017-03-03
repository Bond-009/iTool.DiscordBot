using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public enum ProfileState
    {
        [XmlEnum("0")]
        Offline,
        [XmlEnum("1")]
        Online,
        [XmlEnum("2")]
        Busy,
        [XmlEnum("3")]
        Away,
        [XmlEnum("4")]
        Snooze,
        [XmlEnum("5")]
        LookingToTrade,
        [XmlEnum("6")]
        LookingToPlay
    }
}
