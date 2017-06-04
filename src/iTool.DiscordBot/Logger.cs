using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Serilog;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public static class Logger
    {
        private static readonly ILogger _logger = new LoggerConfiguration()
                .WriteTo.RollingFile(Path.Combine(AppContext.BaseDirectory, "logs", "log_{Date}.log"))
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
                    if (msg.Exception == null)
                        _logger.Error($"{msg.Source}: {msg.Message}");
                    else
                        _logger.Error(msg.Exception, $"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    _logger.Warning($"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    _logger.Information($"{msg.Source}: {msg.Message}");
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.Debug($"{msg.Source}: {msg.Message}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine(msg.ToString());

            Console.ResetColor();

            if (msg.Exception != null) Crash(new Crash(msg.Source, msg.Exception));

            return Task.CompletedTask;
        }

        private static void Crash(Crash crash)
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "crashes"));
            File.WriteAllText(
                Path.Combine(AppContext.BaseDirectory, "crashes", $"crash_{DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss")}.txt"),
                new Serializer().Serialize(crash)
            );
        }
    }
}
