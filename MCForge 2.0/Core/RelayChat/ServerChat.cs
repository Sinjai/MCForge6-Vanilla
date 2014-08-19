using MCForge.Core.RelayChat.Core;
using MCForge.Utils.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Core.RelayChat
{
    public class ServerChat
    {
        private Connection connection;

        public void Connect()
        {
            this.connection = new Connection();
            this.connection.Type = "IRC";
           
            string server = ServerSettings.GetSetting("IRC-Server");
            int port = ServerSettings.GetSettingInt("IRC-Port");
            string nickname = ServerSettings.GetSetting("IRC-Nickname");
            string channel = ServerSettings.GetSetting("IRC-Channel");
            string opChannel = ServerSettings.GetSetting("IRC-OPChannel");
            string password = ServerSettings.GetSetting("IRC-NickServ");

            if (nickname == "" || server == "" || channel == "" || port == -1 || !ServerSettings.GetSettingBoolean("IRC-Enabled"))
                return;

            this.connection.Setup(server, port, nickname, channel, password, opChannel);
            this.connection.Run();
        }

        public void Stop()
        {
            this.connection.Stop();
        }

        public void SendOperatorMessage(string message)
        {
            if (this.connection.OpChannel != null)
                this.connection.SendMessage(this.connection.OpChannel, message);
        }

        public void SendMessage(string message)
        {
            this.connection.SendMessage(message);
        }

        public void SendConsoleMessage(string message)
        {
            this.connection.SendMessage(String.Format("[Console] {0}", message));
        }
    }
}
