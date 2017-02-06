using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;

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
                XDocument xDoc = JsonConvert.DeserializeXNode(await httpclient.GetStringAsync("http://random.cat/meow"), "root");

                await ReplyAsync("", embed: new EmbedBuilder()
                {
                    Color = new Color(3, 144, 255),
                    ImageUrl = xDoc.Element("root").Element("file").Value
                });
            }
        }
    }
}
