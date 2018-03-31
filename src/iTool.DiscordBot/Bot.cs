using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenWeather;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SteamWebAPI2.Interfaces;
using Nett;

namespace iTool.DiscordBot
{
    public class Bot
    {
        private CommandService _commandService;
        private DiscordSocketClient _discordClient;
        private ILogger _logger;
        private IServiceProvider _serviceProvider;
        private Settings _settings = Settings.Load();

        public Bot(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> Start()
        {
            if (_settings.DiscordToken.IsNullOrEmpty())
            {
                _logger.Warning("No token");
                return false;
            }

            if (_settings.GuildSpecificSettings)
            {
                using (GuildSettingsDatabase db = new GuildSettingsDatabase())
                {
                    db.Database.EnsureCreated();
                }
            }

            _discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = _settings.AlwaysDownloadUsers,
                ConnectionTimeout = _settings.ConnectionTimeout,
                DefaultRetryMode = _settings.DefaultRetryMode,
                MessageCacheSize = _settings.MessageCacheSize
            });

            _discordClient.Log += LogDiscord;
            _discordClient.Ready += discordClientReady;

            IServiceCollection serviceCollection = new ServiceCollection()
                .AddSingleton(_logger)
                .AddSingleton(_settings)
                .AddSingleton(new AudioService(_logger))
                .AddSingleton(new AudioFileService(_logger));
            
            if (_settings.SteamKey.IsNullOrEmpty())
            {
                _logger.Warning("No steamkey");
            }
            else
            {
                serviceCollection.AddSingleton<ISteamUser>(new SteamUser(_settings.SteamKey));
                serviceCollection.AddSingleton<ISteamUserStats>(new SteamUserStats(_settings.SteamKey));
            }

            if (_settings.OpenWeatherMapKey.IsNullOrEmpty())
            {
                _logger.Warning("No openweathermap key");
            }
            else
            {
                serviceCollection.AddSingleton(new OpenWeatherClient(_settings.OpenWeatherMapKey, Unit.Metric));
            }

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _commandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = _settings.CaseSensitiveCommands,
                DefaultRunMode = _settings.DefaultRunMode
            });
            _commandService.Log += LogDiscord;

            _discordClient.MessageReceived += handleCommand;

            await loadModules();

            if (_settings.AntiSwear)
            {
                new AntiSwear(_discordClient).AddHandler();
            }

            await _discordClient.LoginAsync(TokenType.Bot, _settings.DiscordToken);
            await _discordClient.StartAsync();

            return true;
        }

        public async Task Stop()
        {
            await _discordClient.LogoutAsync();
            _discordClient.Dispose();

            _settings.Save();
        }

        private async Task loadModules()
        {
            Dictionary<string, bool> enabledmodules = new Dictionary<string, bool>();

            if (File.Exists(Common.ModuleFile))
            {
                enabledmodules = Toml.ReadFile<Dictionary<string, bool>>(Common.ModuleFile);
            }

            foreach (Type type in Assembly.GetEntryAssembly().GetExportedTypes()
                                            .Where(x => typeof(ModuleBase).IsAssignableFrom(x)
                                                || x.IsSubclassOfRawGeneric(typeof(ModuleBase<>))))
            {
                if (!enabledmodules.Keys.Contains(type.Name))
                {
                    enabledmodules.Add(type.Name, true);
                }
                if (enabledmodules[type.Name])
                {
                    await _commandService.AddModuleAsync(type, _serviceProvider);
                    _logger.Information($"Loaded {type.Name}");
                }
            }
            Toml.WriteFile(enabledmodules, Common.ModuleFile);
        }

        private async Task handleCommand(SocketMessage rawMsg)
        {
            // Ignore system messages
            if (!(rawMsg is SocketUserMessage msg)) return;
            // Ignore bot messages
            if (msg.Author.IsBot || msg.Author.IsWebhook) return;

            // Ignore messages from blacklisted users
            if (!_settings.BlacklistedUsers.IsNullOrEmpty()
                && _settings.BlacklistedUsers.Contains(msg.Author.Id))
            {
                return;
            }

            // Ignore if the bot doesn't have the permission to send messages in the channel
            IGuildChannel guildChannel = msg.Channel as IGuildChannel;

            if (guildChannel != null)
            {
                IGuildUser guildUser = await guildChannel.Guild.GetCurrentUserAsync();
                ChannelPermissions channelPermissions = guildUser.GetPermissions(guildChannel);
                if (!channelPermissions.Has(ChannelPermission.SendMessages))
                { return; }
            }

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            string prefix = _settings.Prefix;
            if (_settings.GuildSpecificSettings
                && guildChannel != null)
            {
                using (GuildSettingsDatabase db = new GuildSettingsDatabase())
                {
                    prefix = (await db.GetSettingsAsync(guildChannel.Guild.Id))?.Prefix ?? _settings.Prefix;
                }
            }

            // Determine if the message has a valid prefix, adjust argPos
            if (!(msg.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)
                || msg.HasStringPrefix(prefix, ref argPos)))
            { return; }

            // Execute the Command, store the result
            IResult result = await _commandService.ExecuteAsync(new SocketCommandContext(_discordClient, msg), argPos, _serviceProvider);

            // If the command failed, notify the user
            if (result.Error.HasValue && result.Error != CommandError.UnknownCommand)
            {
                _logger.Error(result.ErrorReason);

                await msg.Channel.SendMessageAsync("", embed: new EmbedBuilder()
                {
                    Title = "Error",
                    Color = _settings.GetErrorColor(),
                    Description = result.ErrorReason
                }.Build());
            }
        }

        private async Task discordClientReady()
        {
            Console.Title = _discordClient.CurrentUser.ToString();

            if (!_settings.Game.IsNullOrEmpty())
            {
                await _discordClient.SetGameAsync(_settings.Game);
            }
        }

        public Task LogDiscord(LogMessage msg)
        {
            if (msg.Exception == null)
            {
                _logger.Write((LogEventLevel)Utils.LogLevelFromSeverity(msg.Severity),
                    $"{msg.Source}: {msg.Message}"
                );
            }
            else
            {
                _logger.Write((LogEventLevel)Utils.LogLevelFromSeverity(msg.Severity),
                    msg.Exception,
                    $"{msg.Source}: {msg.Message}"
                );
            }
            return Task.CompletedTask;
        }
    }
}
