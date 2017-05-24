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

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(new AudioService())
                .AddSingleton(new AudioFileService())
                .AddSingleton(settings)
                .AddSingleton(new Steam.SteamAPI(settings.SteamKey))
                .BuildServiceProvider();

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
