using Discord;
using Discord.Commands;
using OpenWeatherNet;
using System;
using System.Threading.Tasks;

namespace iTool.DiscordBot.Modules
{
    public class Weather : ModuleBase
    {
        [Command("weather")]
        [Summary("Gets the weather info")]
        public async Task GetWeather(string input)
        {
            if (!string.IsNullOrEmpty(Program.settings.OpenWeatherMapKey))
            {
                //TODO: Add weather
                OpenWeatherClient client = new OpenWeatherClient(Program.settings.OpenWeatherMapKey);
                WeatherInfo weather = await client.GetCurrentAsync(input);
                weather.Temperature = weather.Temperature.ToCelsius(); //TODO: Add temperaturescale setting

                EmbedBuilder b = new EmbedBuilder()
                {
                    Title = weather.City.Name + " " + weather.City.Country,
                    Color = new Color(3, 144, 255),
                    ThumbnailUrl = weather.IconURL
                };
                b.AddField(delegate (EmbedFieldBuilder f)
                {
                    f.Name = "Temperature";
                    f.Value = $"Max: {weather.Temperature.Max} {weather.Temperature.Unit}" + Environment.NewLine +
                                $"Gem: {weather.Temperature.Value} {weather.Temperature.Unit}" + Environment.NewLine +
                                $"Min: {weather.Temperature.Min} {weather.Temperature.Unit}";
                });
                await ReplyAsync("", embed: b);
            }
        }
    }
}