using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Reflection;

namespace iTool.DiscordBot
{
    public class CommandHandler
    {
        public CommandService CommandService { get; private set; }

        private DiscordSocketClient client;
        private IDependencyMap map;

        public async Task Install(IDependencyMap _map)
        {
            // Create Command Service, inject it into Dependency Map
            client = _map.Get<DiscordSocketClient>();
            CommandService = new CommandService();
            _map.Add(CommandService);
            map = _map;

            await CommandService.AddModulesAsync(Assembly.GetEntryAssembly());

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
            if (!(message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.HasStringPrefix(Program.Settings.Prefix, ref argPos))) return;

            // Execute the Command, store the result
            IResult result = await CommandService.ExecuteAsync(new CommandContext(client, message), argPos, map);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (result.ErrorReason.ToLower() != "unknown command.")
                {
                    await Program.Log(new LogMessage(LogSeverity.Error, "", result.ErrorReason));

                    await message.Channel.SendMessageAsync("", embed: new EmbedBuilder()
                    {
                        Title = "Error",
                        Color = new Color(204, 0, 0),
                        Description = result.ErrorReason
                    });
                }
            }
        }
    }
}
