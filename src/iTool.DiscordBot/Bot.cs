using BfStats.BfH;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using iTool.DiscordBot.Audio;
using iTool.DiscordBot.Steam;
using OpenWeather;
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
        DependencyMap map = new DependencyMap();
        DiscordSocketClient discordClient;
        List<string> bannedWords;

        public async Task Start()
        {
            map.Add(Settings.Load());
            Logger.LogLevel = map.Get<Settings>().LogLevel;

            bannedWords = Utils.LoadListFromFile(Common.SettingsDir + Path.DirectorySeparatorChar + "banned_words.txt").ToList();

            if (string.IsNullOrEmpty(map.Get<Settings>().DiscordToken))
            {
                Console.WriteLine("No token");
                return;
            }

            discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = map.Get<Settings>().AlwaysDownloadUsers,
                ConnectionTimeout = map.Get<Settings>().ConnectionTimeout,
                DefaultRetryMode = map.Get<Settings>().DefaultRetryMode,
                LogLevel = map.Get<Settings>().LogLevel,
                MessageCacheSize = map.Get<Settings>().MessageCacheSize
            });

            discordClient.Log += Logger.Log;
            discordClient.MessageReceived += DiscordClient_MessageReceived;
            discordClient.Ready += DiscordClient_Ready;

            await discordClient.LoginAsync(TokenType.Bot, map.Get<Settings>().DiscordToken);
            await discordClient.StartAsync();

            map.Add(discordClient);
            map.Add(new AudioService());
            map.Add(new BfHStatsClient(true));
            map.Add(new OpenWeatherClient(map.Get<Settings>().OpenWeatherMapKey));
            map.Add(new SteamAPI(map.Get<Settings>().SteamKey));

            await new CommandHandler().Install(map, new CommandServiceConfig()
            {
                CaseSensitiveCommands = map.Get<Settings>().CaseSensitiveCommands,
                DefaultRunMode = map.Get<Settings>().DefaultRunMode
            });
        }

        public async Task Stop()
        {
            await discordClient.LogoutAsync();
            discordClient.Dispose();

            // TODO: Dispose OpenWeatherClient, BfHStatsClient and SteamAPI

            Settings.Save(map.Get<Settings>());
        }

        private async Task DiscordClient_Ready()
        {
            Console.Title = discordClient.CurrentUser.ToString();

            if (!map.Get<Settings>().Game.IsNullOrEmpty())
            {
                await discordClient.SetGameAsync(map.Get<Settings>().Game);
            }
        }

        private async Task DiscordClient_MessageReceived(SocketMessage arg)
        {
            await Logger.Log(new LogMessage(LogSeverity.Verbose, nameof(Bot), arg.Author.Username + ": " + arg.Content));

            if (map.Get<Settings>().AntiSwear && !bannedWords.IsNullOrEmpty()
                && bannedWords.Any(Regex.Replace(arg.Content.ToLower(), "[^A-Za-z0-9]", "").Contains))
            {
                await arg.DeleteAsync();
                await arg.Channel.SendMessageAsync(arg.Author.Mention + ", please don't put such things in chat");
            }
        }
    }
}
