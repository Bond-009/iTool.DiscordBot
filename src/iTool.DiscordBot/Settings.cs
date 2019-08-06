using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenWeather;
using Nett;

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
        public int MessageCacheSize { get; set; }
        public string Game { get; set; } = string.Empty;
        public string Prefix { get; set; } = "!";
        public Colors Color { get; set; } = Colors.DodgerBlue;
        public Colors ErrorColor { get; set; } = Colors.GuardsmanRed;
        public bool CaseSensitiveCommands { get; set; }
        public RunMode DefaultRunMode { get; set; } = RunMode.Sync;
        public bool GuildSpecificSettings { get; set; } = true;
        public Unit Units { get; set; } = Unit.Metric;

        [TomlIgnore]
        public List<ulong> BlacklistedUsers { get; set; }
        [TomlIgnore]
        public List<ulong> TrustedUsers { get; set; }

        public Color GetColor() => new Color((uint)Color);
        public Color GetErrorColor() => new Color((uint)ErrorColor);

        public async Task SaveAsync()
        {
            if (BlacklistedUsers != null && BlacklistedUsers.Any())
            {
                await SaveListToFile(Common.BlackListFile, BlacklistedUsers).ConfigureAwait(false);
            }

            if (TrustedUsers != null && TrustedUsers.Any())
            {
                await SaveListToFile(Common.TrustedListFile, TrustedUsers).ConfigureAwait(false);
            }

            Toml.WriteFile(this, Common.SettingsFile);
        }

        private async Task SaveListToFile(string path, IEnumerable<ulong> values)
        {
            using (StreamWriter t = new StreamWriter(path, false))
            {
                foreach (var value in path)
                {
                    t.Write(value);
                    await t.WriteAsync('\n').ConfigureAwait(false);
                }
            }
        }

        public static async Task<Settings> LoadAsync()
        {
            Directory.CreateDirectory(Common.SettingsDir);

            Settings settings = File.Exists(Common.SettingsFile) ? Toml.ReadFile<Settings>(Common.SettingsFile) : new Settings();
            settings.BlacklistedUsers = await LoadListFromFile(Common.BlackListFile).ConfigureAwait(false);
            settings.TrustedUsers = await LoadListFromFile(Common.TrustedListFile).ConfigureAwait(false);
            return settings;
        }

        public static async Task<List<ulong>> LoadListFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return new List<ulong>();
            }

            var lines = await File.ReadAllLinesAsync(path).ConfigureAwait(false);
            var values = new List<ulong>(lines.Length);
            foreach (var line in lines)
            {
                if (ulong.TryParse(line, out var value))
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }
}
