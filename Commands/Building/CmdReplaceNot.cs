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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.Core;
using MCForge.World;

namespace CommandDll
{
    public class CmdReplaceNot : ICommand
    {
        public string Name { get { return "ReplaceNot"; } }
        public CommandTypes Type { get { return CommandTypes.building; } }
        public string Author { get { return "Gamemakergm"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 80; } }

        public void Use(Player p, string[] args)
        {
            byte type = 0;
            byte type2 = 0;
            if (args.Length != 2)
            {
                p.SendMessage("Invalid arguments!");
                Help(p);
                return;
            }
            if (!Blocks.ValidBlockName(args[0 | 1]))
            {
                p.SendMessage("Could not find block specified");
            }

            //Block permissions here.
            CatchPos cpos = new CatchPos();
            cpos.type = type;
            cpos.type2 = type2;

            p.SendMessage("Place two blocks to determine the edges.");
            p.CatchNextBlockchange(new Player.BlockChangeDelegate(CatchBlock), (object)cpos);
        }
        public void CatchBlock(Player p, ushort x, ushort z, ushort y, byte NewType, bool placed, object DataPass)
        {
            CatchPos cpos = (CatchPos)DataPass;
            cpos.pos = new Vector3(x, z, y);
            p.CatchNextBlockchange(CatchBlock2, (object)cpos);
        }
        public void CatchBlock2(Player p, ushort x, ushort z, ushort y, byte NewType, bool placed, object DataPass)
        {
            CatchPos FirstBlock = (CatchPos)DataPass;
            List<Pos> buffer = new List<Pos>();

            for (ushort xx = Math.Min((ushort)(FirstBlock.pos.x), x); xx <= Math.Max((ushort)(FirstBlock.pos.x), x); ++xx)
            {
                for (ushort zz = Math.Min((ushort)(FirstBlock.pos.z), z); zz <= Math.Max((ushort)(FirstBlock.pos.z), z); ++zz)
                {
                    for (ushort yy = Math.Min((ushort)(FirstBlock.pos.y), y); yy <= Math.Max((ushort)(FirstBlock.pos.y), y); ++yy)
                    {
                        Vector3 loop = new Vector3(xx, zz, yy);
                        if (p.level.GetBlock(loop) != FirstBlock.type)
                        {
                            BufferAdd(buffer, loop);
                        }
                    }
                }
            }
            //Group Max Blocks permissions here
            p.SendMessage(buffer.Count.ToString() + " blocks.");

            //Level Blockqueue .-.

            buffer.ForEach(delegate(Pos pos)
            {
                p.level.BlockChange((ushort)(pos.pos.x), (ushort)(pos.pos.z), (ushort)(pos.pos.y), FirstBlock.type2);
            });
        }
        public void Help(Player p)
        {
            p.SendMessage("/replacenot [block] [block2] - Replaces everything but <block> with <block2> inside a selected cuboid.");
        }

        public void Initialize()
        {
            string[] CommandStrings = new string[2] { "replacenot", "rn" };
            Command.AddReference(this, CommandStrings);
        }
        void BufferAdd(List<Pos> list, Vector3 type)
        {
            Pos pos;
            pos.pos = type;
            list.Add(pos);
        }
        private struct CatchPos
        {
            public byte type;
            public byte type2;
            public Vector3 pos;
        }
        struct Pos
        {
            public Vector3 pos;
        }
    }
}
