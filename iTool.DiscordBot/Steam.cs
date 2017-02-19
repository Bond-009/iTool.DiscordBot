using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace iTool.DiscordBot
{
    // HACK: 
    static class Steam
    {
        public async static Task<UserStatsForGame> GetUserStatsForGame(int gameID, string playername)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid={gameID}&key={Program.Settings.SteamKey}&steamid={await ResolveVanityURL(playername)}&format=xml"))
                {
                    return (UserStatsForGame)new XmlSerializer(typeof(UserStatsForGame)).Deserialize(stream);
                }
            }
        }

        public async static Task<long> ResolveVanityURL(string playername)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={Program.Settings.SteamKey}&vanityurl={playername}&format=xml"))
                {
                    VanityURL vanityurl = (VanityURL)new XmlSerializer(typeof(VanityURL)).Deserialize(stream);

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
