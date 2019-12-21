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
using Microsoft.Extensions.Logging;
using SteamWebAPI2.Interfaces;
using Nett;

namespace iTool.DiscordBot
{
    public class Bot : IDisposable
    {
        private CommandService _commandService;
        private DiscordSocketClient _discordClient;
        private ILogger _logger;
        private ILoggerFactory _loggerFactory;
        private ServiceProvider _serviceProvider;
        private Settings _settings;
        private bool _disposed = false;

        public Bot(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Bot>();
        }

        public async Task<bool> StartAsync()
        {
            _settings = await Settings.LoadAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(_settings.DiscordToken))
            {
                _logger.LogError("No token");
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
                .AddSingleton(_loggerFactory)
                .AddLogging()
                .AddSingleton(_settings)
                .AddSingleton(typeof(AudioService))
                .AddSingleton(typeof(AudioFileService));

            if (string.IsNullOrEmpty(_settings.SteamKey))
            {
                _logger.LogWarning("No steam api key found");
            }
            else
            {
                serviceCollection.AddSingleton<ISteamUser>(new SteamUser(_settings.SteamKey))
                    .AddSingleton<ISteamUserStats>(new SteamUserStats(_settings.SteamKey));
            }

            if (string.IsNullOrEmpty(_settings.OpenWeatherMapKey))
            {
                _logger.LogWarning("No OpenWeatherMap api key found");
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

            _discordClient.MessageReceived += HandleCommandAsync;

            await LoadModulesAsync();

            await _discordClient.LoginAsync(TokenType.Bot, _settings.DiscordToken).ConfigureAwait(false);
            await _discordClient.StartAsync().ConfigureAwait(false);

            return true;
        }

        public async Task StopAsync()
        {
            await _discordClient.LogoutAsync().ConfigureAwait(false);

            await _settings.SaveAsync().ConfigureAwait(false);
        }

        private async Task LoadModulesAsync()
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
                enabledModules.TryAdd(type.Name, true);

                if (enabledModules[type.Name])
                {
                    await _commandService.AddModuleAsync(type, _serviceProvider).ConfigureAwait(false);
                    _logger.LogInformation("Loaded {Module}", type.Name);
                }
            }

            Toml.WriteFile(enabledModules, Common.ModuleFile);
        }

        private async Task HandleCommandAsync(SocketMessage rawMsg)
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
            if (_settings.BlacklistedUsers != null
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
                {
                    return;
                }
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
            {
                return;
            }

            // Execute the Command, store the result
            IResult result = await _commandService.ExecuteAsync(new SocketCommandContext(_discordClient, msg), argPos, _serviceProvider);

            // If the command failed, notify the user
            if (result.Error.HasValue && result.Error != CommandError.UnknownCommand)
            {
                _logger.LogError(result.ErrorReason);

                await msg.Channel.SendMessageAsync(string.Empty,
                    embed: new EmbedBuilder()
                    {
                        Title = "Error",
                        Color = _settings.GetErrorColor(),
                        Description = result.ErrorReason
                    }.Build()).ConfigureAwait(false);
            }
        }

        private async Task OnDiscordClientReady()
        {
            Console.Title = _discordClient.CurrentUser.ToString();

            if (!string.IsNullOrEmpty(_settings.Game))
            {
                await _discordClient.SetGameAsync(_settings.Game);
            }
        }

        public Task LogDiscord(LogMessage msg)
        {
            if (msg.Exception == null)
            {
                _logger.Log(
                    (LogLevel)Utils.LogLevelFromSeverity(msg.Severity),
                    "{Source}: {Message}",
                    msg.Source,
                    msg.Message);
            }
            else
            {
                _logger.Log(
                    (LogLevel)Utils.LogLevelFromSeverity(msg.Severity),
                    msg.Exception,
                    "{Source}: {Message}",
                    msg.Source,
                    msg.Message);
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

            _discordClient.Log -= LogDiscord;
            _discordClient.Ready -= OnDiscordClientReady;

            _commandService.Log -= LogDiscord;
            _discordClient.MessageReceived -= HandleCommandAsync;

            _commandService = null;
            _discordClient = null;
            _serviceProvider = null;
            _settings = null;

            _disposed = true;
        }
    }
}
