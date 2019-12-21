using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Microsoft.Extensions.Logging;

namespace iTool.DiscordBot
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly ILogger _logger;

        public AudioService(ILogger<AudioService> logger)
        {
            _logger = logger;
        }

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            if (_connectedChannels.ContainsKey(guild.Id)
                || target.Guild.Id != guild.Id)
            {
                return;
            }

            if (_connectedChannels.TryAdd(guild.Id, await target.ConnectAsync()))
            {
                _logger.LogInformation("Connected to voice on {Guild}.", guild.Name);
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (_connectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                _logger.LogInformation("Disconnected from voice on {Guild}.", guild.Name);
            }
        }

        public async Task SendAudioAsync(IGuild guild, string path)
        {
            if (!File.Exists(path))
            {
                _logger.LogInformation("File not found {Path}", path);
                return;
            }

            if (_connectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                _logger.LogInformation($"Starting playback of {path} in {guild.Name}");
                using (Process process = CreateStream(path))
                using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    await process.StandardOutput.BaseStream.CopyToAsync(stream).ConfigureAwait(false);
                    await stream.FlushAsync().ConfigureAwait(false);
                }
            }
        }

        private Process CreateStream(string path)
        {
            string filename = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                ? "ffmpeg.exe" : "ffmpeg";

            return Process.Start(new ProcessStartInfo()
            {
                FileName = filename,
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
