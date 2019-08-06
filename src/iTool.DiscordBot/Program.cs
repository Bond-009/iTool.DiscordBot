using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.AspNetCore;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace iTool.DiscordBot
{
    public static class Program
    {
        private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private static ILogger _logger;
        private static ILoggerFactory _loggerFactory;

        public static async Task<int> Main()
        {
            await CreateLoggerAsync();
            _loggerFactory = new SerilogLoggerFactory();
            _logger = _loggerFactory.CreateLogger("Main");
            _logger.LogInformation("Starting iTool.DiscordBot");

            Console.CancelKeyPress += OnCancelKeyPress;

            using (Bot iToolBot = new Bot(_loggerFactory))
            {
                if (!await iToolBot.StartAsync())
                {
                    if (!Console.IsInputRedirected)
                    {
                        Console.ReadKey();
                    }

                    return 1;
                }

                if (!Console.IsInputRedirected)
                {
                    _ = Task.Run(HandleInput, _tokenSource.Token);
                }

                try
                {
                    await Task.Delay(-1, _tokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    // Don't throw on cancellation
                }

                await iToolBot.StopAsync().ConfigureAwait(false);
            }

            return 0;
        }

        private static async Task CreateLoggerAsync()
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "settings", "serilog.json");

                if (!File.Exists(path))
                {
                    var assembly = typeof(Program).GetTypeInfo().Assembly;
                    using (Stream rscstr = assembly.GetManifestResourceStream("iTool.DiscordBot.Resources.Configuration.serilog.json"))
                    using (Stream fstr = File.Open(path, FileMode.CreateNew))
                    {
                        await rscstr.CopyToAsync(fstr).ConfigureAwait(false);
                    }
                }

                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(path)
                    .Build();

                Serilog.Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                Serilog.Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.Async(x => x.File(
                        Path.Combine(AppContext.BaseDirectory, "logs", "log_.log"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message}{NewLine}{Exception}"))
                    .Enrich.FromLogContext()
                    .CreateLogger();

                Log.Logger.Fatal(ex, "Failed to read logger configuration");
            }
        }

        private static void HandleInput()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                string input = Console.ReadLine().ToLowerInvariant();
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

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _tokenSource.Cancel();
        }
    }
}
