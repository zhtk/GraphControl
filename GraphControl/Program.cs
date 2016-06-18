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
        /// Obiekt reprezentujący okno wyświetlane na ekranie
        /// </summary>
        public static MainScreen Screen {get; private set; }

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
                MessageBox.Show("XML configuration is invalid! " + e.Message, 
                                "Error", MessageBoxButtons.OK);
                return;
            }

            Screen = new MainScreen(loader.DeviceObjects, loader.EdgeLines);
            Screen.Width = loader.ScreenWidth;
            Screen.Height = loader.ScreenHeight;

            Application.Run(Screen);
        }
    }
}
