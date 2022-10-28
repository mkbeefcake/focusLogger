using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test1
{
    static class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();
        [DllImport("kernel32.dll", EntryPoint = "AttachConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern UInt32 AttachConsole(UInt32 dwProcessId);

        private const UInt32 ATTACH_PARRENT = 0xFFFFFFFF;
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;

        static string logFilePath = "";
        static string logFileName = "";
        static int rotateLevel = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Parse command line
            string[] args = Environment.GetCommandLineArgs();
            SetConsole();

            if (ParseArguments(args) == true)
            {
                //Console.WriteLine("Any keypress to exit application....");
                //Console.ReadKey(true);
                return;
            }

            // Launch Main app
            Form1 mainForm = new Form1();
            mainForm.SetLogInfo(logFilePath, logFileName, rotateLevel);

            Application.Run(mainForm);
        }

        public class CommandLineOptions
        {
            [Option(shortName: 'p', longName: "logfilePath", Required = false, HelpText = "Log file's path", Default = "%APPDATA%\\focuslogger")]
            public string logFilePath { get; set; }

            [Option(shortName: 'f', longName: "logfileName", Required = false, HelpText = "Log file's name", Default = "focuslogger")]
            public string logFileName { get; set; }
            [Option(shortName: 'l', longName: "rotateInterval", Required = true, HelpText = " log rotation interval in minutes", Default = 60)]
            public int rotateInterval { get; set; }

            [Usage(ApplicationAlias = "focusLogger.exe")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    return new List<Example>() {
                        new Example(
                            "The example", 
                            new CommandLineOptions { 
                                logFilePath = "%APPDATA%\\focuslogger",
                                logFileName = "focuslogger",
                                rotateInterval = 60
                            }
                        )
                    };
                }
            }
        }

        static bool ParseArguments(string[] args)
        {
            bool ret = true;
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(o => {
                    Program.logFilePath = o.logFilePath;
                    Program.logFileName = o.logFileName;
                    Program.rotateLevel = o.rotateInterval;
                    ret = false;
            })
                .WithNotParsed<CommandLineOptions>(e => {
                    ret = true;
            });
            return ret;
        }

        static void SetConsole()
        {
            AttachConsole(ATTACH_PARRENT);
            //AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }

    }
}
