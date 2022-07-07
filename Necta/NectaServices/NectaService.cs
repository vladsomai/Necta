using Necta.API;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Necta.NectaServices
{
    class NectaService
    {
        public delegate void PrintReceiptDelegate(Receipt receipt);

        public static void RunService()
        {
            PrintReceiptDelegate PRdel = new PrintReceiptDelegate(Necta.PrintReceipt);

            List<Receipt> receipts = null;
            while (true)
            {
                Thread.Sleep(API_Handler.API_REQUEST_INTERVAL);

                if (API_Handler.API_GET_URI == null || API_Handler.API_UPDATE_URI == null)
                {
                    NectaLogService.WriteLog("Fetching data not possible because API GET or UPDATE URIs are not valid, please set valid URIs!", LogLevels.ERROR);
                    continue;
                }

                try
                {
                    NectaLogService.WriteLog("Fetching new receipts from address: " + API_Handler.API_GET_URI.ToString(), LogLevels.INFO);
                    receipts = API_Handler.FetchReceipts(API_Handler.API_GET_URI.ToString());
                }
                catch (Exception ex)
                {
                    NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                    NectaLogService.WriteLog(ex.InnerException?.Message, LogLevels.ERROR);
                }

                if (receipts.Count == 0)
                {
                    NectaLogService.WriteLog("No receipts received from API", LogLevels.INFO);
                    continue;
                }

                foreach (Receipt receipt in receipts)
                {
                    try
                    {
                        Necta.MainThreadDispatcher.Invoke(PRdel, new object[] { receipt });//call PrintReceipt from main thread
                        API_Handler.UpdateReceipt(receipt.ID, API_Handler.API_UPDATE_URI.ToString());
                    }
                    catch (Exception ex)
                    {
                        NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                        NectaLogService.WriteLog(ex.InnerException?.Message, LogLevels.ERROR);
                    }
                }
            }
        }
    }
}
