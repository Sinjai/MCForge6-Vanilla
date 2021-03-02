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
using MCForge.Utils;
using System.Collections.Generic;

namespace MCForge.World.Physics
{
    public class Sand : PhysicsBlock
    {
        public override string Name
        {
            get { return "sand"; }
        }
        public override byte VisibleBlock
        {
            get { return 12; }
        }
        public override byte Permission
        {
            get { return 0; }
        }
        public Sand(int x, int z, int y)
            : base(x, z, y)
        {
        }
        public Sand() { }

        public override object Clone()
        {
            Sand g = new Sand();
            g.X = X;
            g.Y = Y;
            g.Z = Z;
            return g;
        }

        public override void Tick(Level l)
        {
            if (l.GetBlock(X, Z, Y - 1) == Block.BlockList.AIR)
            {
                Remove(l);
                Add(l, new Sand(X, Z, Y - 1));
                l.BlockChange((ushort)X, (ushort)Z, (ushort)Y, Block.BlockList.AIR);
                return;
            }
            
           /* bool north = l.GetBlock(X + 1, Z, Y) == Block.BlockList.AIR;
            bool south = l.GetBlock(X - 1, Z, Y) == Block.BlockList.AIR;
            bool east = l.GetBlock(X, Z + 1, Y) == Block.BlockList.AIR;
            bool west = l.GetBlock(X, Z - 1, Y) == Block.BlockList.AIR;
            if (!north && !south && !east && !west)
                return;
            List<Vector2D> card = new List<Vector2D>();
            if (north)
                card.Add(new Vector2D(1, 0));
            if (south)
                card.Add(new Vector2D(-1, 0));
            if (east)
                card.Add(new Vector2D(0, 1));
            if (west)
                card.Add(new Vector2D(0, -1));
            List<Vector2D> diag = new List<Vector2D>();
            if (north && east)
                diag.Add(new Vector2D(1, 1));
            if (south && east)
                diag.Add(new Vector2D(-1, 1));
            if (north && west)
                diag.Add(new Vector2D(1, -1));
            if (south && west)
                diag.Add(new Vector2D(-1, -1));
            List<Vector2D> check = new List<Vector2D>();
            while (card.Count > 0)
            {
                int i = new Random().Next(0, card.Count);
                check.Add(card[i]);
                card.RemoveAt(i);
            }
            while (diag.Count > 0)
            {
                int i = new Random().Next(0, diag.Count);
                check.Add(diag[i]);
                diag.RemoveAt(i);
            }
            
            for (int i = 0; i < check.Count; ++i)
            {
                int x = (int)check[i].x;
                int z = (int)check[i].z;
                int y = x * z == 0 ? 1 : 2;
                if (l.GetBlock(X + x, Z + z, Y - y) == Block.BlockList.AIR && l.GetBlock(X + x, Z + z, Y - y + 1) == Block.BlockList.AIR)
                {
                    Remove(l);
                    Add(l, new Sand(X + x, Z + z, Y - y));
                    l.BlockChange((ushort)X, (ushort)Z, (ushort)Y, Block.BlockList.AIR);
                    return;
                }
            }*/
        }
    }
}
