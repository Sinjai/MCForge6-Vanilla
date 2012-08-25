using System.Collections.Generic;
using System.Linq;
using MCForge.Entity;
using MCForge.Utils;

namespace Plugins.Zones
{
    class Zone
    {
        public Cuboid ProtectedZone { get; set; }
        public string Owner { get; set; }
        public byte Permission { get; set; }

        public Zone(Vector3D point1, Vector3D point2, Player owner, byte minimumGroup)
        {
            ProtectedZone = new Cuboid(point1, point2);
            Owner = owner.DisplayName;
            Permission = minimumGroup;
            Zones.Add(this);
        }

        public bool CanBuildIn(Player player)
        {
            return Permission <= player.Group.Permission;
        }


        private static readonly List<Zone> Zones = new List<Zone>();


        public static IEnumerable<Zone> FindAllWithin(Vector3S pos)
        {
            var zones = new List<Zone>();
            foreach (var zone in Zones)
            {
                if (zone.ProtectedZone.Within(pos))
                    zones.Add(zone);
            }
            return zones;
        }
        public static Zone FindWithin(Vector3S pos)
        {
            foreach (var zone in Zones)
            {
                if (zone.ProtectedZone.Within(pos))
                    return zone;
            }
            return null;
        }
    }
}
