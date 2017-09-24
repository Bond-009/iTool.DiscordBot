using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Serilog;

namespace iTool.DiscordBot
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private ILogger _logger = Log.ForContext<AudioService>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            if (_connectedChannels.TryGetValue(guild.Id, out IAudioClient client)
                || target.Guild.Id != guild.Id)
            {
                return;
            }

            if (_connectedChannels.TryAdd(guild.Id, await target.ConnectAsync()))
            {
                _logger.Information($"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (_connectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                _logger.Information($"Disconnected from voice on {guild.Name}.");
            }
        }

        public async Task SendAudioAsync(IGuild guild, string path)
        {
            if (!File.Exists(path))
            {
                _logger.Error($"File not found {path}");
                return;
            }

            if (_connectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                _logger.Debug($"Starting playback of {path} in {guild.Name}");
                using (Stream output = CreateStream(path).StandardOutput.BaseStream)
                {
                    AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music);
                    await output.CopyToAsync(stream);
                    await stream.FlushAsync().ConfigureAwait(false);
                }
            }
        }

        private Process CreateStream(string path)
            => Process.Start(new ProcessStartInfo()
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg",
                    Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });
    }
}
