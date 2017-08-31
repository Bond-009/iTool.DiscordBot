using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using OpenWeather;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public class Settings
    {
        public string DiscordToken { get; set; } = string.Empty;
        public string OpenWeatherMapKey { get; set; } = string.Empty;
        public string SteamKey { get; set; } = string.Empty;
        public bool AlwaysDownloadUsers { get; set; }
        public int ConnectionTimeout { get; set; } = 30000;
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;
        public int MessageCacheSize { get; set; }
        public string Game { get; set; } = string.Empty;
        public string Prefix { get; set; } = "!";
        public Colors Color { get; set; } = Colors.DodgerBlue;
        public Colors ErrorColor { get; set; } = Colors.GuardsmanRed;
        public bool CaseSensitiveCommands { get; set; }
        public RunMode DefaultRunMode { get; set; } = RunMode.Sync;
        public bool AntiSwear { get; set; }
        public bool GuildSpecificSettings { get; set; } = true;
        public Unit Units { get; set; } = Unit.Metric;

        [YamlIgnore]
        public List<ulong> BlacklistedUsers { get; set; }
        [YamlIgnore]
        public List<ulong> TrustedUsers { get; set; }

        public Color GetColor() => new Color((uint)Color);
        public Color GetErrorColor() => new Color((uint)ErrorColor);

        public void Save()
        {
            if (!BlacklistedUsers.IsNullOrEmpty())
            { File.WriteAllLines(Common.BlackListFile, BlacklistedUsers.Select(x => x.ToString())); }

            if (!TrustedUsers.IsNullOrEmpty())
            { File.WriteAllLines(Common.TrustedListFile, TrustedUsers.Select(x => x.ToString())); }

            File.WriteAllText(Common.SettingsFile, new SerializerBuilder().EmitDefaults().Build().Serialize(this));
        }

        public static Settings Load()
        {
            Directory.CreateDirectory(Common.SettingsDir);

            Settings settings = File.Exists(Common.SettingsFile) ? new Deserializer().Deserialize<Settings>(File.ReadAllText(Common.SettingsFile)) : new Settings();
            settings.BlacklistedUsers = Utils.LoadListFromFile(Common.BlackListFile)?.Select(ulong.Parse).ToList() ?? new List<ulong>();
            settings.TrustedUsers = Utils.LoadListFromFile(Common.TrustedListFile)?.Select(ulong.Parse).ToList() ?? new List<ulong>();
            return settings;
        }
    }
}
