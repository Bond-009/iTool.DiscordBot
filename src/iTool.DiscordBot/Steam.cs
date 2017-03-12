using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace iTool.DiscordBot.Steam
{
    // HACK: 
    static class Steam
    {
        public async static Task<PlayerBans> GetPlayerBans(ulong[] steamIDs)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={Program.Settings.SteamKey}&steamids={string.Join(",", steamIDs)}&format=xml"))
                {
                    return (PlayerBans)new XmlSerializer(typeof(PlayerBans)).Deserialize(stream);
                }
            }
        }

        public async static Task<PlayerSummaries> GetPlayerSummaries(ulong[] steamIDs)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={Program.Settings.SteamKey}&steamids={string.Join(",", steamIDs)}&format=xml"))
                {
                    return (PlayerSummaries)new XmlSerializer(typeof(PlayerSummaries)).Deserialize(stream);
                }
            }
        }

        public async static Task<UserStatsForGame> GetUserStatsForGame(int gameID, ulong steamID)
        {
            using (HttpClient httpclient = new HttpClient())
            {
                using (Stream stream = await httpclient.GetStreamAsync(
                    $"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?key={Program.Settings.SteamKey}&appid={gameID}&steamid={steamID}&format=xml"))
                {
                    return (UserStatsForGame)new XmlSerializer(typeof(UserStatsForGame)).Deserialize(stream);
                }
            }
        }

        public async static Task<ulong> ResolveVanityURL(string playername)
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
