using Discord;
using Discord.Audio;
using OpenWeather;

namespace iTool.DiscordBot
{
    public class Settings
    {
        public string Prefix { get; set; } = "!";
        public AudioMode AudioMode { get; set; }
        public bool AlwaysDownloadUsers { get; set; }
        public int ConnectionTimeout { get; set; } = 30000;
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
        public int MessageCacheSize { get; set; }
        public string Game { get; set; } = string.Empty;
        public bool AntiSwear { get; set; }
        public string SteamKey { get; set; } = string.Empty;
        public string OpenWeatherMapKey { get; set; } = string.Empty;
        public string DiscordToken { get; set; } = string.Empty;
        public TemperatureScale TemperatureScale { get; set; }
    }
}
