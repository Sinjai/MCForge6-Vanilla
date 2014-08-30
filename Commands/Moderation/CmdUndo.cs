/*
Copyright 2012 MCForge
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
#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCForge.SQL;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.Groups;
using MCForge.Core;
using MCForge.World;
using System.Diagnostics;
using System.Collections.Specialized;
using MCForge.Utils;

namespace MCForge.Commands.Moderation {
    public class CmdUndo : ICommand {
        #region ICommand Members

        public string Name {
            get { return "Undo"; }
        }

        public CommandTypes Type {
            get { return CommandTypes.Mod; }
        }

        public string Author {
            get { return "MCForge Devs"; }
        }

        public int Version {
            get { return 1; }
        }

        public string CUD {
            get { return ""; }
        }

        public byte Permission {
            get { return 0; }
        }

        public void Use(Player p, string[] args) {

            int _time = 30;
            Player who = p;
            long UID = -1;

            //undo <seconds>
            if (args.Length == 1) {
                try {
                    _time = int.Parse(args[0]);
                    if (_time < 1) {
                        p.SendMessage("The time must be greater than 1");
                        return;
                    }
                }
                catch {
                    p.SendMessage("That is not a vaild number");
                    return;
                }
            }


            else if (args.Length == 2) {
                who = Player.Find(args[0]);

                if (who == null) {
                	//Try getting offline player
                    UID = Player.GetUID(args[0]);
                }

                if (args[1].ToLower() == "all") {

                    _time = int.MaxValue;
                }
                else {
                    try {
                        _time = int.Parse(args[1]);
                        if (_time < 1) {
                            p.SendMessage("The time must be greater than 1");
                            return;
                        }
                    }
                    catch {
                        p.SendMessage("That is not a valid number");
                    }
                }
            }

            if (p.Group.Permission < (byte)PermissionLevel.Operator) {
                if (who != p) {
                    p.SendMessage("You cannot undo other peoples stuff");
                    return;
                }
            }
            if (UID != -1)
            {
            	Undo(UID, _time, p.Level, who);
            	Player.UniversalChat(Server.DefaultColor + "Undid " + args[0] + " for &c" + _time + Server.DefaultColor + " seconds");
            }
            else
            {
            	Undo(who, _time);
            	Player.UniversalChat(Server.DefaultColor + "Undid " + who.Color + who.Username + Server.DefaultColor + " for &c" + _time + Server.DefaultColor + " seconds");
            }
        }

        void Undo(long UID, int time, Level l, Player online) {
            long since=DateTime.Now.AddSeconds(-time).Ticks;
            int count = 0;
            foreach (var ch in BlockChangeHistory.GetCurrentIfUID(l.Name, (uint)UID, since)) {
                RedoHistory.Add((uint)UID, l.Name, ch.Item1, ch.Item2, ch.Item3, ch.Item4);
                count++;
            }
            foreach (var ch in BlockChangeHistory.Undo(l.Name, (uint)UID, since)) {
                l.BlockChange(ch.Item1, ch.Item2, ch.Item3, ch.Item4);
            }
            online.SendMessage("&e" + count + Server.DefaultColor + " Blocks changed");
            return;
        }
        void Undo(Player p, int time = 30) {
        	Undo(p.UID, time, p.Level, p);
        }

        public void Help(Player p) {
            p.SendMessage(Server.DefaultColor + "/undo <player> <seconds> - Undoes the block changes for <player> in the past <seconds>");
            p.SendMessage(Server.DefaultColor + "/undo <player> all - Undoes as much as it can.");
            p.SendMessage(Server.DefaultColor + "/undo <seconds> - Unodes the block changes for yourself in the past <seconds>");
            p.SendMessage(Server.DefaultColor + "/undo - Undoes the block changes for yourself in the past&c 30 " + Server.DefaultColor + " seconds");
        }

        public void Initialize() {
            Level.OnAllLevelsLoad.Normal += OnAllLevelsLoadUnload_Normal;
            Level.OnAllLevelsUnload.Normal += OnAllLevelsLoadUnload_Normal;
            Command.AddReference(this, "undo");
        }

        void OnAllLevelsLoadUnload_Normal(Level sender, API.Events.LevelLoadEventArgs args) {
            if (args.Loaded) {
                if (!BlockChangeHistory.Load(sender.Name)) {
                    BlockChangeHistory.SetLevel(sender.Name, (ushort)sender.CWMap.Size.x, (ushort)sender.CWMap.Size.z, (ushort)sender.CWMap.Size.y, sender.CWMap.BlockData);
                }
            }
            else
                BlockChangeHistory.WriteOut(sender.Name, true);
        }

        #endregion
    }
}
