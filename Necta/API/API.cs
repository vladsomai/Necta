using Necta.NectaServices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Necta.API
{
    public class API_Handler
    {
        private static readonly HttpClient client = new HttpClient();
        private const string updateReceiptID_Prefix = "&row=";

        private static readonly object API_GET_URI_LOCK = new object();
        private static readonly object API_UPDATE_URI_LOCK = new object();
        private static readonly object API_REQUEST_INTERVAL_LOCK = new object();

        #region Configuration_variables

        private static Uri _API_GET_URI = null;
        private static Uri _API_UPDATE_URI = null;
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
        public static int API_REQUEST_INTERVAL
        {
            get { lock (API_REQUEST_INTERVAL_LOCK) { return _API_REQUEST_INTERVAL; } }
            set { lock (API_REQUEST_INTERVAL_LOCK) { _API_REQUEST_INTERVAL = value; } }
        }
        #endregion Configuration_variables 

        public static List<Receipt> FetchReceipts(string uri)
        {
            var result = Task.Run(async () => await MakeHTTP_request(uri)).Result;

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
            var result = Task.Run(async () => await MakeHTTP_request(uri)).Result;
        }

        private static async Task<string> MakeHTTP_request(string uri)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetStringAsync(uri);
            return response;
        }
    }
}
