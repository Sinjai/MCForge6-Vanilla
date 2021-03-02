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

using MCForge.Entity;
using MCForge.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace MCForge.Core.RelayChat.Core
{
    public class Connection
    {
        public delegate void IRCCommand(string[] message);

        public string Nick { get; private set; }
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string Channel { get; private set; }
        public string OpChannel { get; private set; }

        /// <summary>
        /// Command Starter, Default is ^
        /// </summary>
        public string CommandStarter { get; set; }

        /// <summary>
        /// Sets the IRC Type
        /// </summary>
        public string Type { get; set; }

        private string password = null;
        private TcpClient socket;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread socketThread;
        private Dictionary<string, IRCCommand> commands = new Dictionary<string, IRCCommand> { };
        private bool isRunning = false;
        private bool isErrored = false;
        private bool isConnected = false;
        private bool nickServId = false;
        private int connectError = 0;

        /// <summary>
        /// Sets up the connection
        /// </summary>
        /// <param name="server">Server Address</param>
        /// <param name="port">Port (normally 6667)</param>
        /// <param name="nick">Bot Nickname</param>
        /// <param name="channel">Default Channel</param>
        /// <param name="password">Bot NickServ Password</param>
        /// <param name="opchannel">Operator Channel</param>
        public void Setup(string server, int port, string nick, string channel, string password = null, string opchannel = null)
        {
            this.CommandStarter = "^";

            this.Server = server;
            this.Port = port;
            this.Nick = nick;
            this.Channel = channel;
            this.password = password;
            this.OpChannel = opchannel;
        }

        /// <summary>
        /// Registers an IRC Command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cmd"></param>
        public void RegisterCommand(string name, IRCCommand cmd)
        {
            this.commands.Add(name, cmd);
        }

        /// <summary>
        /// Runs the IRC bot
        /// </summary>
        public void Run()
        {
            try
            {
                this.socket = new TcpClient(this.Server, this.Port);
                this.reader = new StreamReader(this.socket.GetStream());
                this.writer = new StreamWriter(this.socket.GetStream());
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            this.socketThread = new Thread(new ThreadStart(delegate()
            {
                this.Read();
            }));

            this.socketThread.Start();
        }

        private void Read()
        {
            string line;
            this.isRunning = true;

            Logger.Log("Connecting to IRC...");

            try
            {

                this.SendUserInfo();

                while(this.isRunning)
                {
                    line = this.reader.ReadLine();

                    if (line == null)
                        return;

                    List<object> data = IrcHelper.Parse(line, this);
                    List<string> cmd = (List<string>)data[0];
                    User user = (User)data[1];

                    if (cmd[0] == "ERROR") // DEATH
                    {
                        if (!this.isConnected)
                            this.connectError++;

                        this.isRunning = false;
                        this.isErrored = true;
                        this.isConnected = false;

                        //TODO: ATTEMPT RECONNECT
                    }

                    switch (cmd[0])
                    {
                        case "004": // registered with server
                            this.isConnected = true;
                            this.connectError = 0;

                            // do we have a password?
                            if (this.password != null)
                            {
                                this.SendMessage("NickServ", "IDENTIFY " + this.password);
                                this.nickServId = true;
                            }

                            // FOR GEEKSHED SERVERS: I AM A BOT. NOT A MAN.
                            this.SendRaw("MODE " + this.Nick + " +B");

                            // Join channels
                            this.SendRaw("JOIN " + this.Channel);

                            if(this.OpChannel != null)
                                this.SendRaw("JOIN " + this.OpChannel);

                            Logger.Log("Connected to IRC!");
                            break;

                        case "433":     // nick already in use
                            //TODO
                            break;

                        case "473":     // cannot join channel
                            //TODO
                            break;

                        case "PING":
                            this.SendRaw("PONG " + cmd[1]);
                            break;

                        case "PRIVMSG":
                            if (cmd[1] == this.Nick)
                            {
                                //TODO
                            }

                            if (cmd[1].StartsWith("#"))
                            {
                                if(cmd[2].StartsWith(this.CommandStarter))
                                {
                                    string c = cmd[2].Remove(0, 1);

                                    if(this.commands.ContainsKey(c))
                                    {
                                        this.commands[c](cmd[2].Split(' ').Skip(1).ToArray());
                                    }
                                }
                                else
                                {
                                    if (cmd[2][0] == 0x001 && cmd[2][cmd[2].Length - 1] == 0x001)
                                    {
                                        this.SendActionToGame(user, cmd[2].Substring(7).Trim());
                                    }
                                    else
                                    {
                                        this.SendMessageToGame(user, cmd[2]);
                                    }
                                }
                            }
                            break;

                        case "KICK":

                            break;

                        case "NICK":

                            break;

                        case "PART":

                            break;

                        case "JOIN":

                            break;
                    }

                }
            } 
            catch(Exception e)
            {
                Logger.LogError(e);
            }
        }

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        public void Stop()
        {
            this.isRunning = false;
            this.isConnected = false;

            this.socket.Close();
            this.reader.Close();
            this.reader.Dispose();
            this.writer.Close();
            this.writer.Dispose();

            this.socketThread.Abort();
        }

        /// <summary>
        /// Sends an unformatted message to game
        /// </summary>
        /// <param name="message"></param>
        public void SendToGame(string message)
        {
            try { 
                Player.UniversalChat(message);
                Logger.Log(message);
            }
            catch { }
        }

        /// <summary>
        /// Sends an action (/me) message to game
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public void SendActionToGame(User user, string message)
        {
            this.SendToGame(String.Format("[{0}] *{1} {2}", this.Type, user.Nick, IrcHelper.IRCToClassic(message)));
        }

        /// <summary>
        /// Sends a normal message to game
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public void SendMessageToGame(User user, string message)
        {
            this.SendToGame(String.Format("[{0}] <{1}>: {2}", this.Type, user.Nick, IrcHelper.IRCToClassic(message)));
        }

        /// <summary>
        /// Sends USER and NICK commands
        /// </summary>
        private void SendUserInfo()
        {
            this.SendRaw(String.Format("USER {0} {1} * :{2}", this.Nick, 0, "MCForge IRC Bot"));
            this.SetNick(this.Nick);
        }

        /// <summary>
        /// Sends a message to <location>
        /// </summary>
        /// <param name="location">Channel or username</param>
        /// <param name="message">message</param>
        public void SendMessage(string location, string message)
        {
            foreach (string ln in message.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                this.SendRaw("PRIVMSG " + location + " :" + ln);
        }

        /// <summary>
        /// Sends a message to the default channel
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            foreach (string ln in message.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                this.SendRaw("PRIVMSG " + this.Channel + " :" + ln);
        }

        /// <summary>
        /// Sends a _RAW_ IRC Line. Do not include line endings.
        /// USE ONLY IF YOU HAVE TO
        /// </summary>
        /// <param name="raw"></param>
        public void SendRaw(string raw)
        {
            if (!this.isRunning) //Eenope
                return;

            try
            {
                this.writer.Write(raw + "\r\n");
                this.writer.Flush();
            }
            catch (IOException e)
            {
                Logger.LogError(e);
            }
        }

        /// <summary>
        /// Sets a new nickname
        /// </summary>
        /// <param name="nick"></param>
        public void SetNick(string nick)
        {
            this.Nick = nick;
            this.SendRaw("NICK " + nick);
        }
    }
}
