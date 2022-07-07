using Necta.NectaServices;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Necta
{
    static class Program
    {
        public static Thread logServiceThread = new Thread(NectaLogService.CreateService);
        public static Thread service = new Thread(NectaService.RunService);
     
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Necta());

        }
    }
}
