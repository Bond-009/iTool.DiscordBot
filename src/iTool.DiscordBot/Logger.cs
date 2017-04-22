using Discord;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public static class Logger
    {
        static ILogger logger = new LoggerConfiguration()
                .WriteTo.RollingFile(Common.LogsDir + Path.DirectorySeparatorChar + "log-{Date}.log")
                .CreateLogger();
        public static LogSeverity LogLevel { get; internal set; } = LogSeverity.Verbose;

        public static Task Log(LogMessage msg)
        {
            if (msg.Severity > LogLevel)
            { return Task.CompletedTask; }

            switch(msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    logger.Error($"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    logger.Warning($"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    logger.Information($"{msg.Source}: {msg.Message}");
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    logger.Debug($"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine(msg.ToString());

            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}