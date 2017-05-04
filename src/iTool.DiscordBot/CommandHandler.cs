using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using YamlDotNet.Serialization;

namespace iTool.DiscordBot
{
    public class CommandHandler
    {
        CommandService commandService;
        DiscordSocketClient client;
        IServiceProvider serviceProvider;
        Settings settings;

        public CommandHandler(IServiceProvider serviceProvider, DiscordSocketClient discordClient, CommandServiceConfig config)
        {
            commandService = new CommandService(config);
            commandService.Log += Logger.Log;

            client = discordClient;
            this.serviceProvider = serviceProvider;
            settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

            client.MessageReceived += HandleCommand;
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
                    await commandService.AddModuleAsync(type);
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
            if (!settings.BlacklistedUsers.IsNullOrEmpty()
                && settings.BlacklistedUsers.Contains(message.Author.Id))
            {
                return;
            }

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message has a valid prefix, adjust argPos
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos)
                || message.HasStringPrefix(settings.Prefix, ref argPos)))
            { return; }

            // Execute the Command, store the result
            IResult result = await commandService.ExecuteAsync(new SocketCommandContext(client, message), argPos, serviceProvider);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result is PreconditionResult || result is SearchResult)
                { return; }

                await Logger.Log(new LogMessage(LogSeverity.Error, nameof(CommandHandler), result.ErrorReason));

                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder()
                {
                    Title = "Error",
                    Color = new Color((uint)settings.ErrorColor),
                    Description = result.ErrorReason
                });
            }
        }

        private static bool IsSubclassOfRawGeneric(Type baseType, Type derivedType)
        {
            while (derivedType != null && derivedType != typeof(object))
            {
                var currentType = derivedType.GetTypeInfo().IsGenericType ? derivedType.GetGenericTypeDefinition() : derivedType;
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
