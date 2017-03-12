using Discord;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace iTool.DiscordBot
{
    public static class Utils
    {
        public static bool IsNullOrEmpty<T>(this List<T> list) { return list == null || !list.Any(); }
        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> list) { return list == null || !list.Any(); }
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) { return enumerable == null || !enumerable.Any(); }
        public static TimeSpan GetUptime() => DateTime.Now - Process.GetCurrentProcess().StartTime;
        public static double GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2);
        public static bool IsTrustedUser(this IUser user) { return Program.TrustedUsers.Contains(user.Id) || user.Id == Program.Owner.Id; }
        public static List<string> LoadListFromFile(string path)
        {
            if (File.Exists(Common.SettingsDir + Path.DirectorySeparatorChar + "backlisted_users.txt"))
            {
                return File.ReadAllText(path)
                        .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct().ToList();
            }
            return new List<string>();
        }
    }
}
