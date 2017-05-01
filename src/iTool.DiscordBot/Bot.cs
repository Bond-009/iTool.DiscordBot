using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
        Settings settings = Settings.Load();
        DependencyMap depMap = new DependencyMap();

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

            depMap.Add(new AudioService());
            depMap.Add(new Battlelog.Bf3.Bf3Client());
            depMap.Add(new Battlelog.Bf4.Bf4Client());
            depMap.Add(new Battlelog.BfH.BfHClient());
            depMap.Add(new BattlelogService());
            depMap.Add(new HOTSLogs.HOTSLogsClient());
            depMap.Add(new OpenWeather.OpenWeatherClient(settings.OpenWeatherMapKey));
            depMap.Add(settings);
            depMap.Add(new Steam.SteamAPI(settings.SteamKey));

            await new CommandHandler().Install(depMap, discordClient, new CommandServiceConfig()
            {
                CaseSensitiveCommands = settings.CaseSensitiveCommands,
                DefaultRunMode = settings.DefaultRunMode
            });

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

            depMap.Get<Battlelog.Bf3.Bf3Client>().Dispose();
            depMap.Get<Battlelog.Bf4.Bf4Client>().Dispose();
            depMap.Get<Battlelog.BfH.BfHClient>().Dispose();
            depMap.Get<BattlelogService>().Dispose();
            depMap.Get<HOTSLogs.HOTSLogsClient>().Dispose();
            depMap.Get<OpenWeather.OpenWeatherClient>().Dispose();;
            depMap.Get<Steam.SteamAPI>().Dispose();

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
