using System.Collections.Generic;
using MCForge.Entity;
using MCForge.Utils;
using MCForge.World;

namespace Plugins.Zones
{
    class Zone
    {
        public Cuboid ProtectedZone { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public byte Permission { get; set; }
        public Level Level { get; set; }

        public Zone(Vector3D point1, Vector3D point2, Player owner, string name, byte minimumGroup)
        {
            ProtectedZone = new Cuboid(point1, point2);
            Owner = owner.DisplayName;
            Permission = minimumGroup;
            Zones.Add(this);
            Level = owner.Level;
            Name = name;

            List<Zone> zones;
            if (Level.ExtraData.ContainsKey("zones"))
            {
                zones = (List<Zone>) Level.ExtraData["zones"];
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
            List<Zone> zones;
            Zones.Remove(this);
            if (Level.ExtraData.ContainsKey("zones"))
            {
                zones = (List<Zone>)Level.ExtraData["zones"];
                zones.Remove(this);
            }
            else
            {
                zones = GetAllZonesForLevel(Level);
            }
            Level.ExtraData["zones"] = zones;
   
        }


        private static readonly List<Zone> Zones = new List<Zone>();


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

        public static List<Zone> GetAllZonesForLevel(Level level)
        {
            var zones = new List<Zone>();
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
    }
}
