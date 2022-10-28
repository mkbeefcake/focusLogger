using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;

namespace test1
{
	public partial class Form1 : Form
	{
		System.Timers.Timer _Timer;
		DataItem oldActiveItem = new DataItem();
		DataItem oldForegroundItem = new DataItem();

		//CsvReader _Reader;

		StreamWriter _systemWriter;
		CsvWriter _Writer;

		DateTime loggingStartTime;

		string logfilePath;
		string logfileName;
		int rotateInterval;

		public Form1()
		{
			InitializeComponent();
		}

		void InitTimer()
		{
			_Timer = new System.Timers.Timer();
			_Timer.Elapsed += _Timer_Elapsed;
			_Timer.AutoReset = false;
			_Timer.Interval = 1;
			_Timer.Start();

			UpdateLoggingStartTime();
		}

		private void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			TimeSpan span = DateTime.Now.Subtract(loggingStartTime);
			if (span.TotalMinutes >= rotateInterval)
            {
				UpdateLoggingStartTime();
				CreateCSVFile(GenerateLogFilePath());
            }

			_Timer.Start();
			UpdateInfo();
		}

		object AddLock = new object();

		public void UpdateFromProcess(DataItem item)
		{
			using (var process = Process.GetProcessById(item.ProcessId))
			{
				item.ProcessName = process.ProcessName;
				if (item.ProcessId > 0)
				{
					item.ProcessPath = process.MainModule?.FileName;
				}
			}
		}

		public void UpdateInfo()
		{
			// Active window -  Window that appears in the foreground with a highlighted title bar.
			// Foreground window - Window with which the user is currently working.
			//   The system assigns a slightly higher priority to the thread used to create the foreground window.
			// Focus window - Window that is currently receiving keyboard input.
			//   The focus window is always the active window, a descendent of the active window, or NULL.
			// Top-Level window -  A window that has no parent window.
			//
			lock (AddLock)
			{
				// Get window which or child window of which receives keyboard input.
				//           var activeHandle = NativeMethods.GetActiveWindow();
				//           var activeItem = GetItemFromHandle(activeHandle, true);
				//           if (!activeItem.IsSame(oldActiveItem) && (activeItem.ProcessId != 0))
				//           {
				//               oldActiveItem = activeItem;
				//               UpdateFromProcess(activeItem);
				//if (activeItem.ProcessId != 0)
				//	WriteCSVRecord(activeItem);
				//           }

				// Get foreground window.
				var foregroundHandle = NativeMethods.GetForegroundWindow();
				var foregroundItem = GetItemFromHandle(foregroundHandle);
				if (!foregroundItem.IsSame(oldForegroundItem))
				{
					// oldForegroundItem = foregroundItem;
					UpdateFromProcess(foregroundItem);
					if (foregroundItem.ProcessId != 0)
					{
						WriteCSVRecord(foregroundItem);
						oldForegroundItem = foregroundItem;
					}
				}
			}
		}

		void WriteCSVRecord(DataItem item)
		{
			if (_Writer == null)
				return;

			_Writer.WriteRecord(item.Convert());
			_Writer.NextRecord();
			_Writer.Flush();
		}

		DataItem GetItemFromHandle(IntPtr hWnd, bool isActive = false)
		{
			var item = new DataItem();
			item.Date = DateTime.Now;
			item.IsActive = isActive;
			var info = NativeMethods.GetInfo(hWnd);
			if (info.HasValue)
			{
				item.HasMouse = info.Value.hwndCapture != IntPtr.Zero;
				item.HasKeyboard = info.Value.hwndFocus != IntPtr.Zero;
				item.HasCaret = info.Value.hwndCaret != IntPtr.Zero;
			}
			int processId;
			if (isActive)
			{
				hWnd = NativeMethods.GetTopWindow(hWnd);
			}
			item.WindowTitle = NativeMethods.GetWindowText(hWnd);
			NativeMethods.GetWindowThreadProcessId(hWnd, out processId);
			item.ProcessId = processId;
			return item;
		}

		void CreateCSVFile(string logFullPath)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(logFullPath));

			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				NewLine = Environment.NewLine,
				Delimiter = ";"
			};

			var utf8NoBom = new UTF8Encoding(false);
			FileStream stream = new FileStream(logFullPath, FileMode.Create);

			_systemWriter = new StreamWriter(stream, utf8NoBom);

			_Writer = new CsvWriter(_systemWriter, config);

		}

		public void SetLogInfo(string logfilePath, string logfileName, int rotateInterval)
        {
			this.logfilePath = logfilePath;
			this.logfileName = logfileName;
			this.rotateInterval = rotateInterval;
        }

		private string GenerateLogFilePath()
        {
			if (logfilePath.Contains("%APPDATA%"))
            {
				logfilePath = logfilePath.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }

			return logfilePath + "\\" + logfileName + "-" + loggingStartTime.ToString("yyyy-MM-dd_hhmmss") + ".csv";
		}

		private void UpdateLoggingStartTime()
        {
			loggingStartTime = DateTime.Now;
        }

		private void Form1_Load(object sender, EventArgs e)
        {	
			InitTimer();
			CreateCSVFile(GenerateLogFilePath());
        }
    }
}
