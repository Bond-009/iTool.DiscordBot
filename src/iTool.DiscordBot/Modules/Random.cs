using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace iTool.DiscordBot.Modules
{
    public class Random : ModuleBase
    {
        private Settings _settings;

        public Random(Settings settings) => _settings = settings;

        [Command("random")]
        [Summary("Returns a random number between")]
        public async Task Rand(int num1, int num2)
        {
            if (num1 > num2) return;

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = "Random",
                Color = _settings.GetColor(),
                Description = new System.Random().Next(num1, num2).ToString()
            });
        }

        [Command("cat")]
        [Summary("Returns a random cat image")]
        public async Task Cat()
        {
            using (HttpClient httpclient = new HttpClient())
            {
                JObject o = JObject.Parse(await httpclient.GetStringAsync("http://random.cat/meow"));

                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Cat",
                    Color = _settings.GetColor(),
                    ImageUrl = (string)o["file"]
                });
            }
        }

        [Command("dog")]
        [Summary("Returns a random dog image")]
        public async Task Dog()
        {
            using (HttpClient httpclient = new HttpClient())
            {
                string link;
                while (true)
                {
                    link = "http://random.dog/" + (await httpclient.GetStringAsync("http://random.dog/woof")).ToLower();
                    if (link.EndsWith(".jpg")) { break; }
                }

                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Title = "Dog",
                    Color = _settings.GetColor(),
                    ImageUrl = link
                });
            }
        }
    }
}
