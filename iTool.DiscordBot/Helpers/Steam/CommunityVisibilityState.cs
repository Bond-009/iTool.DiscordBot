using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public enum CommunityVisibilityState
    {
        [XmlEnum("1")]
        Private = 1,
        [XmlEnum("3")]
        Public = 3
    }
}
