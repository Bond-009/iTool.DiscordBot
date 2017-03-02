using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace iTool.DiscordBot.Audio
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            IAudioClient audioClient = await target.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                await Program.Log(new LogMessage(LogSeverity.Info, "AudioService", $"Connected to voice on {guild.Name}."));
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                await Program.Log(new LogMessage(LogSeverity.Info, "AudioService", $"Disconnected from voice on {guild.Name}."));
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                await Program.Log(new LogMessage(LogSeverity.Debug, "", $"Starting playback of {path} in {guild.Name}"));
                Stream output = CreateStream(path).StandardOutput.BaseStream;
                await Program.Log(new LogMessage(LogSeverity.Debug, "", "test"));
                AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music, 1920);
                await Program.Log(new LogMessage(LogSeverity.Debug, "", "test2"));
                await output.CopyToAsync(stream);
                await Program.Log(new LogMessage(LogSeverity.Debug, "", "test3"));
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
            
            return Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
