using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iTool.DiscordBot
{
    public static class Utils
    {
        public static bool IsNullOrEmpty<T>(this List<T> list) { return list == null || !list.Any(); }
        public static TimeSpan GetUptime() => DateTime.Now - Process.GetCurrentProcess().StartTime;
        public static double GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2);
    }
}
