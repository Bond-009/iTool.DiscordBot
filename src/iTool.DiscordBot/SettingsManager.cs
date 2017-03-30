using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public static class SettingsManager
    {
        public static Settings LoadSettings()
        {
            if (!File.Exists(Common.SettingsFile))
            {
                ResetSettings();
            }

            Settings settings  = new Deserializer().Deserialize<Settings>(File.ReadAllText(Common.SettingsFile));
            settings.BlacklistedUsers.AddRange(Utils.LoadListFromFile(Common.BlackListFile).Select(ulong.Parse));
            settings.TrustedUsers.AddRange(Utils.LoadListFromFile(Common.TrustedListFile).Select(ulong.Parse));
            return settings;
        }

        public static void SaveSettings(Settings settings)
        {
            if (!settings.BlacklistedUsers.IsNullOrEmpty())
            { File.WriteAllLines(Common.BlackListFile, settings.BlacklistedUsers.Select(x => x.ToString())); }

            if (!settings.TrustedUsers.IsNullOrEmpty())
            { File.WriteAllLines(Common.TrustedListFile, settings.TrustedUsers.Select(x => x.ToString())); }

            File.WriteAllText(Common.SettingsFile, new SerializerBuilder().EmitDefaults().Build().Serialize(settings));
        }

        public static void ResetSettings()
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
