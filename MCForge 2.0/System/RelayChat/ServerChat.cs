using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Core.RelayChat
{
    public class ServerChat : IRC
    {
        public void Connect()
        {
            string server = ServerSettings.GetSetting("IRC-Server");
            int port = ServerSettings.GetSettingInt("IRC-Port");
            string nickname = ServerSettings.GetSetting("IRC-Nickname");
            string channel = ServerSettings.GetSetting("IRC-Channel");
            string opChannel = ServerSettings.GetSetting("IRC-OPChannel");
            string password = ServerSettings.GetSetting("IRC-NickServ");

            if (nickname == "" || server == "" || channel == "" || port == -1 || !ServerSettings.GetSettingBoolean("IRC-Enabled"))
                return;

            this.Start(server, port, nickname, channel, password, opChannel);
        }
    }
}
