using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Core.RelayChat
{
    public class GlobalChat : IRC
    {
        private string gcServer = "irc.geekshed.net";
        private int gcPort = 6667;
        private string gcNick = "";
        private string gcChannel = "#mcforge";

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
    }
}
