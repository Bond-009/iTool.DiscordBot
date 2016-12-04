using System.Threading.Tasks;
using Discord.Commands;
using OpenWeatherNet;

namespace iTool.DiscordBot.Modules
{
	public class Weather : ModuleBase
	{
		[Command("weather")]
		[Summary("Gets the weather info")]
		public async Task GetWeather([Remainder] string input)
		{
			//TODO: Add weather
			//OpenWeatherClient client = new OpenWeatherClient();
		}
	}
}