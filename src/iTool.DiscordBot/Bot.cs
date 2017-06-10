using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
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

            if (string.IsNullOrEmpty(_settings.DiscordToken))
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
            _discordClient.Ready += DiscordClient_Ready;
            _discordClient.MessageReceived += async msg
                => await Logger.Log(new LogMessage(LogSeverity.Verbose, nameof(Bot), msg.Author.Username + ": " + msg.Content));

            await _discordClient.LoginAsync(TokenType.Bot, _settings.DiscordToken);
            await _discordClient.StartAsync();

            _serviceProvider = new ServiceCollection()
                .AddSingleton(new AudioService())
                .AddSingleton(new AudioFileService())
                .AddSingleton(_settings)
                .AddSingleton(new Steam.SteamAPI(_settings.SteamKey))
                .BuildServiceProvider();

            _commandService = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = _settings.CaseSensitiveCommands,
                DefaultRunMode = _settings.DefaultRunMode
            });
            _commandService.Log += Logger.Log;

            _discordClient.MessageReceived += HandleCommand;

            await LoadModules();

            if (_settings.AntiSwear)
            {
                new AntiSwear(_discordClient).AddHandler();
            }

            return true;
        }

        public async Task Stop()
        {
            await _discordClient.LogoutAsync();
            _discordClient.Dispose();

            Settings.Save(_settings);
        }

        private async Task LoadModules()
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
                    await Logger.Log(new LogMessage(LogSeverity.Info, nameof(Program), $"Loaded {type.Name} module"));
                }
            }
            File.WriteAllText(Common.ModuleFile,
                    new SerializerBuilder().EmitDefaults().Build()
                        .Serialize(enabledmodules));
        }

        private async Task HandleCommand(SocketMessage parameterMessage)
        {
            // Don't handle the command if it is a system message
            SocketUserMessage message = parameterMessage as SocketUserMessage;
            if (message == null) { return; }

            // Check if the user is blacklisted. If so return
            if (!_settings.BlacklistedUsers.IsNullOrEmpty()
                && _settings.BlacklistedUsers.Contains(message.Author.Id))
            {
                return;
            }

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            string prefix = _settings.Prefix;
            if (_settings.GuildSpecificSettings
                && (message.Channel as IGuildChannel)?.Guild != null)
            {
                using (GuildSettingsDatabase db = new GuildSettingsDatabase())
                {
                    prefix = (await db.GetSettingsAsync((message.Channel as IGuildChannel).Guild.Id))?.Prefix ?? _settings.Prefix;
                }
            }

            // Determine if the message has a valid prefix, adjust argPos
            if (!(message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)
                || message.HasStringPrefix(prefix, ref argPos)))
            { return; }

            // Execute the Command, store the result
            IResult result = await _commandService.ExecuteAsync(new SocketCommandContext(_discordClient, message), argPos, _serviceProvider);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result is PreconditionResult || result is SearchResult)
                { return; }

                await Logger.Log(new LogMessage(LogSeverity.Error, nameof(Program), result.ErrorReason));

                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder()
                {
                    Title = "Error",
                    Color = _settings.GetErrorColor(),
                    Description = result.ErrorReason
                });
            }
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
