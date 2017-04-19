using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace iTool.DiscordBot
{
    public class CommandHandler
    {
        private CommandService CommandService;
        private DiscordSocketClient client;
        private IDependencyMap map;

        public async Task Install(IDependencyMap _map, DiscordSocketClient discordClient, CommandServiceConfig config)
        {
            client = discordClient;
            CommandService = new CommandService(config);
            map = _map;

            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

            // HACK: Loads all commands and than unloads the disabled modules
            if (File.Exists(Common.SettingsDir + Path.DirectorySeparatorChar + "disabled_modules.txt"))
            {
                IEnumerable<string> disabledModules = Utils.LoadListFromFile(Common.SettingsDir + Path.DirectorySeparatorChar + "disabled_modules.txt");

                foreach (ModuleInfo moduleInfo in CommandService.Modules.Where(x => disabledModules.Contains(x.Name)).ToArray())
                {
                    await Logger.Log(new LogMessage(LogSeverity.Info, nameof(CommandHandler), $"Disabled {moduleInfo.Name} module"));
                    await CommandService.RemoveModuleAsync(moduleInfo);
                }
            }

            client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {
            // Don't handle the command if it is a system message
            SocketUserMessage message = parameterMessage as SocketUserMessage;
            if (message == null) { return; }

            // Check if the user is blacklisted. If so return
            if (!map.Get<Settings>().BlacklistedUsers.IsNullOrEmpty()
                 && map.Get<Settings>().BlacklistedUsers.Contains(message.Author.Id))
            {
                return;
            }

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message has a valid prefix, adjust argPos 
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) 
                || message.HasStringPrefix(map.Get<Settings>().Prefix, ref argPos)))
            { return; }

            // Execute the Command, store the result
            IResult result = await CommandService.ExecuteAsync(new SocketCommandContext(client, message), argPos, map);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result is PreconditionResult || result is SearchResult)
                { return; }

                await Logger.Log(new LogMessage(LogSeverity.Error, nameof(CommandHandler), result.ErrorReason));

                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder()
                {
                    Title = "Error",
                    Color = new Color((uint)map.Get<Settings>().ErrorColor),
                    Description = result.ErrorReason
                });
            }
        }
    }
}
