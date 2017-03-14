using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Join() =>
            await Program.AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the audio blayback and leaves the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Stop() =>
            await Program.AudioService.LeaveAudio(Context.Guild);

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Plays an audio files")]
        [RequireContext(ContextType.Guild)]
        public async Task Play([Remainder] string song)
        {
            string path = AudioManager.GetSong(song);
            if (path == null) { return; }
            await Program.AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
            await Program.AudioService.SendAudioAsync(Context.Guild, path);
            await Program.AudioService.LeaveAudio(Context.Guild);
        }
    }
}
