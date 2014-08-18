using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using MCForge.Gui.Components;
using MCForge.Gui.Components.Interfaces;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interfaces;
using MCForge.Utils;
using MCForge.Gui.Dialogs;
using System.Diagnostics;
using MCForge.Core;
using MCForge.World;
using System.Reflection;
using MCForge.Core.RelayChat;

namespace MCForge.Gui.Forms {
    public partial class FormMainScreen : AeroForm, IFormSharer {

        public FormMainScreen() {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e) {
            if ( !Natives.CanUseAero ) {
                base.OnPaint(e);
                return;
            }
        }

        #region Gui Event Handlers


        private void FormMainScreen_Load(object sender, EventArgs e) {
            this.GlassArea = new Rectangle {
                X = 0,
                Y = glassMenu.Height,
                Width = 0,
                Height = 0
            };

            Invalidate();

            Logger.OnRecieveLog += new EventHandler<LogEventArgs>(Logger_OnRecieveLog);
            Logger.OnRecieveErrorLog += new EventHandler<ErrorLogEventArgs>(Logger_OnRecieveErrorLog);
            Player.OnAllPlayersConnect.Normal += new API.Events.Event<Player, API.Events.ConnectionEventArgs>.EventHandler(OnAllPlayersConnect_Normal);
            Player.OnAllPlayersDisconnect.Normal += new API.Events.Event<Player, API.Events.ConnectionEventArgs>.EventHandler(OnAllPlayersDisconnect_Normal);
            Level.OnAllLevelsLoad.Normal += new API.Events.Event<Level, API.Events.LevelLoadEventArgs>.EventHandler(OnAllLevelsLoad_Normal);
            Level.OnAllLevelsUnload.Normal += new API.Events.Event<Level, API.Events.LevelLoadEventArgs>.EventHandler(OnAllLevelsUnload_Normal);

            foreach ( var level in Level.Levels )
                lstLevels.Items.Add(level.Name);

            foreach ( var player in Server.Players )
                lstPlayers.Items.Add(player.Color ?? '0' + player.Username);


            Logger.Log("&0MCForge Version: &7" + Server.ServerAssembly.GetName().Version + "  &0Start Time: &7" + DateTime.Now.ToString("T"));

#if DEBUG
            Logger.Log("&6Warning: Running MCForge in Debug mode. Results may vary.");
#endif

            if ( !String.IsNullOrWhiteSpace(Server.URL) )
                Logger.Log(Server.URL);

        }

        private void FormMainScreen_Shown(object sender, EventArgs e) {
            /*if ( GuiSettings.GetSettingBoolean(GuiSettings.SHOW_NEWS_KEY) )
                using ( var news = new NewsDialog() )
                    news.ShowDialog();*/
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e) {

            if ( e.KeyCode == Keys.Enter ) {

                if (txtMessage.Text.StartsWith("/"))
                {
                    ICommand cmd = null;

                    var commandSplit = txtMessage.Text.Remove(0, 1).Split(' ');
                    var args = commandSplit.Where((val, index) => index != 0).ToArray();
                    cmd = Command.Find(commandSplit[0]);

                    if (cmd == null)
                    {
                        Logger.Log("Command not found!");
                        txtMessage.Text = "";
                        return; // cannot run the command
                    }
                    if (cp == null)
                        cp = new ConsolePlayer(cio);
                    try
                    {
                        cmd.Use(cp, args);
                    }
                    catch (Exception ex) { Logger.LogError(ex); Logger.Log("Command aborted"); }
                    Logger.Log("CONSOLE used: /" + commandSplit[0]);
                    txtMessage.InHintState = true;
                    return;
                }

                if ( String.IsNullOrWhiteSpace(txtMessage.Text) ) {
                    Logger.Log("&4Please specify a valid message!");
                    txtMessage.InHintState = true;
                    return;
                }

                if ( cmbChatType.Text == "OpChat" ) {
                    Player.UniversalChatOps("&a<&fTo Ops&a> [&fConsole&a]:&f " + txtMessage.Text);
                    Logger.Log("<OpChat> &5[&1Console&5]: &1" + txtMessage.Text);
                    txtMessage.InHintState = true;
                }

                else if ( cmbChatType.Text == "AdminChat" ) {
                    Player.UniversalChatAdmins("&a<&fTo Admins&a> [&fConsole&a]:&f " + txtMessage.Text);
                    Logger.Log("<AdminChat> &5[&1Console&5]: &1" + txtMessage.Text);
                    txtMessage.InHintState = true;
                }
                else if (cmbChatType.Text == "GlobalChat")
                {
                    Server.GC.SendConsoleMessage(txtMessage.Text);
                    Logger.Log("<GC> &0[&2Console&0]: " + txtMessage.Text);
                    txtMessage.Text = "";
                }
                else {
                    Server.IRC.SendConsoleMessage(txtMessage.Text);
                    Player.UniversalChat("&a[&fConsole&a]:&f " + txtMessage.Text);
                    Logger.Log("&0[&2Console&0]: " + txtMessage.Text);
                    txtMessage.InHintState = true;
                }
            }
            else if ( e.KeyCode == Keys.Down ) {
                cmbChatType.SelectedIndex = cmbChatType.SelectedIndex + ( cmbChatType.Items.Count == 1 ? 0 : cmbChatType.SelectedIndex + 1 );
            }
            else if ( e.KeyCode == Keys.Up ) {
                cmbChatType.SelectedIndex = ( cmbChatType.SelectedIndex == 0 ? cmbChatType.Items.Count - 1 : cmbChatType.SelectedIndex - 1 );
            }

        }

