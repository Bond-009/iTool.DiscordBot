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
    public class Bot : IDisposable
    {
        private CommandService _commandService;
        private DiscordSocketClient _discordClient;
        private ILogger _logger;
        private ServiceProvider _serviceProvider;
        private Settings _settings = Settings.Load();
        private bool _disposed = false;

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
            _discordClient.Ready += OnDiscordClientReady;

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

            await LoadModules();

            await _discordClient.LoginAsync(TokenType.Bot, _settings.DiscordToken);
            await _discordClient.StartAsync();

            return true;
        }

        public async Task Stop()
        {
            await _discordClient.LogoutAsync();

            _settings.Save();
        }

        private async Task LoadModules()
        {
            Dictionary<string, bool> enabledModules = new Dictionary<string, bool>();

            if (File.Exists(Common.ModuleFile))
            {
                enabledModules = Toml.ReadFile<Dictionary<string, bool>>(Common.ModuleFile);
            }

            foreach (Type type in Assembly.GetEntryAssembly().GetExportedTypes()
                                            .Where(x => typeof(ModuleBase).IsAssignableFrom(x)
                                                || x.IsSubclassOfRawGeneric(typeof(ModuleBase<>))))
            {
                if (!enabledModules.Keys.Contains(type.Name))
                {
                    enabledModules.Add(type.Name, true);
                }

                if (enabledModules[type.Name])
                {
                    await _commandService.AddModuleAsync(type, _serviceProvider);
                    _logger.Information("Loaded {Module}", type.Name);
                }
            }

            Toml.WriteFile(enabledModules, Common.ModuleFile);
        }

        private async Task handleCommand(SocketMessage rawMsg)
        {
            // Ignore system messages
            if (!(rawMsg is SocketUserMessage msg))
            {
                return;
            }

            // Ignore bot messages
            if (msg.Author.IsBot || msg.Author.IsWebhook)
            {
                return;
            }

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

            string prefix = _settings.Prefix;
            if (_settings.GuildSpecificSettings
                && guildChannel != null)
            {
                using (GuildSettingsDatabase db = new GuildSettingsDatabase())
                {
                    prefix = (await db.GetSettingsAsync(guildChannel.Guild.Id))?.Prefix ?? _settings.Prefix;
                }
            }

            
            // Mark where the prefix ends and the command begins
            int argPos = 0;

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

        private async Task OnDiscordClientReady()
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
                "{Source}: {Message}", msg.Source, msg.Message);
            }
            else
            {
                _logger.Write((LogEventLevel)Utils.LogLevelFromSeverity(msg.Severity),
                    msg.Exception,
                    "{Source}: {Message}", msg.Source, msg.Message);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _discordClient.Dispose();
                _serviceProvider.Dispose();
            }

            _commandService = null;
            _discordClient = null;
            _serviceProvider = null;
            _settings = null;

            _discordClient.Log -= LogDiscord;
            _discordClient.Ready -= OnDiscordClientReady;

            _commandService.Log -= LogDiscord;
            _discordClient.MessageReceived -= handleCommand;

            _disposed = true;
        }
    }
}
