using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Serilog;

namespace iTool.DiscordBot.Modules
{
    public class AudioModule : ModuleBase
    {
        private readonly AudioService _audioService;
        private readonly AudioFileService _fileService;

        public AudioModule(AudioService audioService, AudioFileService fileService, ILogger logger)
        {
            _audioService = audioService;
            _fileService = fileService;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Join()
            => await _audioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);

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
            await _audioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
            await _audioService.SendAudioAsync(Context.Guild, path);
            await _audioService.LeaveAudio(Context.Guild);
        }
    }
}
