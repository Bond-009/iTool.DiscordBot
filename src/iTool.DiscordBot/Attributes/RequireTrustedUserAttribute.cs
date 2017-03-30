using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace iTool.DiscordBot
{
    public class RequireTrustedUserAttribute : PreconditionAttribute
    {
        // Override the CheckPermissions method
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (map.Get<Settings>().TrustedUsers.Contains(context.User.Id)
                || context.User.Id == (await map.Get<DiscordSocketClient>().GetApplicationInfoAsync()).Owner.Id)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("You must be a trusted user or the owner of the bot to run this command.");
            }
        }
    }
}
