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
		public static void ImgRename(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Console.WriteLine("Папки не существует");
				return;
			}

			var fileList = new DirectoryInfo(dirPath).GetFiles("*.jpg");
			if (fileList.Length == 0)
			{
				Console.WriteLine("Нет файлов для обработки");
				return;
			}

			string newDir = $@"{dirPath}_rename";
			int cntr = 0;

			if (Directory.Exists(newDir))
			{
				Directory.Delete(newDir, true);
			}

			//!!!!!Баг - Если после копирования зайти в папку, то на следующей итерации
			//папка удалится, но потом заново не создастся и в file.CopyTo вылетит
			//System.IO.DirectoryNotFoundException
			Directory.CreateDirectory(newDir);

			foreach (var file in fileList)
			{
				using (Image tmpImg = Image.FromFile(file.FullName))
				{
					const int dateTakenId = 36867;
					string strDateTaken;

					if (tmpImg.PropertyIdList.Contains(dateTakenId))
					{
						PropertyItem imgDateTaken = tmpImg.GetPropertyItem(dateTakenId);
						//"2016:03:25 20:05:28\0" -1 это убрать нуль-терминатор
						strDateTaken = Encoding.UTF8
							.GetString(imgDateTaken.Value, 0, imgDateTaken.Len - 1)
							.Replace(':', '_');
					}
					else
					{
						strDateTaken = file.LastWriteTime.ToString("yyyy_MM_dd HH_mm_ss no_prop");
					}

					file.CopyTo($@"{newDir}\{strDateTaken}.jpg");
					cntr++;
				}
			}
			Console.WriteLine($"Скопировано файлов: {cntr}");
			return;
		}
	}
}
