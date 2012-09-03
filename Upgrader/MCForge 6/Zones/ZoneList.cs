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
using System.Text;
using MCForge.Utils;
using MCForge.World;

namespace Plugins.Zones
{
    class ZoneList
    {
        private readonly List<Zone> _items;

        private int Size
        {
            get { return _items.Count; }
        }

        public Zone this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Size)
                    throw new ArgumentOutOfRangeException();
                return _items[index];
            }
            set 
            {
                if ((uint)index >= (uint)Size)
                    throw new ArgumentOutOfRangeException();
                _items[index] = value;
            }
        }

        public ZoneList()
        {
            _items = new List<Zone>();
        }

        public void Add(Zone item)
        {
            _items.Add(item);
        }

        public void Remove(Zone item)
        {
            _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }
        public IEnumerable<Zone> ToArray()
        {
            return _items.ToArray();
        }

        /*public static implicit operator ZoneList(string list)
        {
            var strings = list.Split(';');
            if (strings.Length == 0)
                return null;
            var zoneList = new ZoneList();
            foreach (var s in strings)
            {
                var str = s.Split(':');
                if (str.Length != 6)
                    continue;
                var name = str[0];
                var owner = str[1];
                var pos1 = str[2].Split(',');
                var pos2 = str[3].Split(',');
                var perm = byte.Parse(str[4]);
                var level = Level.FindLevel(str[5]);
                zoneList.Add(new Zone(
                    new Vector3D(int.Parse(pos1[0]),int.Parse(pos1[1]),int.Parse(pos1[2])), 
                    new Vector3D(int.Parse(pos2[0]),int.Parse(pos2[1]),int.Parse(pos2[2])),
                    owner, name, perm, level));
            }
            return zoneList;

        }*/
        public static ZoneList FromString(string list, Level lvl)
        {
            var strings = list.Split(';');
            if (strings.Length == 0)
                return null;
            var zoneList = new ZoneList();
            foreach (var s in strings)
            {
                var str = s.Split(':');
                if (str.Length != 6)
                    continue;
                var name = str[0];
                var owner = str[1];
                var pos1 = str[2].Split(',');
                var pos2 = str[3].Split(',');
                var perm = byte.Parse(str[4]);
                var level = lvl;
                zoneList.Add(new Zone(
                    new Vector3D(int.Parse(pos1[0]), int.Parse(pos1[1]), int.Parse(pos1[2])),
                    new Vector3D(int.Parse(pos2[0]), int.Parse(pos2[1]), int.Parse(pos2[2])),
                    owner, name, perm, level, true));
            }
            return zoneList;
        }
        public static implicit operator string(ZoneList zoneList)
        {
            var sb = new StringBuilder();
            var array = zoneList.ToArray();
            foreach (var zone in array)
            {
                sb.AppendFormat("{0}:{1}:{2},{3},{4}:{5},{6},{7}:{8}:{9};", zone.Name, zone.Owner,
                                zone.ProtectedZone.Max.x, zone.ProtectedZone.Max.z, zone.ProtectedZone.Max.y,
                                zone.ProtectedZone.Min.x, zone.ProtectedZone.Min.z, zone.ProtectedZone.Min.y,
                                zone.Permission, zone.Level.Name);
            }
            return sb.ToString();
        }
        public static implicit operator List<Zone>(ZoneList zoneList)
        {
            return zoneList._items;
        }
        public override string ToString()
        {
            return this;
        }
    }
}
