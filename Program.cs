using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using photoEditor.ImgProcessing;

namespace photoEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			bool _programIsRunning = true, isCmdParameterExists = false;
			string consoleString, command, parameter;

			Console.WriteLine("Программа редоктирования изображений");
			Console.WriteLine("Поддерживаемые комманды:");
			PrintHelpInfo();

			while (_programIsRunning)
			{
				Console.Write("Введите комманду: ");
				consoleString = Console.ReadLine().ToLower();
				isCmdParameterExists = IsCmdParam(consoleString, out command, out parameter);

				switch (command)
				{
					case "a":
					case "addtimemark":
						//addtimemark
						if (!isCmdParameterExists)
						{
							Console.WriteLine("Addtimemark [path]");
						}

						Processor.AddTimeMark(parameter);
						break;

					case "h":
					case "help":
						PrintHelpInfo();
						break;

					case "q":
					case "quit":
						_programIsRunning = false;
						Console.WriteLine("Выход...");
						break;

					case "r":
					case "rename":
						//rename
						if (!isCmdParameterExists)
						{
							Console.WriteLine("Rename [path]");
						}

						Processor.ImgRename(parameter);
						break;

					case "sy":
					case "sortbyyear":
						//sortbyyear
						if (!isCmdParameterExists)
						{
							Console.WriteLine("SortbyYear [path]");
						}

						Processor.ImgSortByYear(parameter);
						break;

					case "sp":
					case "sortbyplace":
						//sortbyplace
						break;

					default:
						break;
				}
			}
		}

		private static void PrintHelpInfo()
		{
			Console.WriteLine("Addtimemark [path] - добавление на изображения отметки, когда фото было сделано");
			Console.WriteLine("Help - вывод доступных команд");
			Console.WriteLine("Quit - вывход из программы");
			Console.WriteLine("Rename [path] - переименование изображений в соответствии с датой сьемки");
			Console.WriteLine("SortbyYear [path] - сортировка изображений по папкам по годам");
			Console.WriteLine("SortbyPlace [path] - сортировка изображений по папкам по месту сьемки\n");
		}

		public static bool IsCmdParam(string consoleString, out string consoleCommand, out string cmdParameter)
		{
			cmdParameter = null;

			if (consoleString.Contains(" "))
			{
				consoleCommand = consoleString.Substring(0, consoleString.IndexOf(" "));
				cmdParameter = consoleString.Substring(consoleString.IndexOf(" ") + 1);
				return true;
			}
			else
			{
				consoleCommand = consoleString;
				return false;
			}
		}
	}
}
