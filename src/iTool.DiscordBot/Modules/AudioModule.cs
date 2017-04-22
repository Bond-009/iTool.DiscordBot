using Discord;
using Discord.Commands;
using iTool.DiscordBot.Audio;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class AudioModule : ModuleBase
    {
        AudioService AudioService;

        public AudioModule(AudioService audioService) => this.AudioService = audioService;

        [Command("join", RunMode = RunMode.Async)]
        [Summary("Joins the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Join()
            => await AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);

        [Command("stop", RunMode = RunMode.Async)]
        [Summary("Stops the audio playback and leaves the voice channel")]
        [RequireContext(ContextType.Guild)]
        public async Task Stop()
            => await AudioService.LeaveAudio(Context.Guild);

        [Command("play", RunMode = RunMode.Async)]
        [Summary("Plays an audio files")]
        [RequireContext(ContextType.Guild)]
        public async Task Play([Remainder] string song)
        {
            string path = AudioManager.GetSong(song);
            if (path == null) { return; }
            await AudioService.JoinAudio(Context.Guild, (Context.User as IGuildUser).VoiceChannel);
            await AudioService.SendAudioAsync(Context.Guild, path);
            await AudioService.LeaveAudio(Context.Guild);
        }
    }
}
