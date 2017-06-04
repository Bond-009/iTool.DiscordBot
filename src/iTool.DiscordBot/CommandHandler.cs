using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public class CommandHandler
    {
        private CommandService _commandService;
        private DiscordSocketClient _client;
        private IServiceProvider _serviceProvider;
        private Settings _settings;

        public CommandHandler(IServiceProvider serviceProvider, DiscordSocketClient discordClient, CommandServiceConfig config)
        {
            _commandService = new CommandService(config);
            _commandService.Log += Logger.Log;

            _client = discordClient;
            this._serviceProvider = serviceProvider;
            _settings = (Settings)this._serviceProvider.GetService(typeof(Settings));

            _client.MessageReceived += HandleCommand;
        }

        public async Task LoadModules()
        {
            Dictionary<string, bool> enabledmodules = new Dictionary<string, bool>();

            if (File.Exists(Common.ModuleFile))
            {
                enabledmodules = new Deserializer().Deserialize<Dictionary<string, bool>>(File.ReadAllText(Common.ModuleFile));
            }

            foreach (Type type in Assembly.GetEntryAssembly().GetExportedTypes()
                                            .Where(x => typeof(ModuleBase).IsAssignableFrom(x)
                                                || IsSubclassOfRawGeneric(typeof(ModuleBase<>), x)))
            {
                if (!enabledmodules.Keys.Contains(type.Name))
                {
                    enabledmodules.Add(type.Name, true);
                }
                if (enabledmodules[type.Name])
                {
                    await _commandService.AddModuleAsync(type);
                    await Logger.Log(new LogMessage(LogSeverity.Info, nameof(CommandHandler), $"Loaded {type.Name} module"));
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
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos)
                || message.HasStringPrefix(prefix, ref argPos)))
            { return; }

            // Execute the Command, store the result
            IResult result = await _commandService.ExecuteAsync(new SocketCommandContext(_client, message), argPos, _serviceProvider);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result is PreconditionResult || result is SearchResult)
                { return; }

                await Logger.Log(new LogMessage(LogSeverity.Error, nameof(CommandHandler), result.ErrorReason));

                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder()
                {
                    Title = "Error",
                    Color = _settings.GetErrorColor(),
                    Description = result.ErrorReason
                });
            }
        }

        private static bool IsSubclassOfRawGeneric(Type baseType, Type derivedType)
        {
            while (derivedType != null && derivedType != typeof(object))
            {
                Type currentType = derivedType.GetTypeInfo().IsGenericType ? derivedType.GetGenericTypeDefinition() : derivedType;
                if (baseType == currentType)
                {
                    return true;
                }

                derivedType = derivedType.GetTypeInfo().BaseType;
            }
            return false;
        }
    }
}
