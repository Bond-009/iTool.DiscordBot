using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    public class PlayerSummary : Player
    {
        [XmlElementAttribute("communityvisibilitystate")]
        public CommunityVisibilityState CommunityVisibilityState { get; set; }
        [XmlElement("profilestate")]
        public int ProfileState { get; set; }
        [XmlElement("personaname")]
        public string PersonaName { get; set; }
        //[XmlElement("lastlogoff")]
        //public DateTime LastLogOff { get; set; }
        [XmlElement("profileurl")]
        public string ProfileURL { get; set; }
        [XmlElement("avatar")]
        public string Avatar { get; set; }
        [XmlElement("avatarmedium")]
        public string AvatarMedium { get; set; }
        [XmlElement("avatarfull")]
        public string AvatarFull { get; set; }
        [XmlElement("personastate")]
        public PersonaState PersonaState { get; set; }
        [XmlElement("primaryclanid")]
        public ulong PrimaryClanID { get; set; }
        //[XmlElement("timecreated")]
        //public DateTime TimeCreated { get; set; }
        [XmlElement("personastateflags")]
        public int PersonaStateFlags { get; set; }
        [XmlElement("loccountrycode")]
        public string LocCountryCode { get; set; }
    }
}
