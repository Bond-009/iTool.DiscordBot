using Discord;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public static class Logger
    {
        static ILoggerFactory LoggerFactory = new LoggerFactory()
                .AddFile(Common.LogsDir + Path.DirectorySeparatorChar + "log-{Date}.log");
        static ILogger logger = LoggerFactory.CreateLogger(string.Empty);
        public static LogSeverity LogLevel { get; internal set; } = LogSeverity.Verbose;

        public static Task Log(LogMessage msg)
        {
            if (msg.Severity > LogLevel)
            { return Task.CompletedTask; }

            switch(msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    logger.LogError($"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    logger.LogInformation($"{msg.Source}: {msg.Message}");
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine(msg.ToString());

            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}