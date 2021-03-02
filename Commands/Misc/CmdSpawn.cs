﻿/*
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
using System.Linq;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils;

namespace MCForge.Commands
{
    public class CmdSpawn : ICommand
    {
        public string Name { get { return "Spawn"; } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "Gamemakergm"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }

        public void Use(Player p, string[] args)
        {
            if (args.Count() != 0)
            {
                Help(p);
                return;
            }
            Vector3S meep = new Vector3S((short)(16 + p.Level.CWMap.SpawnPos.x * 32), (short)(16 + p.Level.CWMap.SpawnPos.z * 32), (short)(p.Level.CWMap.SpawnPos.y * 32));
            Packet pa = new Packet();
            pa.Add(Packet.Types.SendTeleport);
            pa.Add((byte)0xff);
            pa.Add(meep.x);
            pa.Add(meep.y);
            pa.Add(meep.z);
            pa.Add(new byte[2] { (byte)p.Level.CWMap.SpawnRotation.x, (byte)p.Level.CWMap.SpawnRotation.z });
            p.SendPacket(pa);
        }
        public void Help(Player p)
        {
            p.SendMessage("/spawn - Teleports yourself to the spawn location.");
        }
        public void Initialize()
        {
            Command.AddReference(this, "spawn");
        }
    }
}
