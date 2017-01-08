using System;
using System.IO;
using System.Xml;

namespace iTool.DiscordBot.Prefrences
{
    public static class Settings
    {
        #region Settings

        public static class General
        {
            public static string Game { get; set; }
            public static bool AntiSwear { get; set; }
        }

        public static class ApiKeys
        {
            public static string SteamKey { get; set; }
            public static string OpenWeatherMapKey { get; set; }
            public static string DiscordToken { get; set; }
        }

        public static class Static
        {
            public static readonly string SettingsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "settings";
            public static readonly string SettingsFile = SettingsDir + Path.DirectorySeparatorChar + "settings.xml";
        }

        #endregion
    }
}
