using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Extensions.Logging;
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

            // Log uncaught exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Intercept Ctrl+C and Ctrl+Break
            Console.CancelKeyPress += OnCancelKeyPress;

            // Register a SIGTERM handler
            AppDomain.CurrentDomain.ProcessExit += OnProccessExit;

            using (Bot iToolBot = new Bot(_loggerFactory))
            {
                if (!await iToolBot.StartAsync())
                {
                    if (!Console.IsInputRedirected)
                    {
                        _logger.LogInformation("Press a key to continue...");
                        Console.ReadKey();
                    }

                    // Otherwise we'll log a SIGTERM
                    _tokenSource.Cancel();
                    return 1;
                }

                try
                {
                    await Task.Delay(-1, _tokenSource.Token).ConfigureAwait(false);
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
                    using (Stream fstr = File.Create(path))
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

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
            => _logger.LogCritical((Exception)e.ExceptionObject, "Unhandled Exception");

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_tokenSource.IsCancellationRequested)
            {
                return; // Already shutting down
            }

            e.Cancel = true;
            _logger.LogInformation("Ctrl+C, shutting down");
            Environment.ExitCode = 128 + 2;
            _tokenSource.Cancel();
        }

        private static void OnProccessExit(object sender, EventArgs e)
        {
            if (_tokenSource.IsCancellationRequested)
            {
                return; // Already shutting down
            }

            _logger.LogInformation("Received a SIGTERM signal, shutting down");
            Environment.ExitCode = 128 + 15;
            _tokenSource.Cancel();
        }
    }
}
