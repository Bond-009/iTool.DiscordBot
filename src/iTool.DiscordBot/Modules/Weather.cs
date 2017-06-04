using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenWeather;

namespace iTool.DiscordBot.Modules
{
    public class Weather : ModuleBase
    {
        private static OpenWeatherClient _client;
        private Settings _settings;

        public Weather(Settings settings)
            => _settings = settings;

        protected override void BeforeExecute()
        {
            if (string.IsNullOrEmpty(_settings.OpenWeatherMapKey))
            {
                throw new Exception("No OpenWeatherMapKey found.");
            }
            if (_client == null)
            { _client = new OpenWeatherClient(_settings.OpenWeatherMapKey, _settings.Units); }
        }

        [Command("weather")]
        [Summary("Returns info about the weather")]
        public async Task GetWeather(string city, string countryCode = null)
        {
            WeatherData weather = await _client.GetWeatherAsync(city, countryCode);

            await ReplyAsync("", embed: new EmbedBuilder()
            {
                Title = weather.Name + " " + weather.Sys.Country,
                Color = _settings.GetColor(),
                ThumbnailUrl = _client.GetIconURL(weather.Weather.FirstOrDefault()?.Icon),
                Footer = new EmbedFooterBuilder()
                    {
                        Text = "Powered by openweathermap.org",
                    }
            }
            .AddField(f =>
            {
                f.IsInline = true;
                f.Name = "Temperature";
                f.Value = $"- **Max**: {weather.Main.MaximumTemperature} {GetTemperatureUnit(_settings.Units)}\n" +
                            $"- **Gem**: {weather.Main.Temperature} {GetTemperatureUnit(_settings.Units)}\n" +
                            $"- **Min**: {weather.Main.MinimumTemperature} {GetTemperatureUnit(_settings.Units)}";
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
            }));
        }

        private static string GetTemperatureUnit(Unit units)
        {
            switch(units)
            {
                case Unit.Imperial:
                    return "°F";
                case Unit.Metric:
                    return "°C";
                case Unit.Standard:
                default:
                    return "kelvin";
            }
        }

        private string GetSpeedUnit(Unit units)
        {
            switch(units)
            {
                case Unit.Imperial:
                    return "mi/h";
                case Unit.Metric:
                case Unit.Standard:
                default:
                    return "m/s";
            }
        }
    }
}
