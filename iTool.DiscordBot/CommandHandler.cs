using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

namespace iTool.DiscordBot
{
    public class CommandHandler
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IDependencyMap map;

        public async Task Install(IDependencyMap _map)
        {
            // Create Command Service, inject it into Dependency Map
            client = _map.Get<DiscordSocketClient>();
            commands = new CommandService();
            _map.Add(commands);
            map = _map;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {
            // Don't handle the command if it is a system message
            SocketUserMessage message = parameterMessage as SocketUserMessage;
            if (message == null) return;

            // Mark where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message has a valid prefix, adjust argPos 
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasCharPrefix('!', ref argPos))) return;

            // Execute the Command, store the result
            IResult result = await commands.ExecuteAsync(new CommandContext(client, message), argPos, map);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result.ErrorReason.ToLower() != "unknown command.")
                {
                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
                }
            }
        }
    }
}