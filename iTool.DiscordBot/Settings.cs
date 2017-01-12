using System;
using System.IO;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    public class Settings
    {
        public string Prefix { get; set; } = "!";
        public string Game { get; set; } = string.Empty;
        public bool AntiSwear { get; set; } 
        public string SteamKey { get; set; } = string.Empty;
        public string OpenWeatherMapKey { get; set; } = string.Empty;
        public string DiscordToken { get; set; } = string.Empty;

        public static class Static
        {
            [XmlIgnore]
            public static readonly string SettingsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "settings";
            [XmlIgnore]
            public static readonly string SettingsFile = SettingsDir + Path.DirectorySeparatorChar + "settings.xml";
        }
    }
}
