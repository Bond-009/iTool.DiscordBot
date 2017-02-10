﻿namespace iTool.DiscordBot
{
    public class Settings
    {
        public string Prefix { get; set; } = "!";
        public string Game { get; set; } = string.Empty;
        public bool AntiSwear { get; set; }
        public string SteamKey { get; set; } = string.Empty;
        public string OpenWeatherMapKey { get; set; } = string.Empty;
        public string DiscordToken { get; set; } = string.Empty;
    }
}
