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
using System.Collections.Generic;
using MCForge.Utils;
using MCForge.World;

namespace MCForge.Robot {
    public class BotMap {
        public BotMap(Level l) {
            AirMap = new TriBool[l.CWMap.Size.x, l.CWMap.Size.z, l.CWMap.Size.y];//return x + z * Size.x + y * Size.x * Size.z;
            Size = l.CWMap.Size;
            for (int i = 0; i < l.CWMap.BlockData.Length; i++)
            {
                Vector3S pos = l.IntToPos(i);
                if (isAir(l.GetBlock(i)))
                    AirMap[pos.x, pos.z, pos.y] = true;
                else if (Block.IsOPBlock(l.GetBlock(i)))
                    AirMap[pos.x, pos.z, pos.y] = TriBool.Unknown;
                else
                    AirMap[pos.x, pos.z, pos.y] = false;
            }
            /*for (int x = 0; x < AirMap.GetLength(0); x++) {
                for (int z = 0; z < AirMap.GetLength(1); z++) {
                    for (int y = 0; y < AirMap.GetLength(2); y++) {

                    }
                }
            }*/
        }
        public TriBool[, ,] AirMap;
        public Vector3S Size = new Vector3S(0, 0, 0);
        bool[, ,] PosMap;
        bool isAir(byte block) {
            return Block.CanWalkThrough(block);
        }
        bool onlyAirBetween(Vector3S start, Vector3S end) {
            Vector3D s = new Vector3D(start);
            Vector3D e = new Vector3D(end);
            while ((s - e).Length > 1) {
                if (AirMap[(int)s.x, (int)s.z, (int)s.y] == TriBool.True) return false;
            }
            return true;
        }
        List<Vector3D> fromList=new List<Vector3D>();
        List<Vector3D> toList=new List<Vector3D>();
        int Add(Vector3D from, Vector3D to) {
            fromList.Add(from);
            toList.Add(to);
            return fromList.Count;
        }
        class WayPoint {
            public Vector3D Position;
            public List<Vector3D> Connecteds = new List<Vector3D>();
            public List<double> Distances = new List<double>();
        } 
    }
}
