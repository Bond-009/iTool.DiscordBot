using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace iTool.DiscordBot.Modules
{
    [RequireContext(ContextType.Guild)]
    public class AudioModule : ModuleBase
    {
        private readonly AudioService _audioService;
        private readonly AudioFileService _fileService;

        public AudioModule(AudioService audioService, AudioFileService fileService)
        {
            _audioService = audioService;
            _fileService = fileService;
        }

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel")]
        public Task Join()
            => _audioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the audio playback and leaves the voice channel")]
        public Task Stop()
            => _audioService.LeaveAudio(Context.Guild);

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Plays an audio file")]
        public async Task Play([Remainder] string song)
        {
            string path = _fileService.GetSong(song);
            if (path == null)
            {
                return;
            }

            await _audioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel).ConfigureAwait(false);
            await _audioService.SendAudioAsync(Context.Guild, path).ConfigureAwait(false);
            await _audioService.LeaveAudio(Context.Guild).ConfigureAwait(false);
        }
    }
}
