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
using MCForge.API.Events;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Groups;
using MCForge.Utils;
using MCForge.World;

namespace Plugins.Zones
{
    public class CmdZone : ICommand
    {
        public string Name { get { return "Zone"; } }
        public CommandTypes Type { get { return CommandTypes.Mod;} }
        public string Author { get { return "cazzar"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return (byte) PermissionLevel.Operator; } }
        public void Use(Player p, string[] args)
        {
            if (args.Length < 1)
            {
                Help(p);
                return;
            }
            switch (args[0].ToLower())
            {
                case "add":
                case "a":
                    if (args.Length != 3)
                    {
                        Help(p);
                        return;
                    }
                    if (Zone.GetZoneByName(args[1]) != null)
                    {
                        p.SendMessage("Zone by that name already exists!");
                        return;
                    }

                    byte b = 0;
                    try
                    {
                        b = byte.Parse(args[2]);
                    }
                    catch
                    {
                        try
                        {
                            b = PlayerGroup.Find(args[2]);
                        }
                        catch
                        {
                            p.SendMessage("Error parsing permission");
                            return;
                        }
                    }
                    if (b > p.Group.Permission)
                    {
                        p.SendMessage("Cannot create a zone with higher permission then yourself!");
                        return;
                    }
                    p.SendMessage("Please place the first block to define the region");
                    p.OnPlayerBlockChange.Normal += OnPlayerBlockChangeOnNormal;

                    var data = new KeyValuePair<string, byte>(args[1], b);
                    p.SetDatapass(Name, data);
                    break;
                case "delete":
                case "del":
                case "d":
                    if (args.Length > 2)
                    {
                        Help(p);
                        return;
                    }
                    if (args.Length == 2)
                    {
                        var zone = Zone.GetZoneByName(args[1]);
                        if (zone == null)
                        {
                            p.SendMessage("Zone not found!");
                            return;
                        }
                        if (zone.Permission > p.Group.Permission)
                        {
                            p.SendMessage("You cannot delete this zone!");
                            return;
                        }
                        zone.Delete();
                        p.SendMessage("Zone " + zone.Name + " has been deleted");
                    }
                    
                    p.OnPlayerBlockChange.Normal += (sender, eventArgs) =>
                                                        {
                                                            eventArgs.Cancel();
                                                            eventArgs.Unregister();
                                                            var pos = new Vector3D(eventArgs.X, eventArgs.Z, eventArgs.Y);
                                                            var zoneList = Zone.FindAllWithin(pos);
                                                            Zone zone = null;

                                                            if (zoneList.Count > 1)
                                                            {
                                                                sender.SendMessage("Too many zones found");
                                                                sender.SendMessage("Use /zone del <zone name> to delete the zone");
                                                                return;
                                                            }
                                                            if (zoneList.Count < 1)
                                                            {
                                                                sender.SendMessage("No zone found!");
                                                                return;
                                                            }
                                                            zone = zoneList[0];

                                                            if (zone.Permission > p.Group.Permission)
                                                            {
                                                                sender.SendMessage("You cannot delete this zone!");
                                                                return;
                                                            }
                                                            zone.Delete();
                                                            p.SendMessage("Zone " + zone.Name + " has been deleted");
                                                        };
                    break;
                case "list":
                case "l":
                    Level l = null;
                    if (args.Length == 2)
                        l = Level.FindLevel(args[1]);
                    if (l == null)
                        l = p.Level;
                    var zones = Zone.GetAllZonesForLevel(l);

                    var s = String.Join(", ", zones);

                    p.SendMessage("Zones for Level " + l.Name + ": " + s);
                    break;
                case "info":
                case "i":
                     if (args.Length > 2)
                    {
                        Help(p);
                        return;
                    }
                    if (args.Length == 2)
                    {
                        var zone = Zone.GetZoneByName(args[1]);
                        if (zone == null)
                        {
                            p.SendMessage("Zone not found!");
                            return;
                        }
                        p.SendMessage("Zone name: &f" + zone.Name);
                        p.SendMessage("Zone minimum permission: &0" + zone.Permission);
                        p.SendMessage("Zone Owner: &f" + zone.Owner);
                        p.SendMessage("Zone Level: &5" + zone.Level.Name);
                        p.SendMessage("Can you build in this zone? " + ((zone.CanBuildIn(p)) ? "&9yes": "&4no"));
                    }
                    
                    p.OnPlayerBlockChange.Normal += (sender, eventArgs) =>
                                                        {
                                                            eventArgs.Cancel();
                                                            eventArgs.Unregister();
                                                            var pos = new Vector3D(eventArgs.X, eventArgs.Z, eventArgs.Y);
                                                            var zoneList = Zone.FindAllWithin(pos);
                                                            Zone zone = null;

                                                            if (zoneList.Count > 1)
                                                            {
                                                                sender.SendMessage("Too many zones found");
                                                                sender.SendMessage("Use /zone del <zone name> to delete the zone");
                                                                return;
                                                            }
                                                            if (zoneList.Count < 1)
                                                            {
                                                                sender.SendMessage("No zone found!");
                                                                return;
                                                            }
                                                            zone = zoneList[0];
                                                            if (zone == null)
                                                            {
                                                                p.SendMessage("Zone not found!");
                                                                return;
                                                            }

                                                            sender.SendMessage("Zone name: &f" + zone.Name);
                                                            sender.SendMessage("Zone minimum permission: &0" + zone.Permission);
                                                            sender.SendMessage("Zone Owner: &f" + zone.Owner);
                                                            sender.SendMessage("Zone Level: &5" + zone.Level.Name);
                                                            sender.SendMessage("Can you build in this zone? " + ((zone.CanBuildIn(sender)) ? "&9yes" : "&4no"));
                                                        };
                    break;
            }
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
            new Zone(data.Value, new Vector3D(args.X, args.Z, args.Y), sender, data.Key.Key, data.Key.Value);
        }

        public void Help(Player p)
        {
            p.SendMessage("/zone add [zone name] [permission] - adds a zone");
            p.SendMessage("Aliases: a");
            p.SendMessage("");
            p.SendMessage("/zone delete <zone name> - deletes a zone");
            p.SendMessage("If zone name is not defined Block placement is used for selecting the zone");
            p.SendMessage("Aliases: d, del");
            p.SendMessage("");
            p.SendMessage("/zone list <Level> - list all zones");
            p.SendMessage("If no level is defined the zones shown will be your current map");
            p.SendMessage("Aliases: l");
            p.SendMessage("");
            p.SendMessage("/zone info <zone name> - gets the info on the zone");
            p.SendMessage("If zone name is not defined Block placement is used for selecting the zone");
            p.SendMessage("Aliases: l");
        }

        public void Initialize()
        {
            Command.AddReference(this, "zone");
        }
    }
}
