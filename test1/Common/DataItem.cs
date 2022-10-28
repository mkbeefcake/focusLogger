using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace test1
{
	public class DataRecord
	{
		public string Timestamp { get; set; }
		public int ProcessId { get; set; }
		public string ProcessName { get; set; }
		public string ComputerName { get; set; }
		public string UserName { get; set; }
		public string WindowTitle { get; set; }
	}

	public class DataItem : ISettingsItem, INotifyPropertyChanged
	{

		public DateTime Date { get => _Date; set => SetProperty(ref _Date, value); }
		DateTime _Date;

		public int ProcessId { get => _ProcessId; set => SetProperty(ref _ProcessId, value); }
		int _ProcessId;

		public string ProcessName { get => _ProcessName; set => SetProperty(ref _ProcessName, value); }
		string _ProcessName;

		public string ProcessPath { get => _ProcessPath; set => SetProperty(ref _ProcessPath, value); }
		string _ProcessPath;

		public string WindowTitle { get => _WindowTitle; set => SetProperty(ref _WindowTitle, value); }
		string _WindowTitle;

		public bool HasMouse { get => _HasMouse; set => SetProperty(ref _HasMouse, value); }
		bool _HasMouse;

		public bool HasKeyboard { get => _HasKeyboard; set => SetProperty(ref _HasKeyboard, value); }
		bool _HasKeyboard;

		public bool HasCaret { get => _HasCaret; set => SetProperty(ref _HasCaret, value); }
		bool _HasCaret;

		public bool IsActive { get => _IsActive; set => SetProperty(ref _IsActive, value); }
		bool _IsActive;

		public bool IsSame(DataItem item)
		{
			return
			item.ProcessId == ProcessId &&
			//item.HasMouse == HasMouse &&
			//item.HasKeyboard == HasKeyboard &&
			//item.HasCaret == HasCaret &&
			//item.IsActive == IsActive &&
			item.WindowTitle == WindowTitle;
		}

		public DataRecord Convert()
        {
			return new DataRecord
			{
				Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
				ProcessId = this.ProcessId,
				ProcessName = this.ProcessPath,
				ComputerName = Environment.MachineName.ToString(),
				UserName = Environment.UserName, // System.Security.Principal.WindowsIdentity.GetCurrent().Name,
				WindowTitle = this.WindowTitle
			};
        }

		#region ■ ISettingsItem

		bool ISettingsItem.Enabled { get => IsEnabled; set => IsEnabled = value; }
		private bool IsEnabled;

		public bool IsEmpty =>
			string.IsNullOrEmpty(ProcessName);

		#endregion

		#region ■ INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
		{
			property = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}
}
