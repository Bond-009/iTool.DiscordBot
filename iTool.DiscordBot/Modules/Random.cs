﻿using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace iTool.DiscordBot.Modules
{
    public class Random : ModuleBase
    {
        [Command("cat")]
        [Summary("Gets a random cat image")]
        public async Task Cat()
        {
            XDocument xDoc = JsonConvert.DeserializeXNode(await (new HttpClient().GetStringAsync("http://random.cat/meow")), "root");
            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Color = new Color(3, 144, 255),
                ImageUrl = xDoc.Element("root").Element("file").Value
            });
        }
    }
}