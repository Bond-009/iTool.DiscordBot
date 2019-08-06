using System;
using System.Diagnostics;
using System.Reflection;
using Discord;

namespace iTool.DiscordBot
{
    public static class Utils
    {
        public static TimeSpan GetUptime()
        {
            using (var pro = Process.GetCurrentProcess())
            {
                return DateTime.Now - pro.StartTime;
            }
        }

        public static double GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2);

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
            => Math.Abs((int)severity - 5);
    }
}
