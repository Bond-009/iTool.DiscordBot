using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public class Bot
    {
        DiscordSocketClient discordClient;
        List<string> bannedWords;
        Settings settings = Settings.Load();
        DependencyMap depMap = new DependencyMap();

        public async Task<bool> Start()
        {
            Logger.LogLevel = settings.LogLevel;

            bannedWords = Utils.LoadListFromFile(Common.SettingsDir + Path.DirectorySeparatorChar + "banned_words.txt").ToList();

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
            discordClient.MessageReceived += DiscordClient_MessageReceived;
            discordClient.Ready += DiscordClient_Ready;

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

        private async Task DiscordClient_MessageReceived(SocketMessage arg)
        {
            await Logger.Log(new LogMessage(LogSeverity.Verbose, nameof(Bot), arg.Author.Username + ": " + arg.Content));

            if (settings.AntiSwear && !bannedWords.IsNullOrEmpty()
                && bannedWords.Any(Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains))
            {
                await arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
            }
        }
    }
}
