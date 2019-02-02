using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace photoEditor.Extensions
{
	static class ByteArrayExtensions
	{
		public static double ConvertCoorditate(this byte[] tmpArr)
		{
			double degree = GetDoubleFromByteArray(tmpArr, 0);
			double minute = GetDoubleFromByteArray(tmpArr, 8);
			double sec = GetDoubleFromByteArray(tmpArr, 16);

			return degree + (minute * 60 + sec) / 3600;
		}

		public static double GetDoubleFromByteArray(this byte[] tmpArr, int index)
		{
			return (double)BitConverter.ToInt32(tmpArr, index) / BitConverter.ToInt32(tmpArr, index + 4);
		}
	}
}
