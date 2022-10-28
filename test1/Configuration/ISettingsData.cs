using System.Collections;
using System.IO;

namespace test1
{
	public interface ISettingsData
	{
		bool ResetToDefault();
		void Save();
		void SaveAs(string fileName);
		void Load();
		void LoadFrom(string fileName);
		FileInfo XmlFile { get; }
		IList Items { get; }

	}
}
