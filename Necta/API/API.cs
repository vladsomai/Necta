using Necta.NectaServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Necta.API
{
    public class API_Handler
    {
        private static readonly HttpClient client = new HttpClient();
        private const string updateReceiptID_Prefix = "&row=";

        #region Configuration_variables
        private static readonly object API_GET_URI_LOCK = new object();
        private static readonly object API_UPDATE_URI_LOCK = new object();
        private static readonly object API_PRINTER_INFO_URI_LOCK = new object();
        private static readonly object API_REQUEST_INTERVAL_LOCK = new object();

        private static Uri _API_GET_URI = null;
        private static Uri _API_UPDATE_URI = null;
        private static Uri _API_PRINTER_INFO_URI = null;
        private static int _API_REQUEST_INTERVAL = 3000;
        public static Uri API_GET_URI
        {
            get { lock (API_GET_URI_LOCK) { return _API_GET_URI; } }
            set { lock (API_GET_URI_LOCK) { _API_GET_URI = value; } }
        }
        public static Uri API_UPDATE_URI
        {
            get { lock (API_UPDATE_URI_LOCK) { return _API_UPDATE_URI; } }
            set { lock (API_UPDATE_URI_LOCK) { _API_UPDATE_URI = value; } }
        }
        public static Uri API_PRINTER_INFO_URI
        {
            get { lock (API_PRINTER_INFO_URI_LOCK) { return _API_PRINTER_INFO_URI; } }
            set { lock (API_PRINTER_INFO_URI_LOCK) { _API_PRINTER_INFO_URI = value; } }
        }
        public static int API_REQUEST_INTERVAL
        {
            get { lock (API_REQUEST_INTERVAL_LOCK) { return _API_REQUEST_INTERVAL; } }
            set { lock (API_REQUEST_INTERVAL_LOCK) { _API_REQUEST_INTERVAL = value; } }
        }
        #endregion Configuration_variables 

        public static List<Receipt> FetchReceipts(string uri)
        {
            var result = Task.Run(async () => await MakeGetHTTP_request(uri)).Result;

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultJSON = JsonSerializer.Deserialize<List<Receipt>>(result, options);
                return resultJSON;
            }
            catch (Exception)
            {
                NectaLogService.WriteLog("The received data from the API is not a valid JSON!", LogLevels.ERROR);
                NectaLogService.WriteLog("The following data was received: " + result, LogLevels.ERROR);
                return new List<Receipt>();
            }
        }

        public static void UpdateReceipt(string receiptID, string updateUri)
        {
            string uri = updateUri + updateReceiptID_Prefix + receiptID;
            var result = Task.Run(async () => await MakeGetHTTP_request(uri)).Result;
        }

        public static void SendPrinterErrorInfo(PrinterError printerError, string printerErrorURI)
        {
            string JSON_PrinterInfo = JsonSerializer.Serialize(printerError);

            var result = Task.Run(async () => await MakePostHTTP_request(printerErrorURI, JSON_PrinterInfo)).Result;

            if (result.StatusCode != HttpStatusCode.OK)
                throw new Exception("The POST request with the printer info has failed, please check response from the server: " + result.ToString());
        }

        private static async Task<string> MakeGetHTTP_request(string uri)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetStringAsync(uri);
            return response;
        }

        private static async Task<HttpResponseMessage> MakePostHTTP_request(string uri, string data)
        {
            StringContent stringContent = new StringContent(data);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.PostAsync(uri, stringContent);
            return response;
        }
    }

    public class PrinterError
    {
        public string API_GET_URI { get; set; }
        public string PrinterID { get; set; }
        public string printerInfo {get; set;}
    }
}
