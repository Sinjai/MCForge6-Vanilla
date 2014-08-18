/*
Copyright 2011 MCForge
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MCForge.API.Events;
using MCForge.Core;
using MCForge.Groups;
using MCForge.Robot;
using MCForge.SQL;
using MCForge.Utils;
using MCForge.Utils.Settings;
using MCForge.World;
using MCForge.Core.RelayChat;

namespace MCForge.Entity
{
    public partial class Player : Sender
    {
        private static readonly char[] UnicodeReplacements = " ☺☻♥♦♣♠•◘○\n♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼".ToCharArray();

        /// <summary> List of chat keywords, and emotes that they stand for. </summary>
        public static readonly Dictionary<string, char> EmoteKeywords = new Dictionary<string, char> {
            { "smile", '\u0001' },

			{ "darksmile", '\u0002' }, // ☻

            { "heart", '\u0003' }, // ♥
            { "hearts", '\u0003' },

            { "diamond", '\u0004' }, // ♦
            { "diamonds", '\u0004' },
            { "rhombus", '\u0004' },

            { "club", '\u0005' }, // ♣
            { "clubs", '\u0005' },
            { "clover", '\u0005' },
            { "shamrock", '\u0005' },

            { "spade", '\u0006' }, // ♠
            { "spades", '\u0006' },

            { "*", '\u0007' }, // •
            { "bullet", '\u0007' },
            { "dot", '\u0007' },
            { "point", '\u0007' },

            { "hole", '\u0008' }, // ◘

            { "circle", '\u0009' }, // ○
            { "o", '\u0009' },

            { "male", '\u000B' }, // ♂
            { "mars", '\u000B' },

            { "female", '\u000C' }, // ♀
            { "venus", '\u000C' },

            { "8", '\u000D' }, // ♪
            { "note", '\u000D' },
            { "quaver", '\u000D' },

            { "notes", '\u000E' }, // ♫
            { "music", '\u000E' },

            { "sun", '\u000F' }, // ☼
            { "celestia", '\u000F' },

            { ">>", '\u0010' }, // ►
            { "right2", '\u0010' },

            { "<<", '\u0011' }, // ◄
            { "left2", '\u0011' },

            { "updown", '\u0012' }, // ↕
            { "^v", '\u0012' },

            { "!!", '\u0013' }, // ‼

            { "p", '\u0014' }, // ¶
            { "para", '\u0014' },
            { "pilcrow", '\u0014' },
            { "paragraph", '\u0014' },

            { "s", '\u0015' }, // §
            { "sect", '\u0015' },
            { "section", '\u0015' },

            { "-", '\u0016' }, // ▬
            { "_", '\u0016' },
            { "bar", '\u0016' },
            { "half", '\u0016' },

            { "updown2", '\u0017' }, // ↨
            { "^v_", '\u0017' },

            { "^", '\u0018' }, // ↑
            { "up", '\u0018' },

            { "v", '\u0019' }, // ↓
            { "down", '\u0019' },

            { ">", '\u001A' }, // →
            { "->", '\u001A' },
            { "right", '\u001A' },

            { "<", '\u001B' }, // ←
            { "<-", '\u001B' },
            { "left", '\u001B' },

            { "l", '\u001C' }, // ∟
            { "angle", '\u001C' },
            { "corner", '\u001C' },

            { "<>", '\u001D' }, // ↔
            { "<->", '\u001D' },
            { "leftright", '\u001D' },

            { "^^", '\u001E' }, // ▲
            { "up2", '\u001E' },

            { "vv", '\u001F' }, // ▼
            { "down2", '\u001F' },

            { "house", '\u007F' } // ⌂
        };

        public static string ReplaceEmoteKeywords(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            int startIndex = message.IndexOf('(');
            if (startIndex == -1)
            {
                return message; // break out early if there are no opening braces
            }

            StringBuilder output = new StringBuilder(message.Length);
            int lastAppendedIndex = 0;
            while (startIndex != -1)
            {
                int endIndex = message.IndexOf(')', startIndex + 1);
                if (endIndex == -1)
                {
                    break; // abort if there are no more closing braces
                }

                // see if emote was escaped (if odd number of backslashes precede it)
                bool escaped = false;
                for (int i = startIndex - 1; i >= 0 && message[i] == '\\'; i--)
                {
                    escaped = !escaped;
                }
                // extract the keyword
                string keyword = message.Substring(startIndex + 1, endIndex - startIndex - 1);
                char substitute;
                if (EmoteKeywords.TryGetValue(keyword.ToLowerInvariant(), out substitute))
                {
                    if (escaped)
                    {
                        // it was escaped; remove escaping character
                        startIndex++;
                        output.Append(message, lastAppendedIndex, startIndex - lastAppendedIndex - 2);
                        lastAppendedIndex = startIndex - 1;
                    }
                    else
                    {
                        // it was not escaped; insert substitute character
                        output.Append(message, lastAppendedIndex, startIndex - lastAppendedIndex);
                        output.Append(substitute);
                        startIndex = endIndex + 1;
                        lastAppendedIndex = startIndex;
                    }
                }
                else
                {
                    startIndex++; // unrecognized macro, keep going
                }
                startIndex = message.IndexOf('(', startIndex);
            }
            // append the leftovers
            output.Append(message, lastAppendedIndex, message.Length - lastAppendedIndex);
            return output.ToString();
        }


        private static readonly Regex EmoteSymbols = new Regex("[\x00-\x1F\x7F☺☻♥♦♣♠•◘○\n♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼⌂]");
        #region Incoming Data
        private static void Incoming(IAsyncResult result)
        {
            while (!Server.Started)
                Thread.Sleep(100);

            Player p = (Player)result.AsyncState;

            try
            {
                int length = p.Socket.EndReceive(result);
                if (length == 0) {
                    p.CloseConnection();
                    if (!p.IsBeingKicked) {
                        UniversalChat(p.Color + p.Username + Server.DefaultColor + " has disconnected.");
                        p.GlobalDie();
                    }
                    if (Server.ReviewList.Contains(p)) {
                        Server.ReviewList.Remove(p);
                        foreach (Player pl in Server.ReviewList.ToArray()) {
                            int position = Server.ReviewList.IndexOf(pl);
                            if (position == 0) { pl.SendMessage("You're next in the review queue!"); continue; }
                            pl.SendMessage(position == 1 ? "There is 1 player in front of you!" : "There are " + position + " players in front of you!");
                        }
                    }
                    return;
                }
                else {
                    p.lastReceived = DateTime.Now;
                }


                byte[] b = new byte[p.buffer.Length + length];
                Buffer.BlockCopy(p.buffer, 0, b, 0, p.buffer.Length);
                Buffer.BlockCopy(p.tempBuffer, 0, b, p.buffer.Length, length);
                p.buffer = p.HandlePacket(b);
                p.Socket.BeginReceive(p.tempBuffer, 0, p.tempBuffer.Length, SocketFlags.None, new AsyncCallback(Incoming), p);
            }
            catch (SocketException)
            {
                p.CloseConnection();
                return;
            }
            catch (ObjectDisposedException)
            {
                p.CloseConnection();
                return;
            }
            catch (Exception e)
            {
                p.Kick("Error!");
                Logger.LogError(e);
                return;
            }
        }
        private byte[] HandlePacket(byte[] buffer)
        {

            try
            {
                int length = 0; byte msg = buffer[0];
                // Get the length of the message by checking the first byte
                switch (msg)
                {
                    case 0: length = 130; break; // login
                    case 2: SMPKick("This is not an SMP Server!"); break; // SMP Handshake packet
                    case 5: length = 8; break; // blockchange
                    case 8: length = 9; break; // input
                    case 13: length = 65; break; // chat
                    case 16: length = 66; break;
                    case 17: length = 68; break;
                    case 19: length = 1; break;
                    default:
                        {
                            PacketEventArgs args = new PacketEventArgs(buffer, true, (Packet.Types)msg);
                            bool canceled = OnPlayerReceiveUnknownPacket.Call(this, args, OnAllPlayersReceiveUnknownPacket).Canceled;
                            if (canceled)
                                return new byte[1];
                            Kick("Unhandled message id \"" + msg + "\"!");
                            return new byte[0];
                        }

                }
                if (buffer.Length > length)
                {
                    byte[] message = new byte[length];
                    Buffer.BlockCopy(buffer, 1, message, 0, length);

                    byte[] tempbuffer = new byte[buffer.Length - length - 1];
                    Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

                    buffer = tempbuffer;
                    if (!OnPlayerReceivePacket.Call(this, new PacketEventArgs(message, true, (Packet.Types)msg), OnAllPlayersReceivePacket).Canceled)
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            switch (msg)
                            {
                                case 0: HandleLogin(message); break;
                                case 5: HandleBlockchange(message); break;
                                case 8: HandleIncomingPos(message); break;
                                case 13: HandleChat(message); break;
                                case 16: HandleExtInfo(message); break;
                                case 17: HandleExtEntry(message); break;
                                case 19: HandleCustomBlockSupportLevel(message); break;
                            }
                        });
                    }

                    if (buffer.Length > 0)
                        buffer = HandlePacket(buffer);
                    else
                        return new byte[0];


                }
            }
            catch (Exception e)
            {
                Kick("CONNECTION ERROR: (0x03)");
                Logger.Log("[ERROR]: PLAYER MESSAGE RECEIVE ERROR (0x03)", System.Drawing.Color.Red, System.Drawing.Color.Black);
                Logger.LogError(e);
            }
            return buffer;
        }

        public void HandleExtInfo(byte[] message)
        {
            appName = enc.GetString(message, 0, 64).Trim();
            extensionCount = message[65];
        }

        void HandleExtEntry(byte[] msg)
        {
            CPE tmp; tmp.name = enc.GetString(msg, 0, 64);
            tmp.version = BitConverter.ToInt32(msg, 64);
            ExtEntry.Add(tmp);
        }

        public void HandleCustomBlockSupportLevel(byte[] message)
        {
            customBlockSupportLevel = 1;
        }

        private void HandleLogin(byte[] message)
        {
            try
            {
                if (IsLoggedIn) return;
                byte version = message[0];
                Username = enc.GetString(message, 1, 64).Trim();
                _displayName = Username;
                string BanReason = null;
                bool banned = false;

                foreach (string line in File.ReadAllLines("bans/BanInfo.txt")) { if (Username == line.Split('`')[0]) { BanReason = line.Split('`')[1]; } }
                foreach (string line in Server.IPBans) { if (line == Ip) banned = true; }
                foreach (string line in Server.UsernameBans) { if (line == Username) banned = true; }
                if (banned) { if (BanReason == "No reason given.") { SKick("You are banned because " + BanReason); } else { SKick("You are banned!"); } }
                string verify = enc.GetString(message, 65, 32).Trim();
                if (!VerifyAccount(Username, verify)) return;
                if (Server.Verifying) IsVerified = false;
                else IsVerified = true;
                if (version != ServerSettings.Version) { SKick("Wrong Version!"); return; }
                try
                {
                    Server.TempBan tb = Server.TempBansList.Find(ban => ban.name.ToLower() == Username.ToLower());
                    if (DateTime.Now > tb.allowed)
                    {
                        Server.TempBansList.Remove(tb);
                    }
                    else
                    {
                        SKick("You're still tempbanned!");
                        return;
                    }
                }
                catch { }
                ConnectionEventArgs eargs = new ConnectionEventArgs(true);
                bool cancel = OnPlayerConnect.Call(this, eargs, OnAllPlayersConnect).Canceled;
                if (cancel)
                {
                    Kick("Disconnected by canceled ConnectionEventArgs!");
                }
                byte type = message[129];
            Gotos_Are_The_Devil:
                if (Server.PlayerCount >= ServerSettings.GetSettingInt("MaxPlayers") && !Server.VIPs.Contains(Username) && !Server.Devs.Contains(Username))
                {
                    int LoopAmount = 0;
                    while (Server.PlayerCount >= ServerSettings.GetSettingInt("MaxPlayers"))
                    {
                        LoopAmount++;
                        Thread.Sleep(1000);
                        Packet pa = new Packet();
                        pa.Add(Packet.Types.MOTD);
                        pa.Add(ServerSettings.Version);
                        pa.Add("Waiting in queue, waiting for " + LoopAmount + " seconds", 64);
                        pa.Add(Server.PlayerCount + " players are online right now out of " + ServerSettings.GetSettingInt("MaxPlayers") + "!", 64);
                        pa.Add((byte)0);
                        SendPacket(pa);
                    }
                }

                //TODO Database Stuff
                Logger.Log("[System]: " + Ip + " logging in as " + Username + ".", System.Drawing.Color.Green, System.Drawing.Color.Black);
                try
                {
                    Server.IRC.SendMessage(Username + " joined the game!");
                }
                catch { }
                UniversalChat(Username + " joined the game!");

                //WOM.SendJoin(Username);

                CheckDuplicatePlayers(Username);
                foreach (PlayerGroup g in PlayerGroup.Groups)
                    if (g.Players.Contains(Username.ToLower()))
                        Group = g;

                ExtraData.CreateIfNotExist("HasMarked", false);
                ExtraData.CreateIfNotExist("Mark1", new Vector3S());
                ExtraData.CreateIfNotExist("Mark2", new Vector3S());
                IsLoading = true;
                IsLoggedIn = true;
                SendMotd();
                if (type == 0x42)
                {
                    extension = true;
                    SendExtInfo(15);
                    SendExtEntry("ClickDistance", 1);
                    SendExtEntry("CustomBlocks", 1);
                    SendExtEntry("HeldBlock", 1);
                    SendExtEntry("TextHotKey", 1);
                    SendExtEntry("ExtPlayerList", 1);
                    SendExtEntry("EnvColors", 1);
                    SendExtEntry("SelectionCuboid", 1);
                    SendExtEntry("BlockPermissions", 1);
                    SendExtEntry("ChangeModel", 1);
                    SendExtEntry("EnvMapAppearance", 1);
                    SendExtEntry("EnvWeatherType", 1);
                    SendExtEntry("HackControl", 1);
                    SendExtEntry("EmoteFix", 1);
                    SendExtEntry("MessageTypes", 1);
                    SendCustomBlockSupportLevel(1);
                }
                if (Level == null)
                    Level = Server.Mainlevel;
                else
                    Level = Level;

                ID = FreeId();
                if (Server.PlayerCount >= ServerSettings.GetSettingInt("MaxPlayers"))
                    goto Gotos_Are_The_Devil; 
                                         //Gotos are literally the devil, but it works here so two players dont login at once
                UpgradeConnectionToPlayer();
                short x = (short)((0.5 + Level.SpawnPos.x) * 32);
                short y = (short)((1 + Level.SpawnPos.y) * 32);
                short z = (short)((0.5 + Level.SpawnPos.z) * 32);

                Pos = new Vector3S(x, z, y);
                Rot = Level.SpawnRot;
                oldPos = Pos;
                oldRot = Rot;

                SendSpawn(this);
                SpawnThisPlayerToOtherPlayers();
                UpdatePosition(true);
                SpawnOtherPlayersForThisPlayer();
                SpawnBotsForThisPlayer();
                Server.Players.ForEach(delegate(Player p)
                {
                    if (p != this && p.HasExtension("ExtPlayerList"))
                    {
                        p.SendExtAddPlayerName(ID, Username, Group, Color + Username);
                    }
                    if (HasExtension("ExtPlayerList"))
                    {
                        SendExtAddPlayerName(p.ID, p.Username, p.Group, p.Color + p.Username);
                    }
                });
                IsLoading = false;

                //Load from Database
                Load();

                foreach (string w in ServerSettings.GetSetting("welcomemessage").Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries))
                    SendMessage(w);

            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
        private void HandleBlockchange(byte[] message)
        {
            if (!IsLoggedIn) return;

            ushort x = Packet.NTHO(message, 0);
            ushort y = Packet.NTHO(message, 2);
            ushort z = Packet.NTHO(message, 4);
            byte action = message[6];
            byte newType = message[7];
            HandleBlockchange(x, y, z, action, newType, false);
        }
        private void HandleBlockchange(ushort x, ushort y, ushort z, byte action, byte newType, bool fake) {
            LastClick = new Vector3S(x, y, z);
            if (newType > 65) {
                Kick("HACKED CLIENT!");
                //TODO Send message to op's for adminium hack
                return;
            }

            byte currentType = 50;
            if (y < Level.Size.y) {
                currentType = Level.GetBlock(x, z, y);
                if (!Block.IsValidBlock(currentType) && currentType != 255) {
                    Kick("HACKED SERVER!");
                    return;
                }
            }
            else {
                return;
            }

            bool placing = (action == 1);
            BlockChangeEventArgs eargs = new BlockChangeEventArgs(x, z, y, (placing ? ActionType.Place : ActionType.Delete), newType, Level.GetBlock(x, z, y));
            eargs = OnPlayerBlockChange.Call(this, eargs, OnAllPlayersBlockChange);
            if (eargs.Canceled) {
                if (!fake)
                    SendBlockChange(x, z, y, Level.GetBlock(eargs.X, eargs.Z, eargs.Y));
                return;
            }
            x = eargs.X;
            z = eargs.Z;
            y = eargs.Y;
            action = (byte)((eargs.Action == ActionType.Place) ? 1 : 0);
            placing = action == 1;
            if (fake || eargs.Holding != newType)
                SendBlockChange(x, z, y, eargs.Holding);
            newType = eargs.Holding;
            currentType = eargs.Current;
            if (action == 0) {
                Level.BlockChange(x, z, y, 0, this);
            }
            else {
                Level.BlockChange(x, z, y, newType, this);
            }
        }
        private void HandleIncomingPos(byte[] message)
        {
            if (!IsLoggedIn)
                return;

            byte thisid = message[0];

            ushort x = Packet.NTHO(message, 1);
            ushort y = Packet.NTHO(message, 3);
            ushort z = Packet.NTHO(message, 5);
            byte rotx = message[7];
            byte roty = message[8];
            oldPos = new Vector3S(Pos);
            Vector3S fromPosition = new Vector3S(oldPos.x, oldPos.z, oldPos.y);
            Pos.x = (short)x;
            Pos.y = (short)y;
            Pos.z = (short)z;
            oldRot = Rot;
            Rot = new byte[2] { rotx, roty };
            bool needsOwnPos = false;
            if (!(fromPosition.x == Pos.x && fromPosition.y == Pos.y && fromPosition.z == Pos.z))
            {
                MoveEventArgs eargs = new MoveEventArgs(new Vector3S(fromPosition), new Vector3S(Pos));
                eargs = OnPlayerMove.Call(this, eargs, OnAllPlayersMove);
                if (eargs.Canceled)
                {
                    Pos = fromPosition;
                    oldPos = fromPosition;
                    needsOwnPos = true;
                }
                else
                {
                    if (eargs.ToPosition / 32 != eargs.FromPosition / 32)
                    {
                        eargs = OnPlayerBigMove.Call(this, eargs, OnAllPlayersBigMove);
                        if (eargs.Canceled)
                        {
                            Pos = fromPosition;
                            oldPos = fromPosition;
                            needsOwnPos = true;
                        }
                        else
                        {
                            Pos = eargs.ToPosition;
                            oldPos = eargs.FromPosition;
                        }
                    }
                    else
                    {
                        Pos = eargs.ToPosition;
                        oldPos = eargs.FromPosition;
                    }
                }
            }
            if (oldRot[0] != Rot[0] || oldRot[1] != Rot[1])
            {
                RotateEventArgs eargs = new RotateEventArgs(Rot[0], Rot[1]);
                eargs = OnPlayerRotate.Call(this, eargs, OnAllPlayersRotate);
                if (eargs.Canceled)
                {
                    Rot = eargs.Rot;
                    needsOwnPos = true;
                }
                else
                {
                    Rot = eargs.Rot;
                }
            }
            if (needsOwnPos)
                SendThisPlayerTheirOwnPos();
            UpdatePosition(false);
        }
        private void HandleChat(byte[] message)
        {
            if (!IsLoggedIn) return;

            string incomingText = enc.GetString(message, 1, 64).Trim();

            byte incomingID = message[0];
            if (incomingID != 0xFF && incomingID != ID && incomingID != 0)
            {
                Player.UniversalChatOps("Player " + Username + ", sent a malformed packet!");
                Kick("Hacked Client!");
                return;
            }

            incomingText = Regex.Replace(incomingText, @"\s\s+", " ");

            if (StringUtils.ContainsBadChar(incomingText))
            {
                Kick("Illegal character in chat message!");
                return;
            }

            if (incomingText.Length == 0)
                return;

            if (incomingText[0] == '/' && incomingText.Length == 1)
            {
                if (ExtraData.ContainsKey("LastCmd"))
                {
                    incomingText = "/" + ExtraData["LastCmd"];
                }
                else
                {
                    SendMessage("You need to specify a command!");
                    return;
                }
            }

            //Get rid of whitespace
            var gex = new Regex(@"[ ]{2,}", RegexOptions.None);
            incomingText = gex.Replace(incomingText, @" ");


            //Meep is used above for //Command

            foreach (string word in Server.BadWordsList)
            {

                if (incomingText.Contains(word))
                    incomingText = Regex.Replace(incomingText, word, Server.ReplacementWordsList[playerRandom.Next(0, Server.ReplacementWordsList.Count)]);

            }

            ExtraData.CreateIfNotExist("Muted", false);
            var isMuted = (bool)ExtraData.GetIfExist("Muted");
            if (isMuted)
            {
                SendMessage("You are muted!");
                return;
            }

            ExtraData.CreateIfNotExist("Voiced", false);
            var isVoiced = (bool)ExtraData.GetIfExist("Voiced");
            if (Server.Moderation && !isVoiced && !Server.Devs.Contains<string>(Username))
            {
                SendMessage("You can't talk during chat moderation!");
                return;
            }

            ExtraData.CreateIfNotExist("Jokered", false);
            var isJokered = (bool)ExtraData.GetIfExist("Jokered");
            if (isJokered)
            {
                int a = playerRandom.Next(0, Server.JokerMessages.Count);
                incomingText = Server.JokerMessages[a];
            }

            incomingText = ReplaceEmoteKeywords(incomingText.ToString());

            //Message appending stuff.
            if (ServerSettings.GetSettingBoolean("messageappending"))
            {
                if (!String.IsNullOrWhiteSpace(_storedMessage))
                {
                    if (!incomingText.EndsWith(">") && !incomingText.EndsWith("<"))
                    {
                        incomingText = _storedMessage.Replace("|>|", " ").Replace("|<|", " ") + incomingText;
                        _storedMessage = String.Empty;
                    }
                }
                if (incomingText.EndsWith(">"))
                {
                    _storedMessage += incomingText.Replace(">", "|>|");
                    SendMessage("Message appended!");
                    return;
                }
                else if (incomingText.EndsWith("<"))
                {
                    _storedMessage += incomingText.Replace("<", "|<|");
                    SendMessage("Message appended!");
                    return;
                }
            }

            //This allows people to use //Command and have it appear as /Command in the chat.
            if (incomingText.StartsWith("//"))
            {
                incomingText = incomingText.Remove(0, 1);
            }
            else if (incomingText[0] == '/')
            {
                incomingText = incomingText.Remove(0, 1);

                string[] args = incomingText.Split(' ');
                HandleCommand(args);
                return;
            }

            ChatEventArgs eargs = new ChatEventArgs(incomingText, Username);
            bool canceled = OnPlayerChat.Call(this, eargs, OnAllPlayersChat).Canceled;
            if (canceled || eargs.Message == null || eargs.Message.Length == 0)
                return;
            incomingText = eargs.Message;

            //TODO: add this to a different plugin, its a mess right here, and i hate it
            if (Server.Voting)
            {
                if (Server.KickVote && Server.Kicker == this)
                {
                    SendMessage("You're not allowed to vote!");
                    return;
                }

                ExtraData.CreateIfNotExist("Voted", false);
                var didVote = (bool)ExtraData.GetIfExist("Voted");
                if (didVote)
                {
                    SendMessage("You have already voted...");
                    return;
                }
                string vote = incomingText.ToLower();
                if (vote == "yes" || vote == "y")
                {
                    Server.YesVotes++;
                    ExtraData["Voted"] = true;
                    SendMessage("Thanks for voting!");
                    return;
                }
                else if (vote == "no" || vote == "n")
                {
                    Server.NoVotes++;
                    ExtraData["Voted"] = true;
                    SendMessage("Thanks for voting!");
                    return;
                }
                else
                {
                    SendMessage("Use either %aYes " + Server.DefaultColor + "or %cNo " + Server.DefaultColor + " to vote!");
                }

            }

            ExtraData.CreateIfNotExist("OpChat", false);
            if (incomingText[0] == '#' || (bool)ExtraData.GetIfExist("OpChat")) //Opchat ouo
            {
                incomingText = incomingText.Trim().TrimStart('#');
                UniversalChatOps("&a<&fTo Ops&a> " + Group.Color + Username + ": &f" + incomingText);
                if (Group.Permission < ServerSettings.GetSettingInt("OpChatPermission"))
                {
                    SendMessage("&a<&fTo Ops&a> " + Group.Color + Username + ": &f" + incomingText);
                } //So players who aren't op see their messages
                Logger.Log("<OpChat> <" + Username + "> " + incomingText);
                try
                {
                    Server.IRC.SendOperatorMessage("<OpChat> <" + Username + "> " + incomingText);
                }
                catch { }
                return;
            }
            if (incomingText[0] == '*') //Rank chat
            {
                string groupname = Group.Name;
                incomingText = incomingText.Remove(0, 1);
                if (incomingText == "")
                {
                    return;
                }
                if (!groupname.EndsWith("ed") && !groupname.EndsWith("s"))
                {
                    groupname += "s";
                } //Plural
                RankChat(this, "&a<&fTo " + groupname + "&a> " + Group.Color + DisplayName + ": &f" + incomingText);
                Logger.Log("<" + groupname + " Chat> <" + Username + " as " + DisplayName + "> " + incomingText);
                return;
            }
            if (incomingText[0] == '!') //Level chat
            {
                incomingText = incomingText.Remove(0, 1);
                if (incomingText == "")
                {
                    return;
                }
                LevelChat(this, "&a<&f" + Level.Name + "&a> " + DisplayName + ":&f " + incomingText);
                Logger.Log("<" + Level.Name + " Chat> " + Username + " as " + DisplayName + ": " + incomingText);
                return;
            }

            ExtraData.CreateIfNotExist("AdminChat", false);
            if (incomingText[0] == '+' || (bool)ExtraData.GetIfExist("AdminChat")) //Admin chat
            {
                if (incomingText.StartsWith("+"))
                    incomingText = incomingText.Remove(0, 1);

                if (incomingText == "")
                {
                    return;
                }
                UniversalChatAdmins("&a<&fTo Admins&a> " + Group.Color + Username + ": &f" + incomingText);
                if (Group.Permission < ServerSettings.GetSettingInt("AdminChatPermission"))
                {
                    SendMessage("&a<&fTo Admins&a> " + Group.Color + Username + ": &f" + incomingText);
                }
                Logger.Log("<AdminChat> <" + Username + "> " + incomingText);
                return;
            }

            ExtraData.CreateIfNotExist("Whispering", false);
            if ((bool)ExtraData.GetIfExist("Whispering")) // /whisper command
            {
                ExtraData.CreateIfNotExist("WhisperTo", null);
                Player to = (Player)ExtraData.GetIfExist("WhisperTo");
                if (to == null)
                {
                    SendMessage("Player not found!");
                    return;
                }
                if (to == this)
                {
                    SendMessage("Trying to talk to yourself huh?");
                    return;
                }

                //no whispering nicknames
                SendMessage("[>] <" + to.Username + ">&f " + incomingText);
                to.SendMessage("[<] " + Username + ":&f " + incomingText);
                return;
            }

            ExtraData.CreateIfNotExist("GlobalChat", false);
            if (incomingText[0] == '~' || (bool)ExtraData.GetIfExist("GlobalChat")) //Admin chat
            {
                if(incomingText.StartsWith("~"))
                    incomingText = incomingText.Remove(0, 1);

                if (incomingText == "")
                    return;
                
                Server.GC.SendMessage(this, incomingText);
                Player.UniversalChat(String.Format("[GC] {0}: {1}", this.Username, incomingText));
                Logger.Log("<GC> <" + Username + "> " + incomingText);
                return;
            }

            if (incomingText[0] == '@') //Whisper whisper woosh woosh
            {
                incomingText = incomingText.Trim();
                if (incomingText[1] == ' ')
                {
                    incomingText = incomingText.Remove(1, 1);
                }

                incomingText = incomingText.Remove(0, 1);
                Player to = Player.Find(incomingText.Split(' ')[0]);
                if (to == null)
                {
                    SendMessage("Player not found!");
                    return;
                }

                incomingText = incomingText.Remove(0, to.Username.Length);
                //no whispering nicknames
                SendMessage("[>] <" + to.Username + ">&f " + incomingText.Trim());
                to.SendMessage("[<] " + Username + ":&f " + incomingText.Trim());
                return;
            }
            //TODO: remove to place better
            Logger.Log(Username != DisplayName ? "<" + Username + " as " + DisplayName + "> " + incomingText : "<" + Username + "> " + incomingText);
            var voiceString = (string)ExtraData.GetIfExist("VoiceString") ?? "";
            var mColor = Color ?? Group.Color;
            var mPrefix = (string)ExtraData.GetIfExist("Title") ?? "";
            string msg = voiceString +
                          mPrefix +
                          mColor +
                          DisplayName +
                          ": &f" +
                          incomingText;
            try
            {
                Server.IRC.SendMessage(voiceString + Username + ": " + incomingText);
            }
            catch { }
            UniversalChat(msg);
        }

        public void SetPrefix()
        {
            ExtraData.CreateIfNotExist("Prefix", "");
            var mTitle = ExtraData.GetIfExist("Title");
            var mTColor = ExtraData.GetIfExist("TitleColor");
            var mColor = Color;
            ExtraData["Prefix"] = mTitle == null ? "" : "[" + mTColor ?? Server.DefaultColor + mTitle + mColor ?? Server.DefaultColor + "]";
        }

        #endregion

        #region Outgoing Packets
        private object sendLock = new object();
        public override void SendPacket(Packet pa) {
            lock (sendLock) {
                if (!IsLoggedIn && !IsLoading) return;
                PacketEventArgs args = new PacketEventArgs(pa.bytes, false, (Packet.Types)pa.bytes[0]);
                bool Canceled = OnPlayerSendPacket.Call(this, args, OnAllPlayersSendPacket).Canceled;
                if (pa.bytes != args.Data)
                    pa.bytes = args.Data;
                if (!Canceled) {
                    try {
                        lastPacket = (Packet.Types)pa.bytes[0];
                    }
                    catch (Exception e) { Logger.LogError(e); }
                    for (int i = 0; i < 3; i++) {
                        try {
                            lastPacket = (Packet.Types)pa.bytes[0];
                        }
                        catch (Exception e) { Logger.LogError(e); }
                        for (int z = 0; z < 3; z++) {
                            try {
                                Socket.BeginSend(pa.bytes, 0, pa.bytes.Length, SocketFlags.None, delegate(IAsyncResult result) { }, null);

                                return;
                            }
                            catch {
                                continue;
                            }
                        }
                        CloseConnection();
                    }
                }
            }
        }
        private void SendMessage(byte PlayerID, string message)
        {
            for (int i = 0; i < 10; i++)
            {
                message = message.Replace("%" + i, "&" + i);
                message = message.Replace("&" + i + " &", "&");
            }
            for (char ch = 'a'; ch <= 'f'; ch++)
            {
                message = message.Replace("%" + ch, "&" + ch);
                message = message.Replace("&" + ch + " &", "&");
            }

            //if (!String.IsNullOrWhiteSpace(message) && message.IndexOf("^detail.user") == -1) 
            // caused ^detail.user to remove color from names in normal chat, not needed since wom only detects at beginning of string.
            // might i suggest adding a parameter for adding default color (for womsenddetail and preventing /say to change everyones)
                message = Server.DefaultColor + message;
                string newLine;
            try
            {
                foreach (string line in LineWrapping(message))
                {
                    if (line.TrimEnd(' ')[line.TrimEnd(' ').Length - 1] < '!')
                    {
                            newLine = line + '\'';
                    }
                    else
                    {
                        newLine = line;
                    }
                    Packet pa = new Packet();
                    pa.Add(Packet.Types.Message);
                    pa.Add((byte)0);
                    pa.Add(newLine, 64);
                    SendPacket(pa);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

        }
        private void SendMotd()
        {
            SendPacket(IsAdmin ? MOTDAdmin : MOTDNonAdmin);
        }
        private void SendMap()
        {

            try
            {
                byte[] blocks = new byte[Level.TotalBlocks]; //Temporary byte array so we dont have to keep modding the packet array
                byte block; //A block byte outside the loop, we save cycles by not making this for every loop iteration
                Level.ForEachBlock(pos =>
                {
                    //Here we loop through the whole map and check/convert the blocks as necesary
                    //We then add them to our blocks array so we can send them to the player
                    if(!extension)
                    {
                        block = Block.ConvertCPE(Level.Data[pos]);
                    }
                    else
                    {
                        block = Level.Data[pos];
                    }
                    //TODO ADD CHECKING
                    blocks[pos] = block;
                });
                IsLoading = true;
                SendPacket(mapSendStartPacket); //Send the pre-fab map start packet

                Packet pa = new Packet(); //Create a packet to handle the data for the map
                pa.Add(Level.TotalBlocks); //Add the total amount of blocks to the packet

                pa.Add(blocks); //add the blocks to the packet
                pa.GZip(); //GZip the packet

                int number = (int)Math.Ceiling(((double)(pa.bytes.Length)) / 1024); //The magic number for this packet

                for (int i = 1; pa.bytes.Length > 0; ++i)
                {
                    short length = (short)Math.Min(pa.bytes.Length, 1024);
                    byte[] send = new byte[1027];
                    Packet.HTNO(length).CopyTo(send, 0);
                    Buffer.BlockCopy(pa.bytes, 0, send, 2, length);
                    byte[] tempbuffer = new byte[pa.bytes.Length - length];
                    Buffer.BlockCopy(pa.bytes, length, tempbuffer, 0, pa.bytes.Length - length);
                    pa.bytes = tempbuffer;
                    send[1026] = (byte)(i * 100 / number);

                    Packet Send = new Packet(send);
                    Send.AddStart(new byte[1] { (byte)Packet.Types.MapData });

                    SendPacket(Send);
                }

                pa = new Packet();
                pa.Add(Packet.Types.MapEnd);
                pa.Add((short)Level.Size.x);
                pa.Add((short)Level.Size.y);
                pa.Add((short)Level.Size.z);
                SendPacket(pa);

                IsLoading = false;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
        public void SendSpawn(Player p)
        {
            byte ID = 0xFF;
            if (p != this)
                ID = p.ID;

            Packet pa = new Packet();
            pa.Add(Packet.Types.SendSpawn);
            pa.Add((byte)ID);
            pa.Add(p._displayName, 64);
            pa.Add(p.Pos.x);
            pa.Add((ushort)(p.Pos.y + ((ID == 0xFF) ? -21 : 3)));
            pa.Add(p.Pos.z);
            pa.Add(p.Rot);
            SendPacket(pa);
            p.UpdatePosition(true);
        }
        /// <summary>
        /// This send a blockchange to the player only. (Not other players)
        /// </summary>
        /// <param name="x">The position the block will be placed in (x)</param> 
        /// <param name="z"> The position the block will be placed in (z)</param>
        /// <param name="y"> The position the block will be placed in (y)</param>
        /// <param name="type"> The type of block that will be placed.</param>
        public void SendBlockChange(short x, short z, short y, byte type) {
            if (x < 0 || y < 0 || z < 0) return; //rest gets checked in the next function
            SendBlockChange((ushort)x, (ushort)z, (ushort)y, type);
        }
        /// <summary>
        /// This send a blockchange to the player only. (Not other players)
        /// </summary>
        /// <param name="x">The position the block will be placed in (x)</param>
        /// <param name="z"> The position the block will be placed in (z)</param>
        /// <param name="y"> The position the block will be placed in (y)</param>
        /// <param name="type"> The type of block that will be placed.</param>
        public void SendBlockChange(ushort x, ushort z, ushort y, byte type)
        {
            if (x >= Level.Size.x || y >= Level.Size.y || z >= Level.Size.z) return;
            Packet pa = new Packet();
            pa.Add(Packet.Types.SendBlockchange);
            pa.Add(x);
            pa.Add(y);
            pa.Add(z);
            if(type > 49 && !extension)
            {
                type = Block.ConvertCPE(type);
            }
            pa.Add(type);

            SendPacket(pa);
        }
        /// <summary>
        /// This sends multiple blockchanges to the player only. (Not other players)
        /// </summary>
        /// <param name="blocks">The blocks to be sent</param>
        /// <param name="offset">The offset of the blocks</param>
        /// <param name="type">The type of the blocks</param>
        public void SendBlockChange(Vector3S[] blocks, Vector3S offset, byte type) {
            foreach (Vector3S v in blocks) {
                SendBlockChange((ushort)(v.x + offset.x), (ushort)(v.z + offset.z), (ushort)(v.y + offset.y), type);
            }
        }
        /// <summary>
        /// This sends multiple blockchanges where currently is air to the player only. (Not other players)
        /// </summary>
        /// <param name="blocks">The blocks to be sent</param>
        /// <param name="offset">The offset of the blocks</param>
        /// <param name="type">The type of the blocks</param>
        public void SendBlockChangeWhereAir(Vector3S[] blocks, Vector3S offset, byte type) {
            foreach (Vector3S v in blocks) {
                if (Level.GetBlock(v + offset) == 0)
                    SendBlockChange((ushort)(v.x + offset.x), (ushort)(v.z + offset.z), (ushort)(v.y + offset.y), type);
            }
        }
        /// <summary>
        /// This sends multiple current blocks to the player only. (Not other players)
        /// </summary>
        /// <param name="blocks">The blocks to be sent</param>
        /// <param name="offset">The offset of the blocks</param>
        public void ResendBlockChange(Vector3S[] blocks, Vector3S offset) {
            foreach (Vector3S v in blocks) {
                SendBlockChange((ushort)(v.x + offset.x), (ushort)(v.z + offset.z), (ushort)(v.y + offset.y), Level.GetBlock(v + offset));
            }
        }
        /// <summary>
        /// Sends necessary blockchanges where the current block is air
        /// </summary>
        /// <param name="blocks">The blocks releative to the offsets</param>
        /// <param name="oldOffset">The previous offset (blocks well be resent according to this position</param>
        /// <param name="newOffset">The new offset (new blocks will be sent according to this position)</param>
        /// <param name="toReplace">The block type to replace</param>
        /// <param name="newType">The type of the blocks</param>
        public void SendReplaceNecessaryBlocksWhere(Vector3S[] blocks, Vector3S oldOffset, Vector3S newOffset, byte toReplace, byte newType) {
            Vector3S diff = newOffset - oldOffset;
            foreach (Vector3S v in blocks) {
                int i = 0;
                Vector3S tmp = v + diff;
                for (i = 0; i < blocks.Length; i++) {
                    if (blocks[i] == tmp) break;
                }
                if (i == blocks.Length) {
                    Vector3S theNew = v + newOffset;
                    if (Level.GetBlock(theNew) == toReplace) SendBlockChange(theNew.x, theNew.z, theNew.y, newType);
                }

                tmp = v - diff;
                for (i = 0; i < blocks.Length; i++) {
                    if (blocks[i] == tmp) break;
                }
                if (i == blocks.Length) {
                    Vector3S theOld = v + oldOffset;
                    if (Level.GetBlock(theOld) == toReplace) SendBlockChange(theOld.x, theOld.z, theOld.y, toReplace);
                }

            }
        }
        private void SendKick(string message)
        {

            Packet pa = new Packet();
            pa.Add(Packet.Types.SendKick);
            pa.Add(message, 64);
            SendPacket(pa);
        }
        private void SMPKick(string a)
        {
            //Read first, then kick
            var Stream = Client.GetStream();
            var Reader = new BinaryReader(Stream);
            var Writer = new BinaryWriter(Stream);
            short len = IPAddress.HostToNetworkOrder(Reader.ReadInt16());

            if (len > 1 && len < 17)
            {
                string name = Encoding.BigEndianUnicode.GetString(Reader.ReadBytes(len * 2));
                Logger.Log(String.Format("{0} tried to log in from an smp client", name));

                byte[] messageInBytes = Encoding.BigEndianUnicode.GetBytes(a);
                Writer.Write((byte)255);
                Writer.Write((short)messageInBytes.Length);
                Writer.Write(messageInBytes);
                Writer.Flush();
                CloseConnection();
            }
            else
            {
                Logger.Log("Received unknown packet");
                Kick("Unknown Packet received");
            }

            Reader.Close();
            Reader.Dispose();

            Writer.Close();
            Writer.Dispose();


        }
        private void SendPing()
        {
            SendPacket(pingPacket);
        }

        /// <summary>
        /// Exactly what the function name is, it might be useful to change this players pos first ;)
        /// </summary>
        public void SendThisPlayerTheirOwnPos()
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.SendTeleport);
            pa.Add((byte)255);
            pa.Add(Pos.x);
            pa.Add(Pos.y);
            pa.Add(Pos.z);
            pa.Add(Rot);
            SendPacket(pa);
        }
        /// <summary>
        /// Kick this player with the specified message, the message broadcasts across the server
        /// </summary>
        /// <param name="message">The message to send</param>
        public void Kick(string message)
        {
            //GlobalMessage(message);
            IsBeingKicked = true;
            SKick(message);
        }
        /// <summary>
        /// Kick this player with a specified message, this message will only get sent to op's
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SKick(string message)
        {
            Logger.Log("[Info]: Kicked: *" + Username + "* " + message, System.Drawing.Color.Yellow, System.Drawing.Color.Black);
            SendKick(message);
            //CloseConnection();
        }
        /// <summary>
        /// Sends the specified player to the specified coordinates.
        /// </summary>
        /// <param name="_pos"></param>Vector3 coordinate to send to.
        /// <param name="_rot"></param>Rot to send to.
        public void SendToPos(Vector3S _pos, byte[] _rot)
        {
            oldPos = Pos; oldRot = Rot;
            _pos.x = (_pos.x < 0) ? (short)32 : (_pos.x > Level.Size.x * 32) ? (short)(Level.Size.x * 32 - 32) : (_pos.x > 32767) ? (short)32730 : _pos.x;
            _pos.z = (_pos.z < 0) ? (short)32 : (_pos.z > Level.Size.z * 32) ? (short)(Level.Size.z * 32 - 32) : (_pos.z > 32767) ? (short)32730 : _pos.z;
            _pos.y = (_pos.y < 0) ? (short)32 : (_pos.y > Level.Size.y * 32) ? (short)(Level.Size.y * 32 - 32) : (_pos.y > 32767) ? (short)32730 : _pos.y;


            Packet pa = new Packet();
            pa.Add(Packet.Types.SendTeleport);
            pa.Add(unchecked((byte)-1)); //If the ID is not greater than one it doesn't work :c
            pa.Add(_pos.x);
            pa.Add(_pos.y);
            pa.Add(_pos.z);
            pa.Add(Rot);

            SendPacket(pa);
        }

        internal void UpdatePosition(bool ForceTp)
        {
            Vector3S tempOldPos = new Vector3S(oldPos);
            Vector3S tempPos = new Vector3S(Pos);
            byte[] tempRot = Rot;
            byte[] tempOldRot = oldRot;
            if (tempOldRot == null) tempOldRot = new byte[2];
            if (IsHeadFlipped)
                tempRot[1] = 125;

            oldPos = tempPos;
            oldRot = tempRot;

            short diffX = (short)(tempPos.x - tempOldPos.x);
            short diffZ = (short)(tempPos.z - tempOldPos.z);
            short diffY = (short)(tempPos.y - tempOldPos.y);
            int diffR0 = tempRot[0] - tempOldRot[0];
            int diffR1 = tempRot[1] - tempOldRot[1];

            //TODO rewrite local pos change code
            if (diffX == 0 && diffY == 0 && diffZ == 0 && diffR0 == 0 && diffR1 == 0)
            {
                return; //No changes
            }
            bool teleport = ForceTp || (Math.Abs(diffX) >= 127 || Math.Abs(diffY) >= 127 || Math.Abs(diffZ) >= 127) || true; //Leave true untill issue 38 is fixed!

            Packet pa = new Packet();
            if (teleport)
            {
                pa.Add(Packet.Types.SendTeleport);
                pa.Add(ID);
                pa.Add(tempPos.x);
                pa.Add((short)(tempPos.y + 3));
                pa.Add(tempPos.z);
                pa.Add(tempRot);
            }
            else
            {
                bool rotupdate = diffR0 != 0 || diffR1 != 0;
                bool posupdate = diffX != 0 || diffY != 0 || diffZ != 0;
                if (rotupdate && posupdate)
                {
                    pa.Add(Packet.Types.SendPosANDRotChange);
                    pa.Add(ID);
                    pa.Add((sbyte)diffX);
                    pa.Add((sbyte)diffY);
                    pa.Add((sbyte)diffZ);
                    pa.Add(tempRot);
                }
                else if (rotupdate)
                {
                    pa.Add(Packet.Types.SendRotChange);
                    pa.Add(ID);
                    pa.Add(tempRot);
                }
                else if (posupdate)
                {
                    pa.Add(Packet.Types.SendPosChange);
                    pa.Add(ID);
                    pa.Add((sbyte)(diffX));
                    pa.Add((sbyte)(diffY));
                    pa.Add((sbyte)(diffZ));
                }
                else return;
            }

            Server.ForeachPlayer(delegate(Player p)
            {
                if (p != this && p.Level == Level && p.IsLoggedIn && !p.IsLoading)
                {
                    p.SendPacket(pa);
                }
            });
        }

        public void SendExtInfo(short count)
        {
            SendPacket(pingPacket);
            Packet pa = new Packet();
            pa.Add(Packet.Types.ExtInfo);
            pa.Add("MCForge-Redux", 64);
            pa.Add(count);
            SendPacket(pa);
        }
        public void SendExtEntry(string name, int version)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.ExtEntry);
            pa.Add(name, 64);
            pa.Add(version);
            SendPacket(pa);
        }
        public void SendClickDistance(short distance)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.SetClickDistance);
            pa.Add(distance);
            SendPacket(pa);
        }
        public void SendCustomBlockSupportLevel(byte level)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.CustomBlockSupportLevel);
            pa.Add(level);
            SendPacket(pa);
        }
        public void SendHoldThis(byte type, byte locked)
        { // if locked is on 1, then the player can't change their selected block.
            Packet pa = new Packet();
            pa.Add(Packet.Types.HoldThis);
            pa.Add(type);
            pa.Add(locked);
            SendPacket(pa);
        }
        public void SendTextHotKey(string label, string command, int keycode, byte mods)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.SetTextHotKey);
            pa.Add(label, 64);
            pa.Add(command, 64);
            pa.Add(keycode);
            pa.Add(mods);
            SendPacket(pa);
        }
        public void SendExtAddPlayerName(short id, string name, PlayerGroup grp, string displayname = "")
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.ExtAddPlayerName);
            pa.Add(id);
            pa.Add(name, 64);
            if (displayname == "") { displayname = Color + name; }
            pa.Add(displayname, 64);
            pa.Add(grp.Name.ToUpper() + "s:", 64);
            pa.Add((byte)(grp.Permission));
            SendPacket(pa);
        }

        public void SendExtAddEntity(byte id, string name, string displayname = "")
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.ExtAddEntity);
            byte[] buffer = new byte[129];
            buffer[0] = id;
            pa.Add(name, 64);
            if (displayname == "") { displayname = name; }
            pa.Add(displayname, 64);
            SendPacket(pa);
        }

        public void SendExtRemovePlayerName(short id)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.ExtRemovePlayerName);
            pa.Add(id);
            SendPacket(pa);
        }
        public void SendEnvSetColor(byte type, short r, short g, short b)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.EnvSetColor);
            pa.Add(type);
            pa.Add(r);
            pa.Add(g);
            pa.Add(b);
            SendPacket(pa);
        }
        public void SendMakeSelection(byte id, string label, short smallx, short smally, short smallz, short bigx, short bigy, short bigz, short r, short g, short b, short opacity)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.MakeSelection);
            pa.Add(id);
            pa.Add(label, 64);
            pa.Add(smallx);
            pa.Add(smally);
            pa.Add(smallz);
            pa.Add(bigx);
            pa.Add(bigy);
            pa.Add(bigz);
            pa.Add(r);
            pa.Add(g);
            pa.Add(b);
            pa.Add(opacity);
            SendPacket(pa);
        }
        public void SendDeleteSelection(byte id)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.RemoveSelection);
            pa.Add(id);
            SendPacket(pa);
        }
        public void SendSetBlockPermission(byte type, byte canplace, byte candelete)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.SetBlockPermission);
            pa.Add(canplace);
            pa.Add(candelete);
            SendPacket(pa);
        }
        public void SendChangeModel(byte id, string model)
        {
            if (!HasExtension("ChangeModel")) { return; }
            Packet pa = new Packet();
            pa.Add(Packet.Types.ChangeModel);
            pa.Add(id);
            pa.Add(model, 64);
            SendPacket(pa);
        }
        public void SendSetMapAppearance(string url, byte sideblock, byte edgeblock, short sidelevel)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.EnvMapAppearance);
            byte[] buffer = new byte[68];
            pa.Add(url, 64);
            pa.Add(sideblock);
            pa.Add(edgeblock);
            pa.Add(sidelevel);
            SendPacket(pa);
        }
        public void SendSetMapWeather(byte weather)
        { // 0 - sunny; 1 - raining; 2 - snowing
            Packet pa = new Packet();
            pa.Add(Packet.Types.EnvWeatherType);
            pa.Add(weather);
            SendPacket(pa);
        }
        public void SendHackControl(byte allowflying, byte allownoclip, byte allowspeeding, byte allowrespawning, byte allowthirdperson, byte allowchangingweather, short maxjumpheight)
        {
            Packet pa = new Packet();
            pa.Add(Packet.Types.HackControl);
            pa.Add(allowflying);
            pa.Add(allownoclip);
            pa.Add(allowspeeding);
            pa.Add(allowrespawning);
            pa.Add(allowthirdperson);
            pa.Add(allowchangingweather);
            pa.Add(maxjumpheight);
            SendPacket(pa);
        }
        #endregion

        /// <summary>
        /// Spawns this player to all other players in the server.
        /// </summary>
        public void SpawnThisPlayerToOtherPlayers()
        {
            Server.ForeachPlayer(delegate(Player p)
            {
                if (p != this && p.Level == Level && p.IsLoggedIn && !p.IsLoading && !p.IsHidden)
                    p.SendSpawn(this);
            });
        }
        /// <summary>
        /// Spawns all other players of the server to this player.
        /// </summary>
        public void SpawnOtherPlayersForThisPlayer()
        {
            Server.ForeachPlayer(delegate(Player p)
            {
                if (p != this && p.Level == Level)
                    SendSpawn(p);
            });
        }

        /// <summary>
        /// Spawns all bots to this player
        /// </summary>
        public void SpawnBotsForThisPlayer()
        {
            Server.ForeachBot(delegate(Bot p)
            {
                if (p.Player.Level == Level)
                    SendSpawn(p.Player);
            });
        }
        internal void SendBlockchangeToOthers(Level l, ushort x, ushort z, ushort y, byte block)
        {
            Server.ForeachPlayer(delegate(Player p)
            {
                if (p == this) return;
                if (p.Level == l)
                    p.SendBlockChange(x, z, y, block);
            });
        }
        public static void GlobalBlockchange(Level l, ushort x, ushort z, ushort y, byte block)
        {
            Server.ForeachPlayer(delegate(Player p)
            {
                if (p.Level == l)
                    p.SendBlockChange(x, z, y, block);
            });
        }

        /// <summary>
        /// Kill this player for everyone.
        /// </summary>
        public void GlobalDie()
        {
            Packet pa = new Packet(new byte[2] { (byte)Packet.Types.SendDie, ID });
            Server.ForeachPlayer(p =>
            {
                if (p != this)
                {
                    p.SendPacket(pa);
                }
            });
        }
        /// <summary>
        /// Send a message to everyone, on every world
        /// </summary>
        /// <param name="text">The message to send.</param>
        public static void UniversalChat(string text)
        {
            Server.ForeachPlayer(p =>
            {
                p.SendMessage(text);
            });

        }
        /// <summary>
        /// Sends a message to all operators+
        /// </summary>
        /// <param name="message">The message to send</param>
        public static void UniversalChatOps(string message)
        {
            Server.ForeachPlayer(p =>
            {
                if (p.Group.Permission >= ServerSettings.GetSettingInt("OpChatPermission"))
                {
                    p.SendMessage(message);
                }
            });
        }
        /// <summary>
        /// Sends a message to all admins+
        /// </summary>
        /// <param name="message">The message to be sent</param>
        public static void UniversalChatAdmins(string message)
        {
            Server.ForeachPlayer(p =>
            {
                if (p.Group.Permission >= ServerSettings.GetSettingInt("AdminChatPermission"))
                {
                    p.SendMessage(message);
                }
            });
        }
        /// <summary>
        /// Sends a message to all of the players with the same rank
        /// </summary>
        /// <param name="from">The player sending the message</param>
        /// <param name="message">The message to send</param>
        public static void RankChat(Player from, string message)
        {
            Server.ForeachPlayer(delegate(Player p)
            {
                if (p.Group.Permission == from.Group.Permission)
                {
                    p.SendMessage(message);
                }
            });
        }
        /// <summary>
        /// Sends a message to all of the players on the specified level
        /// </summary>
        /// <param name="from">The player sending the message</param>
        /// <param name="message">The message to be sent</param>
        public static void LevelChat(Player from, string message)
        {
            Server.ForeachPlayer(delegate(Player p)
            {
                if (p.Level == from.Level) { p.SendMessage(message); }
            });
        }
        private void CloseConnection()
        {
            if (IsBot) return;
            ConnectionEventArgs eargs = new ConnectionEventArgs(false);
            OnPlayerDisconnect.Call(this, eargs);
            OnAllPlayersDisconnect.Call(this, eargs);

            GlobalDie();
            Server.RemovePlayer(this);
            if (IsLoggedIn)
            {
                Logger.Log("[System]: " + Username + " Has DC'ed (" + lastPacket + ")", System.Drawing.Color.Gray, System.Drawing.Color.Black);
                try
                {
                    Server.IRC.SendMessage("[System]: " + Username + " has disconnected");
                }
                catch { }

                if (Server.PlayerCount > 0)
                    Player.UniversalChat(Username + " has disconnected");
                //WOM.SendLeave(Username);
            }
            IsLoggedIn = false;
            Server.Connections.Remove(this);

            Socket.Close();
            
            Socket.Dispose();
        }

        internal static void GlobalPing()
        {
            Server.ForeachPlayer(p => p.SendPing());
        }

        internal DateTime lastReceived;
    }
}