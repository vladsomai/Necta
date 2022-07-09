using Necta.API;
using System.IO;
using System.Text.Json;

namespace Necta.NectaServices
{
    class NectaConfigService
    {
        private const string nectaConfigFile = @"C:\Necta\Config\Config.json";
        private const string nectaConfigPath = @"C:\Necta\Config\";

        public static void Initialize()
        {
            if (!Directory.Exists(nectaConfigPath))
                Directory.CreateDirectory(nectaConfigPath);

            if (File.Exists(nectaConfigFile))
                return;

            using (StreamWriter file = new StreamWriter(nectaConfigFile, append: true))
            {
                file.WriteLine("{");
                file.WriteLine("    \"API_GET_URI\": \"https://develop.meals.lv/other/printer/?method=queue&key=rest4\",");
                file.WriteLine("    \"API_UPDATE_URI\": \"https://develop.meals.lv/other/printer/?method=setPrinted&key=rest4\",");
                file.WriteLine("    \"API_PRINTER_INFO_URI\": \"https://develop.meals.lv/other/printer/?method=queue&key=rest4\",");//TODO: CHANGE THE API LINK
                file.WriteLine("    \"API_REQUEST_INTERVAL\": 3000");
                file.WriteLine("}");
            }
        }
        public static ConfigType ReadConfig()
        {
            string configContent = "";

            using (StreamReader configFile = new StreamReader(nectaConfigFile))
            {
                configContent = configFile.ReadToEnd();
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var config = JsonSerializer.Deserialize<ConfigType>(configContent, options);

            return config;
        }
        public static void SaveNewConfig()
        {
            if (!Directory.Exists(nectaConfigPath))
                Directory.CreateDirectory(nectaConfigPath);

            using (StreamWriter file = new StreamWriter(nectaConfigFile))
            {
                file.WriteLine("{");
                file.WriteLine("    \"API_GET_URI\": \"{0}\",", API_Handler.API_GET_URI);
                file.WriteLine("    \"API_UPDATE_URI\": \"{0}\",", API_Handler.API_UPDATE_URI);
                file.WriteLine("    \"API_PRINTER_INFO_URI\": \"{0}\",",API_Handler.API_PRINTER_INFO_URI);
                file.WriteLine("    \"API_REQUEST_INTERVAL\": {0}", API_Handler.API_REQUEST_INTERVAL);
                file.WriteLine("}");
            }
        }
    }

    class ConfigType
    {
        public string API_GET_URI { get; set; }
        public string API_UPDATE_URI { get; set; }
        public string API_PRINTER_INFO_URI { get; set; }
        public int API_REQUEST_INTERVAL { get; set; }
    }
}
