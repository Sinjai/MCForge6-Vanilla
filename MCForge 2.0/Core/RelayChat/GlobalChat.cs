/*
    Copyright 2014 UclCommander
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using MCForge.Core.RelayChat.Core;
using MCForge.Entity;
using MCForge.Utils;
using MCForge.Utils.Settings;
using System;
using System.Linq;
using System.Net;

namespace MCForge.Core.RelayChat
{
    public class UniversalChat
    {
        private Connection connection;
        private string gcServer = "irc.geekshed.net";
        private int gcPort = 6667;
        private string gcNick = "";
        private string gcChannel = "#mcforge";
        private string[] bans = null;

        /// <summary>
        /// 
        /// </summary>
        public UniversalChat()
        {
            this.connection = new Connection();
            ServerSettings.OnSettingChanged += ServerSettings_OnSettingChanged;

            string gcban = "";

            try
            {
                using (WebClient c = new WebClient())
                    gcban = c.DownloadString("http://mcforge.org/gcban/get"); 
            }
            catch (Exception e) { Logger.LogError(e); }

            this.bans = gcban.Split(',');

            //Register all the commands
            //------------------------------------------------------------------------------------------
            this.connection.RegisterCommand("GETINFO", new Connection.IRCCommand(delegate(string[] cmd) 
            {
                if (cmd.Length == 0)
                    return;

                if(cmd[1] == this.gcNick) 
                {
                    this.connection.SendMessage("^NAME: " + ServerSettings.GetSetting("servername"));
                    this.connection.SendMessage("^MOTD: " + ServerSettings.GetSetting("motd"));
                    this.connection.SendMessage("^VERSION: MCForge 7"); // TODO: GET THIS FROM THE SERVER
                    this.connection.SendMessage("^URL: " + Server.URL);
                    this.connection.SendMessage("^PLAYERS: " + Server.PlayerCount + "/" + ServerSettings.GetSetting("maxplayers"));
                }
            }));
            //------------------------------------------------------------------------------------------
            this.connection.RegisterCommand("PLAYERS", new Connection.IRCCommand(delegate(string[] cmd) 
            {
                if (cmd.Length == 0)
                    return;

                if(cmd[1] == this.gcNick)
                    this.connection.SendMessage("^PLAYERS: " + String.Join(",", Server.Players.Select(p => p.Username).ToArray()));
            }));
            //------------------------------------------------------------------------------------------
            this.connection.RegisterCommand("IPGET", new Connection.IRCCommand(delegate(string[] cmd)
            {
                if (cmd.Length == 0)
                    return;

                Player p = Player.Find(cmd[1]);

                if (p == null)
                    return;

                this.connection.SendMessage("^IP FOR " + cmd[1] + ": " + p.Ip);
            }));
            //------------------------------------------------------------------------------------------
            //hardy har harr Aviators reference
            //http://music.soundoftheaviators.com/track/ghosts-in-the-code
            //for robodash/dan... maybe, no idea
            this.connection.RegisterCommand("AMIAMANORANAUTOMATION", new Connection.IRCCommand(delegate(string[] cmd)
            {
                if (cmd.Length == 0)
                    return;

                if (cmd[1] == this.gcNick)
                    this.connection.SendMessage("^IAMANAUTOMATION");
            }));
            //------------------------------------------------------------------------------------------
            //YES, I DID THIS. SHUT UP. I DON'T WANT TO HEAR IT. NOBODY CARES.
            this.connection.RegisterCommand("^", new Connection.IRCCommand(delegate(string[] cmd) { }));
        }

        public void Stop()
        {
            this.connection.Stop();
        }

        void ServerSettings_OnSettingChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.Key == "gc-nick")
            {
                this.gcNick = e.NewValue;
                this.connection.SetNick(e.NewValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Connect()
        {
            if (ServerSettings.GetSetting("GC-Nick") == "")
            {
                this.gcNick = "MCF7-" + (new Random()).Next(100000, 999999);
                ServerSettings.SetSetting("GC-Nick", "Global Chat Nickname", this.gcNick);
                ServerSettings.Save();
            }
            else
                this.gcNick = ServerSettings.GetSetting("GC-Nick");

            this.connection.Type = "GC";
            this.connection.Setup(this.gcServer, this.gcPort, this.gcNick, this.gcChannel);
            this.connection.Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="message"></param>
        public void SendMessage(Player p, string message)
        {
            if (this.bans.Contains(p.Username))
            {
                p.SendMessage("You have been GBBanned. Post an appeal @ MCForge.org");
                return;
            }

            this.connection.SendMessage(String.Format("[{0}] {1}", p.Username, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="message"></param>
        public void SendConsoleMessage(string message)
        {
            this.connection.SendMessage(String.Format("[Console] {0}", message));
        }
    }
}
