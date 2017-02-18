using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    // HACK: 
    static class Steam
    {
        public async static Task<UserStatsForGame> GetStats(int gameID, string playername)
        {
            if (string.IsNullOrEmpty(playername)) { throw new ArgumentNullException(); }

            if (Program.Settings.SteamKey == null)
            {
                throw new Exception("No SteamKey");
            }

            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid={gameID}&key={Program.Settings.SteamKey}&steamid={await ResolveVanityURL(playername)}&format=xml"))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(UserStatsForGame));
                    return (UserStatsForGame)ser.Deserialize(stream);
                }
            }
        }

        public async static Task<long> ResolveVanityURL(string playername)
        {
            if (Program.Settings.SteamKey == null)
            {
                throw new Exception("No SteamKey");
            }

            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={Program.Settings.SteamKey}&vanityurl={playername}&format=xml"))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(VanityURL));
                    VanityURL vanityurl = (VanityURL)ser.Deserialize(stream);

                    switch (vanityurl.Succes)
                    {
                        case 1:
                            return vanityurl.SteamID64.Value;
                        case 42:
                            throw new Exception("No player found.");
                        default:
                            throw new Exception();
                    }
                }
            }
        }
    }
}
