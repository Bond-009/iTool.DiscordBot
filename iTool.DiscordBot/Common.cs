using System;
using System.IO;

namespace iTool.DiscordBot
{
    public static class Common
    {
        public static readonly string SettingsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "settings";
        public static readonly string SettingsFile = SettingsDir + Path.DirectorySeparatorChar + "settings.yaml";
        public static readonly string AudioDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "audio";
        public static readonly string AudioIndexFile = AudioDir + Path.DirectorySeparatorChar + "audioindex.yaml";
    }
}
