using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public class Bot
    {
        DiscordSocketClient discordClient;
        IServiceProvider serviceProvider;
        Settings settings = Settings.Load();

        public async Task<bool> Start()
        {
            Logger.LogLevel = settings.LogLevel;

            if (string.IsNullOrEmpty(settings.DiscordToken))
            {
                Console.WriteLine("No token");
                return false;
            }

            discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = settings.AlwaysDownloadUsers,
                ConnectionTimeout = settings.ConnectionTimeout,
                DefaultRetryMode = settings.DefaultRetryMode,
                LogLevel = settings.LogLevel,
                MessageCacheSize = settings.MessageCacheSize
            });

            discordClient.Log += Logger.Log;
            discordClient.Ready += DiscordClient_Ready;
            discordClient.MessageReceived += async msg
                => await Logger.Log(new LogMessage(LogSeverity.Verbose, nameof(Bot), msg.Author.Username + ": " + msg.Content));

            await discordClient.LoginAsync(TokenType.Bot, settings.DiscordToken);
            await discordClient.StartAsync();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new AudioService());
            serviceCollection.AddSingleton(new Battlelog.Bf3.Bf3Client());
            serviceCollection.AddSingleton(new Battlelog.Bf4.Bf4Client());
            serviceCollection.AddSingleton(new Battlelog.BfH.BfHClient());
            serviceCollection.AddSingleton(new BattlelogService());
            serviceCollection.AddSingleton(new HOTSLogs.HOTSLogsClient());
            serviceCollection.AddSingleton(new OpenWeather.OpenWeatherClient(settings.OpenWeatherMapKey));
            serviceCollection.AddSingleton(settings);
            serviceCollection.AddSingleton(new Steam.SteamAPI(settings.SteamKey));
            serviceProvider = serviceCollection.BuildServiceProvider();

            await new CommandHandler(serviceProvider, discordClient, new CommandServiceConfig()
            {
                CaseSensitiveCommands = settings.CaseSensitiveCommands,
                DefaultRunMode = settings.DefaultRunMode
            }).LoadModules();

            if (settings.AntiSwear)
            {
                new AntiSwear(discordClient);
            }

            return true;
        }

        public async Task Stop()
        {
            await discordClient.LogoutAsync();
            discordClient.Dispose();

            serviceProvider.GetService<Battlelog.Bf3.Bf3Client>().Dispose();
            serviceProvider.GetService<Battlelog.Bf4.Bf4Client>().Dispose();
            serviceProvider.GetService<Battlelog.BfH.BfHClient>().Dispose();
            serviceProvider.GetService<BattlelogService>().Dispose();
            serviceProvider.GetService<HOTSLogs.HOTSLogsClient>().Dispose();
            serviceProvider.GetService<OpenWeather.OpenWeatherClient>().Dispose();;
            serviceProvider.GetService<Steam.SteamAPI>().Dispose();

            Settings.Save(settings);
        }

        private async Task DiscordClient_Ready()
        {
            Console.Title = discordClient.CurrentUser.ToString();

            if (!settings.Game.IsNullOrEmpty())
            {
                await discordClient.SetGameAsync(settings.Game);
            }
        }
    }
}
