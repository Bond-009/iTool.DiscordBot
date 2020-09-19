using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace iTool.DiscordBot
{
    public class RequireGuildSpecificSettingsAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService<Settings>().GuildSpecificSettings)
            {
                return PreconditionResult.FromSuccess();
            }
            await serviceProvider.GetService<CommandService>().RemoveModuleAsync(command.Module).ConfigureAwait(false);
            return PreconditionResult.FromError("GuildSpecificSettings is disabled.");
        }
    }
}
