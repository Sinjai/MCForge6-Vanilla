using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using MCForge.Gui.Properties;
using MCForge.Utils;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using System.Threading;
using MCForge.Gui.Utils;
using MCForge.Gui.Components;
using MCForge.Core;
using MCForge.Interface.Command;
using MCForge.Interface.Plugin;
using MCForge.Utils.Settings;
using MCForge.Groups;
using MCForge.Gui.Dialogs;

namespace MCForge.Gui.Forms {
    public partial class FormSplashScreen : AeroForm {


        public string LogMessage { get; set; }

        private string _devList;

        private readonly static Font DrawingFont;
        private readonly static Bitmap HammerBitmap;
        private readonly static Bitmap MCForgeBitmap;

        static FormSplashScreen() {
            HammerBitmap = Resources.hirez_mcforge;
            MCForgeBitmap = Resources.mcforge_text;
            DrawingFont = new Font("Arial", Constants.DEV_TEXT_SIZE, FontStyle.Regular);
        }

        public FormSplashScreen() {
            InitializeComponent();
            GlassArea = ClientRectangle;
            LogMessage = "Loading The server for setting up of the server..";
            _devList = GenerateDevList();

            Paint += new PaintEventHandler(FormSplashScreen_Paint);
        }

        void FormSplashScreen_Paint(object sender, PaintEventArgs e) {

            if ( Natives.CanUseAero ) {
                Natives.FillBlackRegion(e.Graphics, ClientRectangle);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                var gDevPath = new GraphicsPath();
                var gLogPath = new GraphicsPath();
                var brush = new SolidBrush(Color.FromArgb(0x99, Color.Black));

                gDevPath.AddString(_devList, DrawingFont.FontFamily, (int)FontStyle.Regular, Constants.DEV_TEXT_SIZE,
                                                    new Point(Constants.LOGO_WIDTH + Constants.PADDING, Constants.TEXT_HEIGHT + Constants.PADDING), StringFormat.GenericDefault);

                gLogPath.AddString(LogMessage, DrawingFont.FontFamily, (int)FontStyle.Bold, Constants.LOG_TEXT_SIZE, new Point(Constants.PADDING, Height - 65 - Constants.PADDING), StringFormat.GenericDefault);

                e.Graphics.DrawImage(HammerBitmap, Constants.PADDING, Constants.PADDING, Constants.LOGO_WIDTH, Constants.LOGO_HEIGHT);
                e.Graphics.DrawImage(MCForgeBitmap, Constants.LOGO_WIDTH + Constants.PADDING, Constants.PADDING, Constants.TEXT_WIDTH, Constants.TEXT_HEIGHT);
                e.Graphics.FillPath(brush, gDevPath);
                e.Graphics.FillPath(brush, gLogPath);


                gDevPath.Dispose();
                gLogPath.Dispose();
                brush.Dispose();
            }
        }

        void FormSplashScreen_Load(object sender, EventArgs e) {
            if ( !Natives.CanUseAero ) {

                //Set up the server any way..
                ServerSettings.Init();
                new Thread(new ThreadStart(Server.Init)).Start();
                DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            Logger.OnRecieveErrorLog += new EventHandler<ErrorLogEventArgs>(Logger_OnRecieveErrorLog);
            Server.OnServerFinishSetup += new Server.ServerFinishSetup(Server_OnServerFinishSetup);
        }

        void Server_OnServerFinishSetup() {

            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)Server_OnServerFinishSetup);
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void FormSplashScreen_Shown(object sender, EventArgs e) {

            if ( !Natives.CanUseAero )
                return; //Just incase

            //We all like some good effects every so often.

            DrawText("Loading Settings...");
            ServerSettings.Init();
            Thread.Sleep(500);

            DrawText("Loading Plugins...");
            Thread.Sleep(500);
            Plugin.OnPluginLoad.All += new API.Events.Event<IPlugin, API.Events.PluginLoadEventArgs>.EventHandler(OnPluginLoad_All);

            DrawText("Loading Commands...");
            Thread.Sleep(500);
            Command.OnCommandLoad.All += new API.Events.Event<ICommand, API.Events.CommandLoadEventArgs>.EventHandler(OnCommandLoad_All);

            DrawText("Loading Groups...");
            Thread.Sleep(500);

            DrawText("Setting up server...");
            new Thread(new ThreadStart(Server.Init)).Start();

        }



        void FormSplashScreen_FormClosing(object sender, FormClosingEventArgs e) {
            Logger.OnRecieveLog -= Logger_OnRecieveLog;
            Logger.OnRecieveErrorLog -= Logger_OnRecieveErrorLog;
            Plugin.OnPluginLoad.All -= OnPluginLoad_All;
            Command.OnCommandLoad.All -= OnCommandLoad_All;
            Server.OnServerFinishSetup -= Server_OnServerFinishSetup;
        }

        void DrawText(string text) {
            if ( !Natives.CanUseAero )
                return;

            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { DrawText(text); });
                return;
            }

            LogMessage = text;

            Refresh();
        }

        #region Listeners

        void Logger_OnRecieveLog(object sender, LogEventArgs e) {
            DrawText(e.Message);
        }

        void Logger_OnRecieveErrorLog(object sender, ErrorLogEventArgs e) {

            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { Logger_OnRecieveErrorLog(sender, e); });
                return;
            }

            using ( var errors = new ErrorDialog(e.Exception) ) {
                switch ( errors.ShowDialog() ) {
                    case System.Windows.Forms.DialogResult.Ignore:
                        return;
                    case System.Windows.Forms.DialogResult.Cancel:
                        DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        break;
                    case System.Windows.Forms.DialogResult.Retry:
                        //TODO: Report it 
                        return;
                }
            }

            this.Close();
        }

        void OnPluginLoad_All(IPlugin sender, API.Events.PluginLoadEventArgs args) {
            DrawText("Loaded: " + sender.Name);
        }

        void OnCommandLoad_All(ICommand sender, API.Events.CommandLoadEventArgs args) {
            DrawText("Loaded: " + sender.Name);
        }

        #endregion

        #region Utils

        private string GenerateDevList() {
            var devList = MCForge.Core.Server.Devs;
            var compiledString = "";

            float curr = Constants.LOGO_WIDTH + Constants.PADDING;

            using ( var g = CreateGraphics() )
                for ( int i = 0; i < devList.Length; i++ ) {

                    float len = g.MeasureString(devList[i], DrawingFont).Width;

                    if ( curr + len + 30 >= Width - Constants.PADDING ) {
                        compiledString += devList[i] + ",\n";
                        curr = ( Constants.LOGO_WIDTH + Constants.PADDING );
                    }
                    else {
                        compiledString += devList[i] + ", ";
                        curr += len;
                    }
                }

            return compiledString;
        }

        #endregion




    }
}