        #endregion

        #region MCForge Lib Event Handlers

        //Invoke checks are required for each event handler

        void OnAllLevelsUnload_Normal(Level sender, API.Events.LevelLoadEventArgs args) {
            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { OnAllLevelsUnload_Normal(sender, args); });
                return;
            }

            if ( lstLevels.Items.Contains(sender.Name) )
                lstLevels.Items.Remove(sender.Name);

        }

        void OnAllLevelsLoad_Normal(Level sender, API.Events.LevelLoadEventArgs args) {
            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { OnAllLevelsLoad_Normal(sender, args); });
                return;
            }

            if ( !lstLevels.Items.Contains(sender.Name) )
                lstLevels.Items.Add(sender.Name);

        }

        void OnAllPlayersDisconnect_Normal(Player sender, API.Events.ConnectionEventArgs args) {
            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { OnAllPlayersDisconnect_Normal(sender, args); });
                return;
            }

            lstPlayers.RemoveIfExists(sender.Username);
        }

        void OnAllPlayersConnect_Normal(Player sender, API.Events.ConnectionEventArgs args) {
            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { OnAllPlayersConnect_Normal(sender, args); });
                return;
            }

            char color = '0';

            if ( !String.IsNullOrWhiteSpace(sender.Color) && sender.Color.Length == 2 ) {
                color = sender.Color[1];
            }
            else {
                color = sender.Group.Color[1];
            }

            lstPlayers.AddIfNotExist(color, sender.Username);
        }

        void Logger_OnRecieveErrorLog(object sender, ErrorLogEventArgs e) {
            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { Logger_OnRecieveErrorLog(sender, e); });
                return;
            }

            txtLog.AppendLog("&4\t------[Error]-----" + Environment.NewLine);
            txtLog.AppendLog("&4\t" + e.Message + Environment.NewLine);
            txtLog.AppendLog(Environment.NewLine);
        }

        void Logger_OnRecieveLog(object sender, LogEventArgs e) {
            if ( InvokeRequired ) {
                BeginInvoke((MethodInvoker)delegate { Logger_OnRecieveLog(sender, e); });
                return;
            }

            txtLog.AppendLog(e.Message + Environment.NewLine);
        }

        #endregion

        #region IFormSharer Members

        public Form FormToShare {
            get { return this; }
        }

        #endregion

        #region Colored Reader Context Menu

        private void nightModeToolStripMenuItem_Click(object sender, EventArgs e) {
            if ( MessageBox.Show("Changing to and from night mode will clear your logs. Do you still want to change?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No )
                return;

            txtLog.NightMode = nightModeToolStripMenuItem.Checked;
            nightModeToolStripMenuItem.Checked = !nightModeToolStripMenuItem.Checked;
        }

        private void colorsToolStripMenuItem_Click(object sender, EventArgs e) {
            txtLog.Colorize = !colorsToolStripMenuItem.Checked;
            colorsToolStripMenuItem.Checked = !colorsToolStripMenuItem.Checked;

        }

        private void dateAndTimeToolStripMenuItem_Click(object sender, EventArgs e) {
            txtLog.DateStamp = !dateAndTimeToolStripMenuItem.Checked;
            dateAndTimeToolStripMenuItem.Checked = !dateAndTimeToolStripMenuItem.Checked;
        }

        private void copyLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText(txtLog.SelectedText, TextDataFormat.Text);
        }

        private void clearLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            if ( MessageBox.Show("Are you sure you want to clear logs?", "You sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes ) {
                txtLog.Clear();
            }
        }
        #endregion

        #region Main Menu

        private void portToolsToolStripMenuItem_Click(object sender, EventArgs e) {
            using ( var pTools = new PortToolsDialog() )
                pTools.ShowDialog();
        }


        private void visitForumsToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://mcforge.org/forums/");
        }

        private void reportAProblemToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("http://mcforge.org/forums/viewforum.php?f=8");
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/MCForge/MCForge7-Vanilla");
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
            Server.Restart();
        }

        private void playerManagerToolStripMenuItem_Click(object sender, EventArgs e) {
            using ( var dialog = new PlayerManagerDialog() )
                if ( dialog.ShowDialog() == System.Windows.Forms.DialogResult.Yes ) {
                    //TODO: Save
                }
        }


        private void mapManagerToolStripMenuItem_Click(object sender, EventArgs e) {
            using ( var maps = new MapManagerDialog() )
                maps.ShowDialog();
        }

        private void newsToolStripMenuItem_Click(object sender, EventArgs e) {
            using ( var news = new NewsDialog() )
                news.ShowDialog();
        }

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e) {
            try
            {
                Process.Start("http://mcforge.org/changelog");
            }
            catch { }
        }

        private void makerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (CommandMaker dialog = new CommandMaker())
                dialog.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ServerSettingsDialog settings = new ServerSettingsDialog())
                settings.ShowDialog();
        }

        #endregion


        private static ConsolePlayer cp;
        private static CommandIO cio = new CommandIO();
        class CommandIO : IIOProvider
        {
            public string ReadLine()
            {
                Console.Write("CIO input: ");
                return Console.ReadLine();
            }

            public void WriteLine(string line)
            {
                Logger.Log("CIO output: " + line);
            }

            public void WriteLine(string line, string replyChannel) {}
        }
    }
}
