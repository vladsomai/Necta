﻿using System;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Necta.API
{
    public class WinPrinter
    {
        private static PrintServer printServer = new PrintServer();

        public static PrinterInfo GetPrinterInfo(string printerName)
        {
            printServer.Refresh();
            PrintQueue printQueue = printServer.GetPrintQueue(printerName);
            printQueue.Refresh();

            bool printingError = false;

            try
            {
                var printJobEnum = printQueue.GetPrintJobInfoCollection().GetEnumerator();

                for (int i = 0; i < 2; i++)
                {
                    var nextExist = printJobEnum.MoveNext();

                    if (i == 1 && nextExist)//2 printing jobs are in the queue
                        printingError = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var JSON_RawPrinterInfo = JsonSerializer.Serialize(printQueue);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var PrinterInfo = JsonSerializer.Deserialize<PrinterInfo>(JSON_RawPrinterInfo, options);

            if (printingError)
                PrinterInfo.QueueStatus = PrintQueueStatus.Error;

            return PrinterInfo;
        }

        #region "winspoolAPI"
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);
        #endregion
    }

    public class PrinterInfo
    {
        public PrintQueueStatus QueueStatus { get; set; }
    }
}
