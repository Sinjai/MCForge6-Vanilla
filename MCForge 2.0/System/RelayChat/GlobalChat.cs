using MCForge.Entity;
using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
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
            gc = this;

            string gcban = "";

            try
            {
                using (WebClient c = new WebClient()) // .dev is local testing domain, this will change once the php code is pushed live
                    gcban = c.DownloadString("http://mcforge.dev/gcbans/get"); 
            }
            catch (Exception e) { }

            this.bans = gcban.Split(',');
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
