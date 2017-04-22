using System;
using System.IO;

namespace iTool.DiscordBot
{
    public static class Common
    {
        public static readonly string SettingsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "settings";
        public static readonly string SettingsFile = SettingsDir + Path.DirectorySeparatorChar + "settings.yaml";
        public static readonly string BlackListFile = Common.SettingsDir + Path.DirectorySeparatorChar + "blacklisted_users.txt";
        public static readonly string TrustedListFile = Common.SettingsDir + Path.DirectorySeparatorChar + "trusted_users.txt";

        public static readonly string DataDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "data";
        public static readonly string AudioDir = DataDir + Path.DirectorySeparatorChar + "audio";
        public static readonly string AudioIndexFile = AudioDir + Path.DirectorySeparatorChar + "audioindex.yaml";
        public static readonly string GuildsDir = DataDir + Path.DirectorySeparatorChar + "guilds";

        public static readonly string LogsDir = AppContext.BaseDirectory + Path.DirectorySeparatorChar + "logs";
    }
}
