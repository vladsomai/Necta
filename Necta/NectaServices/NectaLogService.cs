using System;
using System.IO;
using System.Threading;

namespace Necta.NectaServices
{
    class NectaLogService
    {
        private static NectaLogService serviceInstance { get; set; }
        private static string currentLogFile = null;
        private static readonly object lockFile = new object();
        private const string nectaLogPath = @"C:\Necta\Logs\";

        private NectaLogService()
        {
            if (!Directory.Exists(nectaLogPath))
                Directory.CreateDirectory(nectaLogPath);

            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string currentYear = DateTime.Now.Year.ToString();
            string currentHour = DateTime.Now.Hour.ToString();
            string currentMinute = DateTime.Now.Minute.ToString();

            var currentDate = currentDay + "-" + currentMonth + "-" + currentYear + "_" + currentHour + "-" + currentMinute;
            currentLogFile = nectaLogPath + "LogFile_" + currentDate + ".log";

            using (StreamWriter file = new StreamWriter(currentLogFile))
            {
                file.WriteLine(DateTime.Now.ToString() + " | " + "Log file created!");
            }
        }

        public static void RunService()
        {
            if (serviceInstance == null)
                serviceInstance = new NectaLogService();

            while(true)
            {
                Thread.Sleep(10000);

                var config = ConfigContent<ConfigType>.ReadConfig(NectaConfigService.nectaConfigFile);

                lock (lockFile)
                {
                    string[] logFileContent = File.ReadAllLines(currentLogFile);

                    if (logFileContent.Length > config.LOG_FILE_SIZE)
                    {
                        var logFileContentAfterDeletion = new string[config.LOG_FILE_SIZE];
                        Array.Copy(logFileContent, logFileContent.Length - config.LOG_FILE_SIZE , logFileContentAfterDeletion, 0, config.LOG_FILE_SIZE);

                        File.WriteAllLines(currentLogFile, logFileContentAfterDeletion);
                    }
                }
            }
        }

        public static void WriteLog(string source, string log, string logLevel)
        {
            lock (lockFile)
            {
                if (serviceInstance == null) return;
                using (StreamWriter file = new StreamWriter(currentLogFile, append: true))

                {
                    file.WriteLine(DateTime.Now.ToString() + " | " + source + " | " + logLevel + " | " + log);
                }
            }
        }
    }

    public class LogLevels
    {
        public static string ERROR = "ERROR";
        public static string INFO = "INFO";
        public static string WARNING = "WARNING";
    }
}
