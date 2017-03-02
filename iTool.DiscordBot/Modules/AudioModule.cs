using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task Join()
        {
            await Program.AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            await Program.AudioService.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string song)
        {
            await Program.AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
            await Program.AudioService.SendAudioAsync(Context.Guild, Context.Channel, song);
        }
    }
}