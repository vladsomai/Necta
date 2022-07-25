using Necta.API;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Necta.NectaServices
{
    class NectaConfigService
    {
        private const string nectaConfigPath = @"C:\Necta\Config\";
        public const string nectaConfigFile = @"C:\Necta\Config\Config.json";
        public const string nectaPasswordFile = @"C:\Necta\Config\Password.json";
        private const string defaultPassword = "meals";

        private static readonly object LogFileSize_lock = new object();

        private static int _LogFileSize=50000;
        public static int LogFileSize
        {
            get { lock (LogFileSize_lock) { return _LogFileSize; } }
            set { lock (LogFileSize_lock) { _LogFileSize = value; } }
        }

        public static void Initialize()
        {
            if (!Directory.Exists(nectaConfigPath))
                Directory.CreateDirectory(nectaConfigPath);

            if (File.Exists(nectaConfigFile) && File.Exists(nectaPasswordFile))
                return;

            using (StreamWriter file = new StreamWriter(nectaConfigFile))
            {
                file.WriteLine("{");
                file.WriteLine("    \"API_GET_URI\": \"https://develop.meals.lv/other/printer/?method=queue&key=rest4\",");
                file.WriteLine("    \"API_UPDATE_URI\": \"https://develop.meals.lv/other/printer/?method=setPrinted&key=rest4\",");
                file.WriteLine("    \"API_PRINTER_INFO_URI\": \"https://develop.meals.lv/other/printer/?method=printerStatus\",");
                file.WriteLine("    \"CHROME_PATH\": \"C:/Program Files/Google/Chrome/Application/\",");
                file.WriteLine("    \"LOG_FILE_SIZE\": 500000,");
                file.WriteLine("    \"API_REQUEST_INTERVAL\": 3000");
                file.WriteLine("}");
            }

            using (StreamWriter file = new StreamWriter(nectaPasswordFile))
            {
                file.WriteLine("{");
                file.WriteLine("    \"Password\": \"{0}\"", Hasher.GetSha256Hash(defaultPassword));
                file.WriteLine("}");
            }
            File.SetAttributes(nectaPasswordFile, FileAttributes.Hidden);
        }
        public static void SaveNewConfig()
        {
            if (!Directory.Exists(nectaConfigPath))
                Directory.CreateDirectory(nectaConfigPath);

            var currentConfig = ConfigContent<ConfigType>.ReadConfig(nectaConfigFile);

            using (StreamWriter file = new StreamWriter(nectaConfigFile))
            {
                file.WriteLine("{");
                file.WriteLine("    \"API_GET_URI\": \"{0}\",", API_Handler.API_GET_URI);
                file.WriteLine("    \"API_UPDATE_URI\": \"{0}\",", API_Handler.API_UPDATE_URI);
                file.WriteLine("    \"API_PRINTER_INFO_URI\": \"{0}\",", API_Handler.API_PRINTER_INFO_URI);
                file.WriteLine("    \"CHROME_PATH\": \"{0}\",", currentConfig.CHROME_PATH);
                file.WriteLine("    \"LOG_FILE_SIZE\": {0},", currentConfig.LOG_FILE_SIZE);
                file.WriteLine("    \"API_REQUEST_INTERVAL\": {0}", API_Handler.API_REQUEST_INTERVAL);
                file.WriteLine("}");
            }
        }
    }

    //generic class that will read any json document and return the c# object 
    class ConfigContent<T>
    {
        private static readonly object ConfigFile_lock = new object();

        public static T ReadConfig(string pathToFile)
        {
            lock (ConfigFile_lock)
            {
                string configContent = "";

                using (StreamReader configFile = new StreamReader(pathToFile))
                {
                    configContent = configFile.ReadToEnd();
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var config = JsonSerializer.Deserialize<T>(configContent, options);

                return config;
            }
        }
    }

    public static class Hasher
    {
        public static string GetSha256Hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }

            return Sb.ToString();
        }
    }

    class ConfigType
    {
        public string API_GET_URI { get; set; }
        public string API_UPDATE_URI { get; set; }
        public string API_PRINTER_INFO_URI { get; set; }
        public string CHROME_PATH { get; set; }
        public int LOG_FILE_SIZE { get; set; }
        public int API_REQUEST_INTERVAL { get; set; }
    }

    class PasswordType
    {
        public string Password { get; set; }
    }
}
