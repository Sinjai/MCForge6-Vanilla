using MCForge.Entity;
using MCForge.Utils;
using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MCForge.Core.RelayChat
{
    public class GlobalChat : IRC
    {
        private static GlobalChat gc;

        private string gcServer = "irc.geekshed.net";
        private int gcPort = 6667;
        private string gcNick = "";
        private string gcChannel = "#mcforge";
        private string[] bans = null;

        /// <summary>
        /// 
        /// </summary>
        public GlobalChat()
        {
            ServerSettings.OnSettingChanged += ServerSettings_OnSettingChanged;
            gc = this;

            string gcban = "";

            try
            {
                using (WebClient c = new WebClient()) // .dev is local testing domain, this will change once the php code is pushed live
                    gcban = c.DownloadString("http://mcforge.org/gcban/get"); 
            }
            catch (Exception e) { Logger.LogError(e); }

            this.bans = gcban.Split(',');

            this.AddCommand("^GETINFO", new IRCCommand(delegate(string[] cmd) 
            {
                if(cmd[1] == this.gcNick) 
                {
                    this.SendMessage("^NAME: " + ServerSettings.GetSetting("servername"));
                    this.SendMessage("^MOTD: " + ServerSettings.GetSetting("motd"));
                    this.SendMessage("^VERSION: MCForge 7"); // TODO: GET THIS FROM THE SERVER
                    this.SendMessage("^URL: " + Server.URL);
                    this.SendMessage("^PLAYERS: " + Server.PlayerCount + "/" + ServerSettings.GetSetting("maxplayers"));
                }
            }));

            this.AddCommand("^PLAYERS", new IRCCommand(delegate(string[] cmd) 
            {
                if(cmd[1] == this.gcNick)
                    this.SendMessage("^PLAYERS: " + String.Join(",", Server.Players.Select(p => p.Username).ToArray()));
            }));
        }

        void ServerSettings_OnSettingChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.Key == "gc-nick")
            {
                this.gcNick = e.NewValue;
                base.SetNick(e.NewValue);
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

            this.type = "GC";
            this.Start(this.gcServer, this.gcPort, this.gcNick, this.gcChannel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="message"></param>
        public static void SendMessage(Player p, string message)
        {
            if(gc.bans.Contains(p.Username))
            {
                p.SendMessage("You have been GBBanned. Post an appeal @ MCForge.org");
                return;
            }

            gc.SendMessage(String.Format("[{0}] {1}", p.Username, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="message"></param>
        public static void SendConsoleMessage(string message)
        {
            gc.SendMessage(String.Format("[Console] {0}", message));
        }
    }
}
