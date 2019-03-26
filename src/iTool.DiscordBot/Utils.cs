using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Discord;

namespace iTool.DiscordBot
{
    public static class Utils
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
            => enumerable == null || !enumerable.Any();

        public static TimeSpan GetUptime() => DateTime.Now - Process.GetCurrentProcess().StartTime;

        public static double GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2);

        public static IEnumerable<string> LoadListFromFile(string path)
        {
            if (!File.Exists(path)) return null;

            return File.ReadAllText(path)
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct();
        }

        public static bool IsSubclassOfRawGeneric(this Type derivedType, Type baseType)
        {
            while (derivedType != null && derivedType != typeof(object))
            {
                Type currentType = derivedType.GetTypeInfo().IsGenericType ? derivedType.GetGenericTypeDefinition() : derivedType;
                if (baseType == currentType)
                {
                    return true;
                }

                derivedType = derivedType.GetTypeInfo().BaseType;
            }

            return false;
        }

        public static int LogLevelFromSeverity(LogSeverity severity)
            => (Math.Abs((int)severity - 5));
    }
}
