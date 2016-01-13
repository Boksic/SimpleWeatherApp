using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AppExpert1
{
	public class Core
	{
		public static string queryString;

		public static async Task<Weather> GetWeather(string stringName)
		{
			dynamic results = await DataService.getDataFromService (queryString).ConfigureAwait (false);

			dynamic weatherOverview = results ["query"] ["results"] ["channel"];

			if ((string)weatherOverview ["title"] != "Yahoo! Weather - Error")
			{
				Weather weather = new Weather ();

				weather.Title = (string)weatherOverview ["description"];

				dynamic wind = weatherOverview ["wind"];
				weather.Temperature = (string)wind ["chill"];
				weather.Wind = (string)wind ["speed"];

				dynamic atmosphere = weatherOverview ["atmosphere"];
				weather.Humidity = (string)atmosphere ["humidity"];
				weather.Visibility = (string)atmosphere ["visibility"];

				dynamic sun = weatherOverview ["astronomy"];
				weather.Sunrise = (string)sun ["sunrise"];
				weather.Sunset = (string)sun ["sunset"];

				var regex = new Regex (@"src=""(?<src>.*?)""");

				var match = regex.Match ((string) weatherOverview["item"]["description"]);

				weather.ImagePath = match.Groups ["src"].Value;

				dynamic ville = weatherOverview ["location"];
				weather.VilleName = (string)ville ["city"];

				return weather;
			}
			else 
			{
				return null;
			}
		}

		public static async Task<Weather> GetWeatherByPosition()
		{
			dynamic results = await DataService.getDataFromService (queryString).ConfigureAwait (false);

			dynamic weatherOverview = results ["query"] ["results"];

			Weather weather = new Weather ();

			dynamic Result = weatherOverview ["Result"];
			weather.Line2 = (string)Result ["line2"];

			return weather;
		}
	}
}