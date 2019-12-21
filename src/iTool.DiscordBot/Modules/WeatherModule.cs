using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenWeather;

namespace iTool.DiscordBot.Modules
{
    public class WeatherModule : ModuleBase
    {
        private readonly OpenWeatherClient _client;
        private readonly Settings _settings;

        public WeatherModule(Settings settings, OpenWeatherClient openweatherClient)
        {
            _settings = settings;
            _client = openweatherClient;
        }

        [Command("weather")]
        [Summary("Returns info about the weather")]
        public async Task GetWeather(string city, string countryCode = null)
        {
            WeatherData weather = await _client.GetWeatherAsync(city, countryCode).ConfigureAwait(false);

            Weather baseweather = weather.Weather[0];

            string temperatureUnit = GetTemperatureUnit(_settings.Units);

            await ReplyAsync(
                "",
                embed: new EmbedBuilder()
                {
                    Title = weather.Name + " " + weather.Sys.Country,
                    Color = _settings.GetColor(),
                    ThumbnailUrl = _client.GetIconURL(baseweather.Icon),
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = "Powered by openweathermap.org",
                    }
                }
                .AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Temperature";
                    f.Value = $"- **Max**: {weather.Main.MaximumTemperature} {temperatureUnit}\n" +
                                $"- **Gem**: {weather.Main.Temperature} {temperatureUnit}\n" +
                                $"- **Min**: {weather.Main.MinimumTemperature} {temperatureUnit}";
                })
                .AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Humidity";
                    f.Value = weather.Main.Humidity + "%";
                })
                .AddField(f =>
                {
                    f.IsInline = true;
                    f.Name = "Wind";
                    f.Value = weather.Wind.Speed + " " + GetSpeedUnit(_settings.Units);
                }).Build()).ConfigureAwait(false);
        }

        private static string GetTemperatureUnit(Unit units)
        {
            return units switch
            {
                Unit.Imperial => "°F",
                Unit.Metric => "°C",
                _ => "kelvin",
            };
        }

        private static string GetSpeedUnit(Unit units)
        {
            return units switch
            {
                Unit.Imperial => "mi/h",
                _ => "m/s",
            };
        }
    }
}
