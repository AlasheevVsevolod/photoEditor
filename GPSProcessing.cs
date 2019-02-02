using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using photoEditor.Extensions;

namespace photoEditor
{
	static class GPSProcessing
	{
		public static string GetLocation(double lat, double longit)
		{
			string responseFromServer;

			WebRequest newRequset = WebRequest.Create("https://geocode-maps.yandex.ru/1.x/?geocode=" +
				$"{longit},{lat}" + "&kind=locality" + "&lang=en_US");

			using (HttpWebResponse response = (HttpWebResponse)newRequset.GetResponse())
			{
				using (Stream dataStream = response.GetResponseStream())
				{
					// Open the stream using a StreamReader for easy access.
					using (StreamReader reader = new StreamReader(dataStream))
					{
						// Read the content.
						responseFromServer = reader.ReadToEnd();
					}
				}
			}

			string locationMarker;

			locationMarker = "CountryName";
			string fullLocation = responseFromServer.GetTagValue(locationMarker);

			locationMarker = "AdministrativeAreaName";
			fullLocation += $"__{responseFromServer.GetTagValue(locationMarker)}";

			locationMarker = "SubAdministrativeAreaName";
			fullLocation += $"__{responseFromServer.GetTagValue(locationMarker)}";

			locationMarker = "LocalityName";
			fullLocation += $"__{responseFromServer.GetTagValue(locationMarker)}";

			return fullLocation;
		}
	}
}
