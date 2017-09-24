using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.SystemConsole.Themes;

namespace iTool.DiscordBot
{
    public static class Program
    {
        private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public static async Task<int> Main(string[] args)
        {
            Settings settings = Settings.Load();

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Is(settings.LogLevel)
                            .WriteTo.Console(theme: AnsiConsoleTheme.Code, 
                                outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                            .WriteTo.Async(a => a.RollingFile(Path.Combine(AppContext.BaseDirectory, "logs", "log_{Date}.log"),
                                buffered: true,
                                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"))
                            .CreateLogger();

            Console.CancelKeyPress += cancelKeyPress;

            Bot iToolBot = new Bot(settings, Log.Logger);
            if (!await iToolBot.Start())
            {
                if (!Console.IsInputRedirected) Console.ReadKey();
                return 1;
            }

            if (!Console.IsInputRedirected)
            {
                Task task = Task.Run(() => handleInput(), _tokenSource.Token);
            }

            await Task.Delay(-1, _tokenSource.Token).ContinueWith(x => {});

            await iToolBot.Stop();

            Log.CloseAndFlush();

            return 0;
        }

        private static void handleInput()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                string input = Console.ReadLine().ToLower();
                switch (input)
                {
                    case "quit":
                    case "exit":
                    case "stop":
                        _tokenSource.Cancel();
                        break;
                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;
                    case "":
                        break;
                    default:
                        Console.WriteLine(input + ": command not found");
                        break;
                }
            }
        }

        private static void cancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _tokenSource.Cancel();
        }
    }
}
