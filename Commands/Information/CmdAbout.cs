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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.API.Events;
using MCForge.SQL;
using MCForge.Utils;
using MCForge.Core;
using MCForge.World;
namespace MCForge.Commands.Information {
    public class CmdAbout : ICommand {
        #region ICommand Members

        public string Name {
            get { return "About"; }
        }

        public CommandTypes Type {
            get { return CommandTypes.Information; }
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

        public void Use(MCForge.Entity.Player p, string[] args) {
            p.SendMessage("Break block to get info");
            p.OnPlayerBlockChange.Normal += OnBlockChange;
        }

        public void Help(MCForge.Entity.Player p) {
           
        }

        public void Initialize() {
            Command.AddReference(this, "b", "about");
        }

        #endregion

        void OnBlockChange(Player sender, BlockChangeEventArgs e) {
            sender.OnPlayerBlockChange.Normal -= OnBlockChange;
            e.Cancel();
            int count = 0;
            foreach (var info in BlockChangeHistory.About(sender.Level.Name, e.X, e.Z, e.Y)) {
                string name = Player.GetName(info.Item2);
                string color = Player.GetColor(info.Item2);
                sender.SendMessage(((name == null) ? "nobody" : ((color == null) ? "" : color) + name) + " changed to " + ((info.Item1 == 0) ? "&4" : "&3") + ((Block)info.Item1).Name + " " + Server.DefaultColor + "at " + new DateTime(info.Item3).ToString("dd/MM/yy HH:mm:ss"));
                count++;
            }
            sender.SendMessage(count + Server.DefaultColor + " changes listed.");
            if (sender.StaticCommandsEnabled) {
                sender.SendMessage("Break block to get info");
                sender.OnPlayerBlockChange.Normal += OnBlockChange;
            }
        }
    }
}
