using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace iTool.DiscordBot
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            if (_connectedChannels.TryGetValue(guild.Id, out IAudioClient client)
                || target.Guild.Id != guild.Id)
            {
                return;
            }

            if (_connectedChannels.TryAdd(guild.Id, await target.ConnectAsync()))
            {
                await Logger.Log(new LogMessage(LogSeverity.Info, nameof(AudioService), $"Connected to voice on {guild.Name}."));
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (_connectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                await Logger.Log(new LogMessage(LogSeverity.Info, nameof(AudioService), $"Disconnected from voice on {guild.Name}."));
            }
        }

        public async Task SendAudioAsync(IGuild guild, string path)
        {
            if (!File.Exists(path))
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, nameof(AudioService), $"File not found {path}"));
                return;
            }

            if (_connectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                await Logger.Log(new LogMessage(LogSeverity.Debug, nameof(AudioService), $"Starting playback of {path} in {guild.Name}"));
                Stream output = CreateStream(path).StandardOutput.BaseStream;
                AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music);
                await output.CopyToAsync(stream);
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }

        private Process CreateStream(string path)
        {
            string filename;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filename = "ffmpeg.exe";
            }
            else { filename = "ffmpeg"; }
            
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
