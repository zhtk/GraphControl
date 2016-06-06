using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ConfigLoader loader;
            try
            {
                loader = new ConfigLoader();
            }
            catch (Exception e) {
                MessageBox.Show("XML configuration is invalid!", 
                                "Error", MessageBoxButtons.OK);
                return;
            }

            MainScreen screen = new MainScreen(loader.DeviceObjects, loader.EdgeLines);
            screen.Width = loader.ScreenWidth;
            screen.Height = loader.ScreenHeight;

            Application.Run(screen);
        }
    }
}
