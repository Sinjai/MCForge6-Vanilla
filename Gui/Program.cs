using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MCForge.Gui.Forms;
using MCForge.Utils.Settings;
using MCForge.Gui.Utils;
using MCForge.Core;
using MCForge.Utils;

namespace NewMCForgeGui {
    static class Program {

        private static FormSplashScreen _splashScreen;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread] //Single Apartment Thread only
        static void Main() {
            Logger.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            _splashScreen = new FormSplashScreen();

            Application.Run(_splashScreen);
#if !DEBUG
            if (_splashScreen.DialogResult == DialogResult.OK){
#endif
            _splashScreen.Dispose();
            Application.Run(new FormMainScreen());
#if !DEBUG
            }
#endif

            if ( Server.ShuttingDown || Server.Running ) {
                Server.Stop();
                ServerSettings.Save();
                GuiSettings.Save();
            }

        }

    }
}
