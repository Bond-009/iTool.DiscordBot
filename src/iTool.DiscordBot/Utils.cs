using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace iTool.DiscordBot
{
    public static class Utils
    {
        public static bool IsNullOrEmpty<T>(this List<T> list) => list == null || !list.Any();

        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> list) => list == null || !list.Any();

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || !enumerable.Any();

        public static TimeSpan GetUptime() => DateTime.Now - Process.GetCurrentProcess().StartTime;

        public static double GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2);

        public static IEnumerable<string> LoadListFromFile(string path)
        {
            if (!File.Exists(path)) return null;

            return File.ReadAllText(path)
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct();
        }
    }
}
