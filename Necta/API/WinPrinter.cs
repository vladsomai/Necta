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

            var JSON_RawPrinterInfo = JsonSerializer.Serialize(printQueue);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var PrinterInfo = JsonSerializer.Deserialize<PrinterInfo>(JSON_RawPrinterInfo, options);

            return PrinterInfo;
        }

    #region "winspoolAPI"
    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);
        #endregion
    }

    public class PrinterInfo
    {
        public bool NeedUserIntervention { get; set; }
        public bool HasToner { get; set; }
        public bool IsTonerLow { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsOffline { get; set; }
        public bool HasPaperProblem { get; set; }
        public bool IsOutOfPaper { get; set; }
        public bool IsPaperJammed { get; set; }
        public bool IsInError { get; set; }
        public int QueueStatus { get; set; }//1 - if the printer is in queue and is busy or 0-if the printer has noting to print
        public string FullName { get; set; }
        public int NumberOfJobs { get; set; }//number of pages in the queue that need to be printed
    }
}
