using iTool.DiscordBot.Prefrences;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OpenWeatherNet;

namespace iTool.DiscordBot.Modules
{
	public class Weather : ModuleBase
	{
		[Command("weather")]
		[Summary("Gets the weather info")]
		public async Task GetWeather(string input)
		{
			//TODO: Add weather
			OpenWeatherClient client = new OpenWeatherClient(Settings.ApiKeys.OpenWeatherMapKey);
			WeatherInfo weather = await client.GetByCityNameAsync(input);
			weather.temperature.ToCelsius(); //TODO: Add temperaturescale setting

			EmbedBuilder b = new EmbedBuilder()
			{
				Title = weather.city.name + " " + weather.city.country,
				Color = new Color(3, 144, 255),
				ThumbnailUrl = weather.iconLocation
			};
			b.AddField(delegate (EmbedFieldBuilder f)
			{
				f.Name = "Temperature";
				f.Value = $"Max: {weather.temperature.max} {weather.temperature.unit}" + Environment.NewLine +
							$"Gem: {weather.temperature.value} {weather.temperature.unit}" + Environment.NewLine +
							$"Min: {weather.temperature.min} {weather.temperature.unit}";
			});
			await ReplyAsync("", false, b);
		}
	}
}