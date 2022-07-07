using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;
using Necta.API;
using Necta.NectaServices;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Necta
{
    public partial class Necta : MaterialForm
    {
        private static readonly WebBrowser webBrowser = new WebBrowser();

        //the following dispatcher object will be used to invoke the print method from another thread
        public static Dispatcher MainThreadDispatcher = Dispatcher.CurrentDispatcher;
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
                SaveAndValidateURI();

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
            ApiRequestInterval_value.Value = config.API_REQUEST_INTERVAL;
        }

        private void SaveAndValidateURI()
        {
            NectaLogService.WriteLog("Saving new uri links..", LogLevels.INFO);
            try
            {
                Uri getURI = new Uri(ApiGetUri_textBox.Text);
                Uri updateURI = new Uri(ApiUpdateUri_textBox.Text);

                API_Handler.API_GET_URI = getURI;
                API_Handler.API_UPDATE_URI = updateURI;
                API_Handler.API_REQUEST_INTERVAL = (int)ApiRequestInterval_value.Value;

                NectaConfigService.SaveNewConfig();

                if (API_Handler.API_REQUEST_INTERVAL < 3000)
                    NectaLogService.WriteLog("The request interval value is too low, please enter a value greater than 3000ms!", LogLevels.WARNING);

                NectaLogService.WriteLog("The following new URI links will now be used to send and receive data through HTTP requests from the API!", LogLevels.INFO);
                NectaLogService.WriteLog("GET URI: " + API_Handler.API_GET_URI.ToString(), LogLevels.INFO);
                NectaLogService.WriteLog("UPDATE URI: " + API_Handler.API_UPDATE_URI.ToString(), LogLevels.INFO);
                NectaLogService.WriteLog("Request interval: " + API_Handler.API_REQUEST_INTERVAL.ToString(), LogLevels.INFO);
            }
            catch (Exception ex)
            {
                NectaLogService.WriteLog(ex.Message, LogLevels.ERROR);
                NectaLogService.WriteLog("Please enter a valid URI in the GET and UPDATE URI text boxes!", LogLevels.ERROR);

                API_Handler.API_GET_URI = null;
                API_Handler.API_UPDATE_URI = null;
                API_Handler.API_REQUEST_INTERVAL = 3000;

                MessageBox.Show(this, "Please enter valid URI links in both text boxes!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PrintReceipt(Receipt receipt)
        {
            bool defaultPrinterResult = SetDefaultPrinter(receipt.PrinterName);
            webBrowser.DocumentText = receipt.HTML;

            if (defaultPrinterResult)
            {
                NectaLogService.WriteLog("**************** Printing in progress ****************", LogLevels.INFO);
                NectaLogService.WriteLog("Receipt ID: " + receipt.ID, LogLevels.INFO);
                NectaLogService.WriteLog("Receipt will be printed on printer: " + receipt.PrinterName + " with ID: " + receipt.Printer, LogLevels.INFO);
                //webBrowser.Print();
            }
            else
                NectaLogService.WriteLog("Invalid printer name for receipt ID: " + receipt.ID, LogLevels.ERROR);
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);
    }
}
