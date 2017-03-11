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
            if (Context.Guild == null) { await ReplyAsync("This command can only be ran in a server."); return; }

            await Program.AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task Stop()
        {
            if (Context.Guild == null) { await ReplyAsync("This command can only be ran in a server."); return; }

            await Program.AudioService.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task Play([Remainder] string song)
        {
            if (Context.Guild == null) { await ReplyAsync("This command can only be ran in a server."); return; }

            string path = AudioManager.GetSong(song);
            if (path == null) { return; }
            await Program.AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
            await Program.AudioService.SendAudioAsync(Context.Guild, Context.Channel, path);
            await Program.AudioService.LeaveAudio(Context.Guild);
        }
    }
}
