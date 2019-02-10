using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using GPSCoordinatesToAddress;

namespace photoEditor.Extensions
{
	public static class ImageExtensions
	{
		public static string GetDateTaken(this Image tmpImg, DateTime lastWriteTime)
		{
			const int dateTakenId = 36867;

			if (tmpImg.PropertyIdList.Contains(dateTakenId))
			{
				PropertyItem imgDateTaken = tmpImg.GetPropertyItem(dateTakenId);
				//"2016:03:25 20:05:28\0" -1 это убрать нуль-терминатор
				return Encoding.UTF8
					.GetString(imgDateTaken.Value, 0, imgDateTaken.Len - 1)
					.Replace(':', '_');
			}
			else
			{
				return lastWriteTime.ToString("yyyy_MM_dd HH_mm_ss");
			}
		}

		public static string GetGPSPlaceName(this Image tmpImg)
		{
			const int idLatSign = 1, idLat = 2, idLongitSing = 3, idLongit = 4;

			byte[] latArray = tmpImg.GetPropertyItem(idLat).Value;
			double latitude = Math.Round(latArray.ConvertCoorditate(), 6);
			if (BitConverter.ToChar(tmpImg.GetPropertyItem(idLatSign).Value, 0) == 'S')
			{
				latitude = 0 - latitude;
			}

			byte[] longitArray = tmpImg.GetPropertyItem(idLongit).Value;
			double longitude = Math.Round(longitArray.ConvertCoorditate(), 6);
			if (BitConverter.ToChar(tmpImg.GetPropertyItem(idLongitSing).Value, 0) == 'W')
			{
				longitude = 0 - longitude;
			}

			return GPSProcessing.GetLocation(latitude, longitude);
		}
	}
}
