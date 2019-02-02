using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
namespace photoEditor.ImgProcessing
{
	public static class Processor
	{
		public static string GetStrDateTaken(FileInfo tmpFile)
		{ 
			using (Image tmpImg = Image.FromFile(tmpFile.FullName))
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
					return tmpFile.LastWriteTime.ToString("yyyy_MM_dd HH_mm_ss");
				}
			}
		}

		public static bool IsDirExists(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Console.WriteLine("Папки не существует");
				return false;
			}
			return true;
		}

		public static bool IsFilesExists(string dirPath, out FileInfo[] fileList)
		{
			fileList = new DirectoryInfo(dirPath).GetFiles("*.jpg");
			if (fileList.Length == 0)
			{
				Console.WriteLine("Нет файлов для обработки");
				return false;
			}
			return true;
		}

		public static void CreateEmptyDir(string newDir)
		{
			if (Directory.Exists(newDir))
			{
				Directory.Delete(newDir, true);
			}

			//!!!!!Баг - Если после копирования зайти в папку, то на следующей итерации
			//папка удалится, но потом заново не создастся и в file.CopyTo вылетит
			//System.IO.DirectoryNotFoundException
			Directory.CreateDirectory(newDir);
		}

		public static void ImgRename(string dirPath)
		{
			if (!IsDirExists(dirPath))
			{
				return;
			}

			if (!IsFilesExists(dirPath, out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_rename";
			CreateEmptyDir(newDir);

			Console.WriteLine("Копирование...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				string strDateTaken = GetStrDateTaken(file);

				file.CopyTo($@"{newDir}\{strDateTaken}.jpg");
				cntr++;
			}
			Console.WriteLine($"Скопировано файлов: {cntr}");
			return;
		}

		public static void ImgSortByYear(string dirPath)
		{
			if (!IsDirExists(dirPath))
			{
				return;
			}

			if (!IsFilesExists(dirPath, out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_SortedByYear";
			CreateEmptyDir(newDir);

			Console.WriteLine("Сортировка...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				string strYearDateTaken = GetStrDateTaken(file).Substring(0, 4);
				string newDirWithYear = $@"{newDir}\{strYearDateTaken}";

				Directory.CreateDirectory(newDirWithYear);
				file.CopyTo($@"{newDirWithYear}\{file.Name}.jpg");
				cntr++;
			}
			Console.WriteLine($"Отсортировано файлов: {cntr}");
			return;
		}

		public static void AddTimeMark(string dirPath)
		{
			if (!IsDirExists(dirPath))
			{
				return;
			}

			if (!IsFilesExists(dirPath, out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_MarkedImg";
			CreateEmptyDir(newDir);

			Console.WriteLine("Изменение...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				string strDateTaken = GetStrDateTaken(file);

				int _fontSize = 100;
				Brush fontColor = Brushes.Crimson;
				Font font = new Font("Calibri", _fontSize);

				using (Image tmpImg = Image.FromFile(file.FullName))
				{
					int x = 100;
					int y = 100;

					Graphics tmpG = Graphics.FromImage(tmpImg);

					tmpG.DrawString(strDateTaken, font, fontColor, x, y);
					tmpImg.Save($@"{newDir}\{file.Name}.jpg");
				}
				cntr++;
			}
			Console.WriteLine($"Изменено файлов: {cntr}");
			return;
		}

		public static void ImgSortByPlace(string dirPath)
		{
			if (!IsDirExists(dirPath))
			{
				return;
			}

			if (!IsFilesExists(dirPath, out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_SortedByPlace";
			CreateEmptyDir(newDir);

			Console.WriteLine("Сортировка...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				using (Image tmpImg = Image.FromFile(file.FullName)) 
				{
					const int idLatSign = 1, idLat = 2, idLongtSing = 3, idLongt = 4;

					if (tmpImg.PropertyIdList.Contains(idLat) && tmpImg.PropertyIdList.Contains(idLongt))
					{
						byte[] latArray = tmpImg.GetPropertyItem(idLat).Value;
						double latitude = Math.Round(ConvertCoorditate(latArray),6);
						if (BitConverter.ToChar(tmpImg.GetPropertyItem(idLatSign).Value, 0) == 'S')
						{
							latitude = 0 - latitude;
						}

						byte[] longtArray = tmpImg.GetPropertyItem(idLongt).Value;
						double longtitude = Math.Round(ConvertCoorditate(longtArray),6);
						if (BitConverter.ToChar(tmpImg.GetPropertyItem(idLongtSing).Value, 0) == 'W')
						{
							longtitude = 0 - longtitude;
						}

						string imgLocation = GetLocation(latitude, longtitude);

						string newDirWithLocation = $@"{newDir}\{imgLocation}";
						Directory.CreateDirectory(newDirWithLocation);

						file.CopyTo($@"{newDirWithLocation}\{file.Name}.jpg");
						cntr++;
					}
				}

			}
			Console.WriteLine($"Отсортировано файлов: {cntr}");
			return;
		}

		public static double ConvertCoorditate(byte[] tmpArr)
		{
			double degree = GetDoubleFromByteArray(tmpArr, 0);
			double minute = GetDoubleFromByteArray(tmpArr, 8);
			double sec = GetDoubleFromByteArray(tmpArr, 16);

			return degree + (minute * 60 + sec) / 3600;
		}

		public static double GetDoubleFromByteArray(byte[] tmpArr, int index)
		{ 
			return (double)BitConverter.ToInt32(tmpArr, index) / BitConverter.ToInt32(tmpArr, index + 4);
		}


		public static string GetLocation(double lat, double longt)
		{
			WebRequest newRequset = WebRequest.Create("https://geocode-maps.yandex.ru/1.x/?geocode=" +
				$"{longt},{lat}" + "&kind=locality" + "&lang=en_US");

			HttpWebResponse response = (HttpWebResponse)newRequset.GetResponse();
			Stream dataStream = response.GetResponseStream();

			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);

			// Read the content.
			string responseFromServer = reader.ReadToEnd();

			int startIndex;
			int stopIndex;
			string locationMarker;

			locationMarker = "<CountryName>";
			startIndex = responseFromServer.IndexOf(locationMarker);
			stopIndex = responseFromServer.IndexOf("</CountryName>");
			string fullLocation = $@"{responseFromServer.Substring(startIndex + locationMarker.Length, stopIndex - startIndex - locationMarker.Length)}";

			locationMarker = "<AdministrativeAreaName>";
			startIndex = responseFromServer.IndexOf(locationMarker);
			stopIndex = responseFromServer.IndexOf("</AdministrativeAreaName>");
			fullLocation += $@"__{responseFromServer.Substring(startIndex + locationMarker.Length, stopIndex - startIndex - locationMarker.Length)}";

			locationMarker = "<SubAdministrativeAreaName>";
			startIndex = responseFromServer.IndexOf(locationMarker);
			stopIndex = responseFromServer.IndexOf("</SubAdministrativeAreaName>");
			fullLocation += $@"__{responseFromServer.Substring(startIndex + locationMarker.Length, stopIndex - startIndex - locationMarker.Length)}";

			locationMarker = "<LocalityName>";
			startIndex = responseFromServer.IndexOf(locationMarker);
			stopIndex = responseFromServer.IndexOf("</LocalityName>");
			fullLocation += $@"__{responseFromServer.Substring(startIndex + locationMarker.Length, stopIndex - startIndex - locationMarker.Length)}";

			return fullLocation;
		}
	}
}
