using System;
using System.IO;

namespace iTool.DiscordBot
{
    public static class Common
    {
        public static readonly string SettingsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "settings";
        public static readonly string SettingsFile = SettingsDir + Path.DirectorySeparatorChar + "settings.toml";
        public static readonly string ModuleFile = SettingsDir + Path.DirectorySeparatorChar + "modules.toml";
        public static readonly string BlackListFile = SettingsDir + Path.DirectorySeparatorChar + "blacklisted_users.txt";
        public static readonly string TrustedListFile = SettingsDir + Path.DirectorySeparatorChar + "trusted_users.txt";

        public static readonly string DataDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "data";
        public static readonly string AudioDir = DataDir + Path.DirectorySeparatorChar + "audio";
        public static readonly string AudioIndexFile = AudioDir + Path.DirectorySeparatorChar + "audioindex.toml";
    }
}
