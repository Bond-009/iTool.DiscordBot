using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Audio : ModuleBase
    {
        AudioService audioService;
        AudioFileService fileService;

        public Audio(AudioService audioService, AudioFileService fileService)
        {
            this.audioService = audioService;
            this.fileService = fileService;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Join()
            => await audioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the audio playback and leaves the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Stop()
            => await audioService.LeaveAudio(Context.Guild);

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Plays an audio file")]
        [RequireContext(ContextType.Guild)]
        public async Task Play([Remainder] string song)
        {
            string path = fileService.GetSong(song);
            if (path == null) { return; }
            await audioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
            await audioService.SendAudioAsync(Context.Guild, path);
            await audioService.LeaveAudio(Context.Guild);
        }
    }
}
