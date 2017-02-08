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
                    Color = new Color(3, 144, 255),
                    ImageUrl = (string)o["file"]
                });
            }
        }
    }
}
