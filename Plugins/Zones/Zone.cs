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
using MCForge.Entity;
using MCForge.Utils;
using MCForge.World;

namespace Plugins.Zones
{
    class Zone
    {
        public Cuboid ProtectedZone { get; private set; }
        public string Owner { get; private set; }
        public string Name { get; private set; }
        public byte Permission { get; private set; }
        public Level Level { get; private set; }

        public Zone(Vector3D point1, Vector3D point2, Player owner, string name, byte minimumGroup)
        {
            ProtectedZone = new Cuboid(point1, point2);
            Owner = owner.DisplayName;
            Permission = minimumGroup;
            Zones.Add(this);
            Level = owner.Level;
            Name = name;

            ZoneList zones;
            if (Level.ExtraData.ContainsKey("zones"))
            {
                zones = ZoneList.FromString((string)Level.ExtraData.GetIfExist("zones"));
                zones.Add(this);
            }
            else
            {
                zones = GetAllZonesForLevel(Level);
            }
            Level.ExtraData["zones"] = zones;
        }

        public bool CanBuildIn(Player player)
        {
            return Permission <= player.Group.Permission;
        }
        public void Delete()
        {
            ZoneList zones;
            Zones.Remove(this);
            if (Level.ExtraData.ContainsKey("zones"))
            {
                zones = ZoneList.FromString((string)Level.ExtraData["zones"]);
                zones.Remove(this);
            }
            else
            {
                zones = GetAllZonesForLevel(Level);
            }
            Level.ExtraData["zones"] = zones;
   
        }


        private static readonly List<Zone> Zones = new List<Zone>();

        public Zone(Vector3D point1, Vector3D point2, string owner, string name, byte minimumGroup, Level level, bool skipChecks = false)
        {
            ProtectedZone = new Cuboid(point1, point2);
            Owner = owner;
            Permission = minimumGroup;
            Zones.Add(this);
            Level = level;
            Name = name;

            if (skipChecks)
                return;
            ZoneList zones;
            if (Level.ExtraData.ContainsKey("zones"))
            {
                zones = ZoneList.FromString((string)Level.ExtraData.GetIfExist("zones"));
                zones.Add(this);
            }
            else
            {
                zones = GetAllZonesForLevel(Level);
            }
            Level.ExtraData["zones"] = zones;
        }


        public static List<Zone> FindAllWithin(Vector3D pos)
        {
            var zones = new List<Zone>();
            foreach (var zone in Zones)
            {
                if (zone.ProtectedZone.Within(pos))
                    zones.Add(zone);
            }
            return zones;
        }
        public static Zone FindWithin(Vector3D pos)
        {
            foreach (var zone in Zones)
            {
                if (zone.ProtectedZone.Within(pos))
                    return zone;
            }
            return null;
        }

        public static ZoneList GetAllZonesForLevel(Level level)
        {
            ZoneList zonelist = null;
            if (level.ExtraData.ContainsKey("zones"))
            {
                zonelist = ZoneList.FromString((string)level.ExtraData.GetIfExist("zones"), level);
            }


            if (zonelist != null)
                foreach (var zone in zonelist.ToArray())
                {
                    if (!Zones.Contains(zone))
                        Zones.Add(zone);
                }

            var zones = new ZoneList();
            foreach (var zone in Zones)
            {
                if (zone.Level == level)
                    zones.Add(zone);
            }
            return zones;
        }
        public static Zone GetZoneByName(string name)
        {
            foreach (var zone in Zones)
            {
                if (zone.Name.ToLower() == name.ToLower())
                    return zone;
            }
            return null;
        }

        public static implicit operator string(Zone z)
        {
            return z.Name;
        }
        public override string ToString()
        {
            return this;
        }
    }
}
