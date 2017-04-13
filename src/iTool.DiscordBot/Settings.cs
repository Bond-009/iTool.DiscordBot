using Discord;
using Discord.Commands;
using OpenWeather;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public TemperatureScale TemperatureScale { get; set; }

        [YamlIgnore]
        public List<ulong> BlacklistedUsers { get; set; } = new List<ulong>();
        [YamlIgnore]
        public List<ulong> TrustedUsers { get; set; } = new List<ulong>();

        public static Settings Load()
        {
            if (!File.Exists(Common.SettingsFile))
            {
                Reset();
            }

            Settings settings  = new Deserializer().Deserialize<Settings>(File.ReadAllText(Common.SettingsFile));
            settings.BlacklistedUsers.AddRange(Utils.LoadListFromFile(Common.BlackListFile).Select(ulong.Parse));
            settings.TrustedUsers.AddRange(Utils.LoadListFromFile(Common.TrustedListFile).Select(ulong.Parse));
            return settings;
        }

        public static void Save(Settings settings)
        {
            if (!settings.BlacklistedUsers.IsNullOrEmpty())
            { File.WriteAllLines(Common.BlackListFile, settings.BlacklistedUsers.Select(x => x.ToString())); }

            if (!settings.TrustedUsers.IsNullOrEmpty())
            { File.WriteAllLines(Common.TrustedListFile, settings.TrustedUsers.Select(x => x.ToString())); }

            File.WriteAllText(Common.SettingsFile, new SerializerBuilder().EmitDefaults().Build().Serialize(settings));
        }

        public static void Reset()
        {
            Directory.CreateDirectory(Common.SettingsDir);

            if (File.Exists(Common.SettingsFile))
            { File.Delete(Common.SettingsFile); }

            if (File.Exists(Common.BlackListFile))
            { File.Delete(Common.BlackListFile); }

            if (File.Exists(Common.TrustedListFile))
            { File.Delete(Common.TrustedListFile); }

            Console.WriteLine("Settings reset.");

            File.WriteAllText(Common.SettingsFile, 
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(new Settings()));
        }
    }
}
