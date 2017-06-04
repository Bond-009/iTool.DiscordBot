using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace iTool.DiscordBot
{
    public class Bot
    {
        private DiscordSocketClient _discordClient;
        private Settings _settings = Settings.Load();

        public async Task<bool> Start()
        {
            Logger.LogLevel = _settings.LogLevel;

            if (string.IsNullOrEmpty(_settings.DiscordToken))
            {
                Console.WriteLine("No token");
                return false;
            }

            _discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = _settings.AlwaysDownloadUsers,
                ConnectionTimeout = _settings.ConnectionTimeout,
                DefaultRetryMode = _settings.DefaultRetryMode,
                LogLevel = _settings.LogLevel,
                MessageCacheSize = _settings.MessageCacheSize
            });

            _discordClient.Log += Logger.Log;
            _discordClient.Ready += DiscordClient_Ready;
            _discordClient.MessageReceived += async msg
                => await Logger.Log(new LogMessage(LogSeverity.Verbose, nameof(Bot), msg.Author.Username + ": " + msg.Content));

            await _discordClient.LoginAsync(TokenType.Bot, _settings.DiscordToken);
            await _discordClient.StartAsync();

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton(new AudioService())
                .AddSingleton(new AudioFileService())
                .AddSingleton(_settings)
                .AddSingleton(new Steam.SteamAPI(_settings.SteamKey))
                .BuildServiceProvider();

            await new CommandHandler(serviceProvider, _discordClient, new CommandServiceConfig()
            {
                CaseSensitiveCommands = _settings.CaseSensitiveCommands,
                DefaultRunMode = _settings.DefaultRunMode
            }).LoadModules();

            if (_settings.AntiSwear)
            {
                new AntiSwear(_discordClient);
            }

            return true;
        }

        public async Task Stop()
        {
            await _discordClient.LogoutAsync();
            _discordClient.Dispose();

            Settings.Save(_settings);
        }

        private async Task DiscordClient_Ready()
        {
            Console.Title = _discordClient.CurrentUser.ToString();

            if (!_settings.Game.IsNullOrEmpty())
            {
                await _discordClient.SetGameAsync(_settings.Game);
            }
        }
    }
}
