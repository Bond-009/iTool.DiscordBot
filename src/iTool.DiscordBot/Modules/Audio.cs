using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    public class Audio : ModuleBase
    {
        private AudioService _audioService;
        private AudioFileService _fileService;

        public Audio(AudioService audioService, AudioFileService fileService)
        {
            _audioService = audioService;
            _fileService = fileService;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Join()
            => await _audioService.JoinAudio(Context.Guild, ((IGuildUser)Context.User).VoiceChannel);

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the audio playback and leaves the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Stop()
            => await _audioService.LeaveAudio(Context.Guild);

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Plays an audio file")]
        [RequireContext(ContextType.Guild)]
        public async Task Play([Remainder] string song)
        {
            string path = _fileService.GetSong(song);
            if (path == null) { return; }
            await _audioService.JoinAudio(Context.Guild, ((IGuildUser)Context.User).VoiceChannel);
            await _audioService.SendAudioAsync(Context.Guild, path);
            await _audioService.LeaveAudio(Context.Guild);
        }
    }
}
