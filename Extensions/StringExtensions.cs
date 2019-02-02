using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace photoEditor.Extensions
{
	static class StringExtensions
	{
		public static bool IsDirExists(this string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Console.WriteLine("Папки не существует");
				return false;
			}
			return true;
		}

		public static bool IsFilesExists(this string dirPath, out FileInfo[] fileList)
		{
			fileList = new DirectoryInfo(dirPath).GetFiles("*.jpg");
			if (fileList.Length == 0)
			{
				Console.WriteLine("Нет файлов для обработки");
				return false;
			}
			return true;
		}

		public static void CreateEmptyDir(this string newDir)
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

		public static string GetTagValue(this string srcString, string tagName)
		{
			int startIndex;
			int stopIndex;

			startIndex = srcString.IndexOf($"<{tagName}>");
			stopIndex = srcString.IndexOf($"</{tagName}>");
			return $@"{srcString.Substring(startIndex + tagName.Length + 2, stopIndex - 2 - startIndex - tagName.Length)}";
		}
	}
}
