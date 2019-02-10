using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using photoEditor.Extensions;

namespace photoEditor.ImgProcessing
{
	public static class Processor
	{
		public static void ImgRename(string dirPath)
		{
			if (!dirPath.IsDirExists())
			{
				return;
			}

			if (!dirPath.IsFilesExists(out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_rename";
			newDir.CreateEmptyDir();

			Console.WriteLine("Копирование...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				using (Image tmpImg = Image.FromFile(file.FullName))
				{
					string strDateTaken = tmpImg.GetDateTaken(file.LastWriteTime);

					file.CopyTo($@"{newDir}\{strDateTaken}.jpg");
					cntr++;
				}
			}
			Console.WriteLine($"Скопировано файлов: {cntr}");
			return;
		}

		public static void ImgSortByYear(string dirPath)
		{
			if (!dirPath.IsDirExists())
			{
				return;
			}

			if (!dirPath.IsFilesExists(out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_SortedByYear";
			newDir.CreateEmptyDir();

			Console.WriteLine("Сортировка...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				using (Image tmpImg = Image.FromFile(file.FullName))
				{
					string strYearDateTaken = tmpImg.GetDateTaken(file.LastWriteTime).Substring(0, 4);
					string newDirWithYear = $@"{newDir}\{strYearDateTaken}";

					Directory.CreateDirectory(newDirWithYear);
					file.CopyTo($@"{newDirWithYear}\{file.Name}.jpg");
					cntr++;
				}
			}
			Console.WriteLine($"Отсортировано файлов: {cntr}");
			return;
		}

		public static void AddTimeMark(string dirPath)
		{
			if (!dirPath.IsDirExists())
			{
				return;
			}

			if (!dirPath.IsFilesExists(out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_MarkedImg";
			newDir.CreateEmptyDir();

			Console.WriteLine("Изменение...");
			int cntr = 0;
			foreach (var file in fileList)
			{

				using (Image tmpImg = Image.FromFile(file.FullName))
				{
					int _fontSize;

					string strDateTaken = tmpImg.GetDateTaken(file.LastWriteTime);

					//на разных фотках шрифт выглядит по разному, отсюда разделение на то
					//что "меньше" экрана и остальное
					if (tmpImg.Height < 1000)
					{
						_fontSize = tmpImg.Height / 100;
					}
					else
					{
						_fontSize = tmpImg.Height / 30;
					}
					Brush fontColor = Brushes.Crimson;
					Font font = new Font("Calibri", _fontSize);

					int x = 0;
					int y = 0;
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
			if (!dirPath.IsDirExists())
			{
				return;
			}

			if (!dirPath.IsFilesExists(out FileInfo[] fileList))
			{
				return;
			}

			string newDir = $@"{dirPath}_SortedByPlace";
			newDir.CreateEmptyDir();

			Console.WriteLine("Сортировка...");
			int cntr = 0;
			foreach (var file in fileList)
			{
				using (Image tmpImg = Image.FromFile(file.FullName)) 
				{
					const int idLat = 2, idLongit = 4;

					if (tmpImg.PropertyIdList.Contains(idLat) && tmpImg.PropertyIdList.Contains(idLongit))
					{
						string imgLocation = tmpImg.GetGPSPlaceName();

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
	}
}
