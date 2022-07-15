﻿using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;
using Necta.API;
using Necta.NectaServices;
using System.Windows.Threading;
using PuppeteerSharp;
using PdfPrintingNet;

namespace Necta
{
    public partial class Necta : MaterialForm
    {
        //the following dispatcher object will be used to invoke the print method from another thread
        public static Dispatcher MainThreadDispatcher = Dispatcher.CurrentDispatcher;
        public static string PathToReceipt = "C:/Necta/Receipt.pdf";
        
        private static BrowserFetcher browserFetcher = new BrowserFetcher();
        private static Browser browserForConverting = null;

        public Necta()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red300, Primary.Red300, Primary.BlueGrey500, Accent.LightBlue200, TextShade.BLACK);
            
            browserFetcher.DownloadAsync();

            try
            {
                NectaConfigService.Initialize();
                SetDataFromConfig();
                SaveAndValidateURI(true);

                Program.logServiceThread.Start();

                Program.service.SetApartmentState(System.Threading.ApartmentState.STA);
                Program.service.Start();
            }
            catch (Exception ex)
            {
                NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveAndValidateURI();
        }

        private void SetDataFromConfig()
        {
            ConfigType config = NectaConfigService.ReadConfig();

            ApiGetUri_textBox.Text = config.API_GET_URI;
            ApiUpdateUri_textBox.Text = config.API_UPDATE_URI;
            ApiPrinterInfoUri_textBox.Text = config.API_PRINTER_INFO_URI;
            ApiRequestInterval_value.Value = config.API_REQUEST_INTERVAL;
        }

        private void SaveAndValidateURI(bool isFirstPaint = false)
        {
            NectaLogService.WriteLog("Saving new uri links..", LogLevels.INFO);
            try
            {
                Uri getURI = new Uri(ApiGetUri_textBox.Text);
                Uri updateURI = new Uri(ApiUpdateUri_textBox.Text);
                Uri printerInfoURI = new Uri(ApiPrinterInfoUri_textBox.Text);

                API_Handler.API_GET_URI = getURI;
                API_Handler.API_UPDATE_URI = updateURI;
                API_Handler.API_PRINTER_INFO_URI = printerInfoURI;
                API_Handler.API_REQUEST_INTERVAL = (int)ApiRequestInterval_value.Value;

                NectaConfigService.SaveNewConfig();

                if (API_Handler.API_REQUEST_INTERVAL < 3000)
                    NectaLogService.WriteLog("The request interval value is too low, please enter a value greater than 3000ms!", LogLevels.WARNING);

                NectaLogService.WriteLog("The following new URI links will now be used to send and receive data through HTTP requests from the API!", LogLevels.INFO);
                NectaLogService.WriteLog("GET URI: " + API_Handler.API_GET_URI.ToString(), LogLevels.INFO);
                NectaLogService.WriteLog("UPDATE URI: " + API_Handler.API_UPDATE_URI.ToString(), LogLevels.INFO);
                NectaLogService.WriteLog("PRINTER INFO URI: " + API_Handler.API_PRINTER_INFO_URI.ToString(), LogLevels.INFO);
                NectaLogService.WriteLog("Request interval: " + API_Handler.API_REQUEST_INTERVAL.ToString(), LogLevels.INFO);

                if (!isFirstPaint)
                    MessageBox.Show(this, "Data has been succesfully saved!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
            }
            catch (Exception ex)
            {
                NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                NectaLogService.WriteLog("Please enter a valid URI in the GET, UPDATE and PRINTER INFO URI text boxes!", LogLevels.ERROR);

                API_Handler.API_GET_URI = null;
                API_Handler.API_UPDATE_URI = null;
                API_Handler.API_PRINTER_INFO_URI = null;
                API_Handler.API_REQUEST_INTERVAL = 3000;

                MessageBox.Show(this, "Please enter valid URI links in all text boxes!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
            }
        }

        public static void PrintReceipt(Receipt receipt)
        {
            bool defaultPrinterResult = WinPrinter.SetDefaultPrinter(receipt.PrinterName);
            

            if (defaultPrinterResult)
            {
                NectaLogService.WriteLog("**************** Printing in progress ****************", LogLevels.INFO);
                NectaLogService.WriteLog("Receipt ID: " + receipt.ID, LogLevels.INFO);
                NectaLogService.WriteLog("Receipt will be printed on printer: " + receipt.PrinterName + " with ID: " + receipt.Printer, LogLevels.INFO);

                try
                {
                    ChromeConvertHtmlToPdf(receipt.HTML);
                }
                catch (Exception ex)
                {
                    NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                    NectaLogService.WriteLog(ex.InnerException?.Message, LogLevels.ERROR);
                    return;
                }

                API_Handler.UpdateReceipt(receipt.ID, API_Handler.API_UPDATE_URI.ToString());
            }
            else
            {
                NectaLogService.WriteLog("Invalid printer name for receipt ID: " + receipt.ID, LogLevels.ERROR);
            }
        }

        private void NectaNotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            NectaNotifyIcon1.Visible = false;
        }

        private void onResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                NectaNotifyIcon1.Visible = false;
            }
            else
            {
                Hide();
                this.WindowState = FormWindowState.Minimized;
                NectaNotifyIcon1.Visible = true;
            }
        }

        private static void PdfPrinting()
        {
            var pdfPrint = new PdfPrint("Meals", "demoKey");
            pdfPrint.Scale = PdfPrint.ScaleTypes.None;

            var status = pdfPrint.Print(PathToReceipt);
            if (status == PdfPrint.Status.OK)
            {
                NectaLogService.WriteLog("Successfully printed the receipt!", LogLevels.INFO);
            }
        }

        private static async void ChromeConvertHtmlToPdf(string html)
        {
            string[] args1 = { "--disable-gpu" };
            browserForConverting = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = args1,
            });

            using (var page = await browserForConverting.NewPageAsync())
            {
                var navigtionOptions = new NavigationOptions();
                WaitUntilNavigation[] waitUntilNavigations = { WaitUntilNavigation.Load, WaitUntilNavigation.Networkidle2 };
                navigtionOptions.WaitUntil = waitUntilNavigations;

                await page.SetContentAsync(html, navigtionOptions);
                await page.PdfAsync(PathToReceipt);
                await page.CloseAsync();
            }
            await browserForConverting.CloseAsync();

            PdfPrinting();
        }
    }
}
