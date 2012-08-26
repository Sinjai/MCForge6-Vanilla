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
using System.Collections.Generic;
using MCForge.API.Events;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Groups;
using MCForge.Utils;

namespace Plugins.Zones
{
    class CmdZone : ICommand
    {
        public string Name { get { return "Zone"; } }
        public CommandTypes Type { get { return CommandTypes.Mod;} }
        public string Author { get { return "cazzar"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return (byte) PermissionLevel.Operator; } }
        public void Use(Player p, string[] args)
        {
            p.SendMessage("Please place the first block to define the region");
            p.OnPlayerBlockChange.Normal += OnPlayerBlockChangeOnNormal;
            if (Zone.GetZoneByName(args[0]) != null)
            {
                p.SendMessage("Zone by that name already exists!");
                return;
            }
            
            byte b = 0;
            try
            {
                b = byte.Parse(args[1]);
            }
            catch
            {
                try
                {
                    b = PlayerGroup.Find(args[1]);
                }
                catch
                {
                    p.SendMessage("Error parsing permission");
                    return;
                }
            }
            var data = new KeyValuePair<string, byte>(args[0], b);
            p.SetDatapass(Name, data);
        }

        private void OnPlayerBlockChangeOnNormal(Player sender, BlockChangeEventArgs args)
        {
            args.Unregister();
            args.Cancel();
            var data = (KeyValuePair<string, byte>)sender.GetDatapass(Name);
            sender.SetDatapass(Name,new KeyValuePair<KeyValuePair<string, byte>,Vector3D>(data, new Vector3D(args.X, args.Z, args.Y)));
            sender.SendMessage("Now, please place a 2nd block to finish defining the region");
            sender.OnPlayerBlockChange.Normal += OnPlayerBlockChangeOnNormal2;
        }

        private void OnPlayerBlockChangeOnNormal2(Player sender, BlockChangeEventArgs args)
        {
            args.Unregister();
            args.Cancel();
            var data = (KeyValuePair<KeyValuePair<string, byte>,Vector3D>) sender.GetDatapass(Name);
            var z = new Zone(data.Value, new Vector3D(args.X, args.Z, args.Y), sender, data.Key.Key, data.Key.Value);
            
        }

        public void Help(Player p)
        {
           
        }

        public void Initialize()
        {
            Command.AddReference(this, "zone");
        }
    }
}
