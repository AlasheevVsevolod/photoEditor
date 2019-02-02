using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace photoEditor.Extensions
{
	static class FileInfoExtensions
	{
		public static string GetStrDateTaken(this FileInfo tmpFile)
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
	}
}
