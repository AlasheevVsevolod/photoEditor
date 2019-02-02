using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

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
					return tmpFile.LastWriteTime.ToString("yyyy_MM_dd HH_mm_ss no_prop");
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
	}
}
