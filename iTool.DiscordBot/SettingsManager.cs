using System;
using System.IO;
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

            return new Deserializer().Deserialize<Settings>(File.ReadAllText(Common.SettingsFile));
        }

        public static void SaveSettings(Settings settings) =>
            File.WriteAllText(Common.SettingsFile, new Serializer().Serialize(settings));

        public static void ResetSettings()
        {
            if (!Directory.Exists(Common.SettingsDir))
            {
                Directory.CreateDirectory(Common.SettingsDir);
            }
            if (File.Exists(Common.SettingsFile))
            {
                File.Delete(Common.SettingsFile);
                Console.WriteLine("Settings reset.");
            }
            File.WriteAllText(Common.SettingsFile, 
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(new Settings()));
        }
    }
}
