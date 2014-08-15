/*
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
using System;
using MCForge.API.Events;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils;
using MCForge.World;
using MCForge.World.Blocks;
using MCForge.World.Physics;

namespace MCForge.Commands
{
    public class CmdDoor : ICommand
    {
        public string Name { get { return "Door"; } }
        public CommandTypes Type { get { return CommandTypes.Building; } }
        public string Author { get { return "Hetal"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }
        Door doory = null;
        public void Use(Player p, string[] args)
        {
            if (doory == null)
            {
                Door door = new Door(Block.NameToBlock(args[0]), Block.NameToBlock(args[1]));
                doory = door;
            }
            if (args.Length == 0) { Help(p); return; }
            if (!p.ExtraData.ContainsKey("Mode"))
            {
                p.ExtraData.Add("Door", false);
            }

            if (!(bool)p.ExtraData["Door"])
            {
                Block b = Block.NameToBlock(args[0]);
                Block b1 = Block.NameToBlock(args[1]);
                if (b is UNKNOWN || b1 is UNKNOWN)
                {
                    p.SendMessage("Cannot find block \"" + args[0] + "\"!");
                    return;
                }
                PhysicsBlock.Add(doory);
                if (b.Permission > p.Group.Permission || b1.Permission > p.Group.Permission)
                {
                    p.SendMessage("Cannot place " + StringUtils.TitleCase(b.Name) + "!");
                    return;
                }

                p.ExtraData["Door"] = true;
                if (!p.ExtraData.ContainsKey("BlockDoor"))
                    p.ExtraData.Add("BlockDoor", b);
                p.SendMessage("&b" + StringUtils.TitleCase(b.Name) + Server.DefaultColor + " door mode &9on");
                p.OnPlayerBlockChange.Normal += OnPlayerBlockChangeOnNormal;
                return;
            }
            else
            {
                if (args[0] != ((Block)p.ExtraData["BlockDoor"]).Name)
                {
                    if (doory == null)
                    {
                        Door door = new Door(Block.NameToBlock(args[0]), Block.NameToBlock(args[1]));
                        doory = door;
                    }
                    Block b = Block.NameToBlock(args[0]);
                    Block b1 = Block.NameToBlock(args[1]);
                    if (b is UNKNOWN || b1 is UNKNOWN)
                    {
                        p.SendMessage("Cannot find block \"" + args[0] + "\"!");
                        return;
                    }
                    if (b.Permission > p.Group.Permission || b1.Permission > p.Group.Permission)
                    {
                        p.SendMessage("Cannot place " + StringUtils.TitleCase(b.Name) + "!");
                        return;
                    }

                    p.ExtraData["Door"] = true;
                    p.ExtraData["BlockDoor"] = b;

                    p.SendMessage("&b" + StringUtils.TitleCase(b.Name) + Server.DefaultColor + " mode &9on");
                    return;
                }
                if (!p.ExtraData.ContainsKey("BlockMode"))
                    throw new Exception("No block set in block mode");

                Block prev = (Block)p.ExtraData["BlockMode"];
                p.OnPlayerBlockChange.Normal -= OnPlayerBlockChangeOnNormal;
                doory = null;
                p.SendMessage("&b" + StringUtils.TitleCase(prev.Name) + Server.DefaultColor + " mode &coff");
                p.ExtraData["Mode"] = false;
                p.ExtraData["BlockMode"] = null;
            }
        }

        private void OnPlayerBlockChangeOnNormal(Player sender, BlockChangeEventArgs args)
        {
            bool tried = false;
            if(!tried)
            { doory.Tick(sender.Level); }
            var b = (Block)sender.ExtraData["BlockDoor"];
            if (args.Action == ActionType.Delete) return;
            args.Holding = b;
            var physicsBlock = b as PhysicsBlock;
            if (physicsBlock == null) return;
            sender.Level.pblocks.Add(physicsBlock);
        }

        public void Help(Player p)
        {
            p.SendMessage("/door <block> <door> - makes every placed block turn into the block specified");
            p.SendMessage("/<block> will also work");
        }

        public void Initialize()
        {
            Command.AddReference(this, "door");
        }
    }
}

