using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;
using Necta.API;
using Necta.NectaServices;
using System.Windows.Threading;
using PuppeteerSharp;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Necta
{
    public partial class Necta : MaterialForm
    {
        //the following dispatcher object will be used to invoke the print method from another thread
        public static Dispatcher MainThreadDispatcher = Dispatcher.CurrentDispatcher;
        public static string PathToReceipt = @"C:\Necta\Receipt.pdf";
        public static string PathToChrome = "";
        private static Browser browserForConverting = null;

        private static readonly object printingInProgressLock = new object();
        private static bool _printingInProgress = false;
        public static bool printingInProgress
        {
            get { lock (printingInProgressLock) { return _printingInProgress; } }
            set { lock (printingInProgressLock) { _printingInProgress = value; } }
        }

        public Necta()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red300, Primary.Red300, Primary.BlueGrey500, Accent.LightBlue200, TextShade.BLACK);

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
            PathToChrome = config.CHROME_PATH + "chrome.exe";
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

        public static async void PrintReceipt(Receipt receipt)
        {
            NectaLogService.WriteLog("Setting default printer", LogLevels.INFO);
            bool defaultPrinterResult = WinPrinter.SetDefaultPrinter(receipt.PrinterName);

            if (defaultPrinterResult)
            {
                NectaLogService.WriteLog("**************** Printing in progress ****************", LogLevels.INFO);
                NectaLogService.WriteLog("Receipt ID: " + receipt.ID, LogLevels.INFO);
                NectaLogService.WriteLog("Receipt will be printed on printer: " + receipt.PrinterName + " with ID: " + receipt.Printer, LogLevels.INFO);

                try
                {
                    await ChromeConvertHtmlToPdf(receipt.HTML);
                    printingInProgress = false;
                }
                catch (Exception ex)
                {
                    NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                    NectaLogService.WriteLog(ex.InnerException?.Message, LogLevels.ERROR);

                    if (browserForConverting != null)
                        await browserForConverting.CloseAsync();

                    if (ex.Message == "Failed to launch browser! path to executable does not exist")
                    {
                        NectaLogService.WriteLog("You must stop Necta.exe and add the corect path to the chrome executable in C:/Necta/Config/Config.json!", LogLevels.ERROR);
                    }
                    printingInProgress = false;
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

        private static async Task<int> ChromeConvertHtmlToPdf(string html)
        {
            {
                string[] args1 = { "--disable-gpu" };
                browserForConverting = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = PathToChrome,
                    Args = args1,
                });

                NectaLogService.WriteLog("Starting new page", LogLevels.INFO);
                using (var page = await browserForConverting.NewPageAsync())
                {
                    var navigtionOptions = new NavigationOptions();
                    WaitUntilNavigation[] waitUntilNavigations = { WaitUntilNavigation.Load };
                    navigtionOptions.WaitUntil = waitUntilNavigations;
                    NectaLogService.WriteLog("Setting HTML content on page", LogLevels.INFO);
                    await page.SetContentAsync(html, navigtionOptions);
                    await page.WaitForTimeoutAsync(1000);
                    NectaLogService.WriteLog("Saving the PDF", LogLevels.INFO);
                    await page.PdfAsync(PathToReceipt, new PdfOptions() { PrintBackground = true, OmitBackground=false});
                }
                await browserForConverting.CloseAsync();
            }

            ProcessPrint();
            NectaLogService.WriteLog("Printing done!", LogLevels.INFO);

            return 0;
        }

        private static void ProcessPrint()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = PathToReceipt;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
        }
    }
}
