using Necta.API;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;

namespace Necta.NectaServices
{
    class NectaService
    {
        public delegate void PrintReceiptDelegate(Receipt receipt);

        public static void RunService()
        {
            PrintReceiptDelegate PRdel = new PrintReceiptDelegate(NectaApp.PrintReceipt);

            List<Receipt> receipts = null;

            while (true)
            {
                while (NectaApp.printingInProgress)
                    Thread.Sleep(500);

                Thread.Sleep(API_Handler.API_REQUEST_INTERVAL);

                if (API_Handler.API_GET_URI == null || API_Handler.API_UPDATE_URI == null)
                {
                    NectaLogService.WriteLog("Fetching data not possible because GET / UPDATE / INFO URIs are not valid, please set valid URIs!", LogLevels.ERROR);
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
                    continue;
                }

                if (receipts.Count == 0)
                {
                    NectaLogService.WriteLog("No receipts received from API", LogLevels.INFO);
                    continue;
                }

                foreach (Receipt receipt in receipts)
                {
                    while (NectaApp.printingInProgress)
                        Thread.Sleep(500);

                    try
                    {
                        bool printerIsInError = true;
                        while (printerIsInError)
                        {
                            PrinterInfo printer = null;
                            try
                            {
                                printer = WinPrinter.GetPrinterInfo(receipt.PrinterName);
                            }
                            catch (Exception ex)
                            {
                                NectaLogService.WriteLog("The Printer's info could not be fetched because: ", LogLevels.ERROR);
                                NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                                NectaLogService.WriteLog("The Printer's info must be fetched before sending a printing command, please solve the issue.", LogLevels.ERROR);
                            }

                            if (printer == null)
                            {
                                printer = new PrinterInfo()
                                {
                                    QueueStatus = System.Printing.PrintQueueStatus.Error
                                };
                            }

                            if (printer.QueueStatus != System.Printing.PrintQueueStatus.None)
                            {
                                PrinterError printerError = new PrinterError();
                                printerError.API_GET_URI = API_Handler.API_GET_URI.ToString();
                                printerError.PrinterID = receipt.Printer.ToString();
                                printerError.printerInfo = printer.QueueStatus.ToString();

                                if (API_Handler.API_PRINTER_INFO_URI == null)
                                {
                                    NectaLogService.WriteLog("Cannot send printer info because PRINTER_INFO_URI is not valid.", LogLevels.ERROR);
                                }
                                else
                                {
                                    NectaLogService.WriteLog("Printer has an error, please check the printer info below:", LogLevels.ERROR);
                                    var options = new JsonSerializerOptions { WriteIndented = true };
                                    NectaLogService.WriteLog(JsonSerializer.Serialize(printerError, options), LogLevels.ERROR);
                                    API_Handler.SendPrinterErrorInfo(printerError, API_Handler.API_PRINTER_INFO_URI.ToString());
                                }

                                Thread.Sleep(API_Handler.API_REQUEST_INTERVAL);
                            }
                            else
                            {
                                printerIsInError = false;
                            }
                        }

                        NectaApp.printingInProgress = true;

                        //call PrintReceipt from main thread
                        NectaApp.MainThreadDispatcher.Invoke(PRdel, new object[] { receipt });
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