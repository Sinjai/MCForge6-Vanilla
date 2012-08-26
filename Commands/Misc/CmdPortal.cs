using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils.Settings;
using MCForge.Utils;
using System.Collections.Generic;
using System;
namespace MCForge.Commands.Misc {
    public class CmdPortal : ICommand {
        public string Name {
            get { return "Portal"; }
        }

        public CommandTypes Type {
            get { return CommandTypes.Misc; }
        }

        public string Author {
            get { return "ninedrafted"; }
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

        public void Use(Player p, string[] args) {
            if (p.Group.Permission >= Permission) {
                p.SendMessage("Build as many blocks as you want for the portal entry, break a block to set the exit");
                p.OnPlayerBlockChange.Low += OnPlayerBlockChange_Low;
            }
            else {
                p.SendMessage("Your not allowed to create portals");
            }
        }

        void OnPlayerBlockChange_Low(Player sender, API.Events.BlockChangeEventArgs args) {
            if (!args.Canceled && args.Action == API.Events.ActionType.Delete) {
                object tmp = sender.ExtraData["TmpBlockList"];
                if (tmp != null && ((List<Tuple<string, Vector3S>>)tmp).Count != 0) {
                    List<Tuple<string, Vector3S>> tmplist = (List<Tuple<string, Vector3S>>)tmp;
                    string level = sender.Level.Name;
                    string tpLocal = "/tp " + (args.X * 32 + 16) + " " + (args.Z * 32 + 16) + " " + ((args.Y + 1) * 32 + 16);
                    string tpLevel = "/tp " + (args.X * 32 + 16) + " " + (args.Z * 32 + 16) + " " + ((args.Y + 1) * 32 + 16) + " " + level;
                    ICommand mb = Command.Find("messageblock");
                    if (mb == null) {
                        Logger.Log("MessageBlock command and plugin are required to create portals");
                        sender.SendMessage("MessageBlock command and plugin are required to create portals");
                        args.Cancel();
                        return;
                    }
                    foreach (Tuple<string, Vector3S> e in tmplist) {
                        List<string> mbArgs = new List<string>() { "+", e.Item2.x + "", e.Item2.z + "", e.Item2.y + "", e.Item1 };
                        if (e.Item1 == level) {
                            mbArgs.AddRange(tpLocal.Split(' '));
                        }
                        else {
                            mbArgs.AddRange(tpLevel.Split(' '));
                        }
                        mb.Use(sender, mbArgs.ToArray());
                    }
                    sender.SendMessage("Portal created");
                    sender.OnPlayerBlockChange.Low -= OnPlayerBlockChange_Low;

                }

            }
            else {
                object tmp = sender.ExtraData["TmpBlockList"];
                if (tmp == null) {
                    tmp = new List<Tuple<string, Vector3S>>();
                    sender.ExtraData["TmpBlockList"] = tmp;
                }
                ((List<Tuple<string, Vector3S>>)tmp).Add(new Tuple<string, Vector3S>(sender.Level.Name, new Vector3S(args.X, args.Z, args.Y)));
            }
        }

        public void Help(Player p) {
            p.SendMessage("/portal - creates a portal, players stepping on it will get teleported");
        }

        public void Initialize() {
            Command.AddReference(this, "portal");
        }
    }
}
