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
using MCForge.API.Events;
using MCForge.Entity;
using MCForge.Groups;
using MCForge.Interface.Plugin;
using MCForge.Utils;

namespace Plugins.Zones
{
    public class PluginZones : IPlugin
    {
        public string Name
        {
            get { return "Zones"; }
        }

        public string Author
        {
            get { return "cazzar"; }
        }

        public int Version
        {
            get { return 1; }
        }

        public string CUD
        {
            get { return ""; }
        }

        public void OnLoad(string[] args)
        {
            Player.OnAllPlayersBlockChange.High += OnAllPlayersBlockChangeOnHigh;
        }

        private void OnAllPlayersBlockChangeOnHigh(Player sender, BlockChangeEventArgs args)
        {
            var zones = Zone.FindAllWithin(new Vector3D(args.X, args.Z, args.Y));
            foreach (var zone in zones)
            {
                if (zone.CanBuildIn(sender)) continue;

                sender.SendMessage("You cannot build in this Zone! This zone if for: " + (PlayerGroup.FindPermInt(zone.Permission) ?? ("Permission: " + zone.Permission)) + " or higher!");
                args.Cancel();
                return;
            }
        }

        public void OnUnload()
        {
            Player.OnAllPlayersBlockChange.High -= OnAllPlayersBlockChangeOnHigh;
        }
    }
}
