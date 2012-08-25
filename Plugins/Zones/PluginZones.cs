using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.API.Events;
using MCForge.Entity;
using MCForge.Interface.Plugin;
using MCForge.Utils;

namespace Plugins.Zones
{
    class PluginZones : IPlugin
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
                if (!zone.CanBuildIn(sender))
                {
                    sender.SendMessage("You cannot build in this Zone!");
                    args.Cancel();
                    return;
                }
            }
        }

        public void OnUnload()
        {
            Player.OnAllPlayersBlockChange.High -= OnAllPlayersBlockChangeOnHigh;
        }
    }
}
