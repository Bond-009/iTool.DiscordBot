using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class PlayerSummary : Player
    {
        public ulong SteamID { get; set; }
        [JsonProperty("communityvisibilitystate")]
        [JsonRequired]
        public CommunityVisibilityState CommunityVisibilityState { get; set; }
        [JsonProperty("profilestate")]
        [JsonRequired]
        public int ProfileState { get; set; }
        [JsonProperty("personaname")]
        [JsonRequired]
        public string PersonaName { get; set; }
        //[JsonProperty("lastlogoff")]
        //[JsonRequired]
        //public DateTime LastLogOff { get; set; }
        [JsonProperty("profileurl")]
        [JsonRequired]
        public string ProfileURL { get; set; }
        [JsonProperty("avatar")]
        [JsonRequired]
        public string Avatar { get; set; }
        [JsonProperty("avatarmedium")]
        [JsonRequired]
        public string AvatarMedium { get; set; }
        [JsonProperty("avatarfull")]
        [JsonRequired]
        public string AvatarFull { get; set; }
        [JsonProperty("personastate")]
        [JsonRequired]
        public PersonaState PersonaState { get; set; }
        [JsonProperty("primaryclanid")]
        [JsonRequired]
        public ulong PrimaryClanID { get; set; }
        //[JsonProperty("timecreated")]
        //[JsonRequired]
        //public DateTime TimeCreated { get; set; }
        [JsonProperty("personastateflags")]
        [JsonRequired]
        public int PersonaStateFlags { get; set; }
        [JsonProperty("loccountrycode")]
        [JsonRequired]
        public string LocCountryCode { get; set; }
    }
}
