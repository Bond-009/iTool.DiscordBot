using System;
using System.IO;
using System.Xml.Serialization;

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
            using (FileStream fs = new FileStream(Common.SettingsFile, FileMode.Open))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                return (Settings)ser.Deserialize(fs);
            }
        }

        public static void SaveSettings(Settings settings)
        {
            using (FileStream fs = new FileStream(Common.SettingsFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(fs, settings);
            }
        }

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
            using (FileStream fs = new FileStream(Common.SettingsFile, FileMode.OpenOrCreate))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Settings));
                ser.Serialize(fs, new Settings());
            }
        }
    }
}
