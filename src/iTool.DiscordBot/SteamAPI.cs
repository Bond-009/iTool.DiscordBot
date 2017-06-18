using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iTool.DiscordBot.Steam
{
    public class SteamAPI : IDisposable
    {
        private HttpClient _httpClient = new HttpClient();
        private string _key;
        private bool _disposed = false;

        public SteamAPI(string apiKey)
        {
            this._key = apiKey;
            _httpClient.BaseAddress = new Uri("https://api.steampowered.com");
        }

        public async Task<IEnumerable<PlayerBan>> GetPlayerBans(ulong[] steamIDs)
            => JsonConvert.DeserializeObject<PlayerList<PlayerBan>>(
                    await _httpClient.GetStringAsync(
                        $"/ISteamUser/GetPlayerBans/v1/?key={_key}&steamids={string.Join(",", steamIDs)}"
                )).Players;

        public async Task<IEnumerable<PlayerSummary>> GetPlayerSummaries(ulong[] steamIDs)
            => JsonConvert.DeserializeObject<SteamResponse<PlayerList<PlayerSummary>>>(
                    await _httpClient.GetStringAsync(
                        $"/ISteamUser/GetPlayerSummaries/v0002/?key={_key}&steamids={string.Join(",", steamIDs)}"
                    )).Data.Players;

        public async Task<UserStatsForGame> GetUserStatsForGame(int gameID, ulong steamID)
            => JsonConvert.DeserializeObject<UserStatsForGameResponse>(
                    await _httpClient.GetStringAsync(
                        $"/ISteamUserStats/GetUserStatsForGame/v0002/?key={_key}&appid={gameID}&steamid={steamID}"
                    )).Data;

        public async Task<ulong> ResolveVanityURL(string playername)
        {
            VanityURL vanityurl = JsonConvert.DeserializeObject<VanityURlResponse>(
                                    await _httpClient.GetStringAsync(
                                        $"/ISteamUser/ResolveVanityURL/v0001/?key={_key}&vanityurl={playername}"
                                    )).Data;

            switch (vanityurl.Success)
            {
                case 1:
                    return vanityurl.SteamID64.Value;
                case 42:
                    throw new Exception("No player found.");
                default:
                    throw new Exception();
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }

            _key = null;

            _disposed = true;
        }
    }
}
