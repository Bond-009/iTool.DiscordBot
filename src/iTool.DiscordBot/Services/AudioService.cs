using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace iTool.DiscordBot
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public AudioService() => Directory.CreateDirectory(Common.TempDir);

        public async Task JoinAudio(IVoiceChannel voiceChannel)
        {
            if (_connectedChannels.TryGetValue(voiceChannel.Guild.Id, out IAudioClient client))
            {
                return;
            }

            if (_connectedChannels.TryAdd(voiceChannel.Guild.Id, await voiceChannel.ConnectAsync()))
            {
                await Logger.Log(new LogMessage(LogSeverity.Info, nameof(AudioService), $"Connected to voice on {voiceChannel.Guild.Name}."));
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

                using (Stream output = CreateStream(path).StandardOutput.BaseStream)
                using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    await output.CopyToAsync(stream);
                }
            }
        }

        public async Task SendYTVideoAsync(IGuild guild, string videoID)
        {
            if (videoID.Length != 11) return;

            YoutubeClient ytClient = new YoutubeClient();

            VideoInfo info = await ytClient.GetVideoInfoAsync(videoID);

            AudioStreamInfo streamInfo = info.AudioStreams.OrderBy(x => x.Bitrate <= 128 * 1024).Last(); // .OrderBy(x => x.Bitrate).Last();

            if (_connectedChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                await Logger.Log(new LogMessage(LogSeverity.Debug, nameof(AudioService), $"Starting playback of {streamInfo.Url} in {guild.Name}"));

                //using (MediaStream output = (await ytClient.GetMediaStreamAsync(streamInfo)))
                //using (AudioOutStream stream = client.CreateOpusStream())
                using (Stream output = CreateStreamFromUrl(streamInfo.Url).StandardOutput.BaseStream)
                using (AudioOutStream stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    await output.CopyToAsync(stream);
                }
            }
            /*
            Regex RGX = new Regex("[^a-zA-Z0-9 -]");
            string Title = RGX.Replace(info.Title, "");

            string path = Common.TempDir + Path.DirectorySeparatorChar + $"{guild.Id}{Title}.{streamInfo.Container.GetFileExtension()}";

            try
            {
                using (var input = await client.GetMediaStreamAsync(streamInfo))
                using (var file = File.Create(path))
                {
                    await input.CopyToAsync(file);
                }
                await SendAudioAsync(guild, path);
            }
            finally
            {
                File.Delete(path);
            }
            */
        }

        private Process CreateStreamFromUrl(string path)
            => Process.Start(new ProcessStartInfo()
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

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
