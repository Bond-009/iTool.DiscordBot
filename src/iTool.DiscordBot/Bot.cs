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
using SteamWebAPI2.Interfaces;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public class Bot
    {
        private CommandService _commandService;
        private DiscordSocketClient _discordClient;
        private IServiceProvider _serviceProvider;
        private Settings _settings = Settings.Load();

        public async Task<bool> Start()
        {
            Logger.LogLevel = _settings.LogLevel;

            if (_settings.DiscordToken.IsNullOrEmpty())
            {
                Console.WriteLine("No token");
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
                LogLevel = _settings.LogLevel,
                MessageCacheSize = _settings.MessageCacheSize
            });

            _discordClient.Log += Logger.Log;
            _discordClient.Ready += discordClientReady;
            _discordClient.MessageReceived += async msg
                => await Logger.Log(new LogMessage(LogSeverity.Verbose, nameof(Bot), msg.Author.Username + ": " + msg.Content));

            
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddSingleton<AudioService>()
                .AddSingleton<AudioFileService>()
                .AddSingleton(_settings);
            
            if (_settings.SteamKey.IsNullOrEmpty())
            {
                await Logger.Log(new LogMessage(LogSeverity.Warning, nameof(Program), "No steamkey found."));
            }
            else
            {
                serviceCollection.AddSingleton<ISteamUser>(new SteamUser(_settings.SteamKey));
                serviceCollection.AddSingleton<ISteamUserStats>(new SteamUserStats(_settings.SteamKey));
            }

            if (_settings.OpenWeatherMapKey.IsNullOrEmpty())
            {
                await Logger.Log(new LogMessage(LogSeverity.Warning, nameof(Program), "No steamkey found."));
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
            _commandService.Log += Logger.Log;

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
                enabledmodules = new Deserializer().Deserialize<Dictionary<string, bool>>(File.ReadAllText(Common.ModuleFile));
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
                    await _commandService.AddModuleAsync(type);
                    await Logger.Log(new LogMessage(LogSeverity.Info, nameof(Program), $"Loaded {type.Name}"));
                }
            }
            File.WriteAllText(Common.ModuleFile,
                    new SerializerBuilder().EmitDefaults().Build()
                        .Serialize(enabledmodules));
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

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            string prefix = _settings.Prefix;
            if (_settings.GuildSpecificSettings
                && msg.Channel is IGuildChannel guildChannel)
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
            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand)
                { return; }

                await Logger.Log(new LogMessage(LogSeverity.Error, nameof(Program), result.ErrorReason));

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
    }
}
