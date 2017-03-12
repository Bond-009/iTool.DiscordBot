using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Random : ModuleBase
    {
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
                    Color = new Color(3, 144, 255),
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
                    Color = new Color(3, 144, 255),
                    ImageUrl =  link
                });
            }
        }
    }
}
