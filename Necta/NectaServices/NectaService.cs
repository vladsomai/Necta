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
        public delegate void LoadHtmlDelegate(string receiptHTML);

        public static void RunService()
        {
            PrintReceiptDelegate PRdel = new PrintReceiptDelegate(Necta.PrintReceipt);
            LoadHtmlDelegate LHDdel = new LoadHtmlDelegate(Necta.LoadHtmlDocument);

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
                    continue;
                }

                if (receipts.Count == 0)
                {
                    NectaLogService.WriteLog("No receipts received from API", LogLevels.INFO);
                    continue;
                }

                foreach (Receipt receipt in receipts)
                {
                    PrinterInfo printer = null;
                    try
                    {
                        printer = WinPrinter.GetPrinterInfo(receipt.PrinterName);

                    }
                    catch(Exception ex)
                    {
                        NectaLogService.WriteLog("The Printer's info could not be fetched because: ", LogLevels.ERROR);
                        NectaLogService.WriteLog("Invalid printer name for receipt ID: " + receipt.ID, LogLevels.ERROR);
                        NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                    }

                    try 
                    {
                        if (printer == null) continue;

                        if (printer.HasPaperProblem ||
                        !printer.HasToner ||
                        printer.IsInError ||
                        printer.IsNotAvailable ||
                        printer.IsOffline ||
                        printer.IsOutOfPaper ||
                        printer.IsPaperJammed ||
                        printer.IsTonerLow ||
                        printer.NeedUserIntervention)
                        {
                            PrinterError printerError = new PrinterError();
                            printerError.API_GET_URI = API_Handler.API_GET_URI.ToString();
                            printerError.PrinterID = receipt.Printer.ToString();
                            printerError.printerInfo = printer;

                            if (API_Handler.API_PRINTER_INFO_URI == null)
                            {
                                NectaLogService.WriteLog("Cannot send printer info because PRINTER_INFO_URI is not valid.", LogLevels.ERROR);
                                continue;
                            }
                            
                            NectaLogService.WriteLog("Printer has an error, please check the printer info below:", LogLevels.ERROR);
                            var options = new JsonSerializerOptions { WriteIndented = true };
                            NectaLogService.WriteLog(JsonSerializer.Serialize(printerError, options), LogLevels.ERROR);
                            API_Handler.SendPrinterErrorInfo(printerError, API_Handler.API_PRINTER_INFO_URI.ToString());
                            continue;
                        }

                        //load the html document on the main thread
                        Necta.MainThreadDispatcher.Invoke(LHDdel, new object[] { receipt.HTML });

                        //wait for the document to complete loading(done in main thread) 
                        while (!Necta.mHtmlDocumentIsLoaded)
                            Thread.Sleep(100);

                        //call PrintReceipt from main thread
                        Necta.MainThreadDispatcher.Invoke(PRdel, new object[] { receipt });
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
