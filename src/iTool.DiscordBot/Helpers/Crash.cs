using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace iTool.DiscordBot
{
    public class Crash
    {
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public Version Version { get; set; } = Assembly.GetEntryAssembly().GetName().Version;
        public string OS { get; set; } = RuntimeInformation.OSDescription;
        public string Framework { get; set; } = RuntimeInformation.FrameworkDescription;
        public Architecture Architecture { get; set; } = RuntimeInformation.OSArchitecture;
        public string DiscordNetVersion { get; set; } = Discord.DiscordConfig.Version;
        public Exception Exception { get; set; }
        public string Source { get; set; }

        public Crash(string source, Exception ex)
        {
            Exception = ex;
            Source = source;
        }
    }
}
