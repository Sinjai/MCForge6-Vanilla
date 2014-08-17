using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.Utils;

namespace MCForge.World.Physics {
    public class Door : PhysicsBlock {

        public Door(byte open, byte closed)
        {
            this.open = open;
            this.closed = closed;
        }

        public void SetPos(ushort x, ushort z, ushort y)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Door(byte open, byte closed, int x, int z, int y) {
            this.open = open;
            this.closed = closed;

            this.X = x;
            this.Z = z;
            this.Y = y;

            this.time = 0;
        }
        public override void Tick(Level l) {
            if (level == null) level = l;
            state = true;
            l.SetBlock(l.PosToInt(X, Z, Y), open);
            time += 16;
            if (time >= 16)
            {
                l.SetBlock(l.PosToInt(X, Z, Y), closed);
                state = false;
                time = 0;
            }
        }
        private byte open = 25;
        private byte closed = 21;

        public int time { get; private set; }

        bool state = false;
        bool opened = false;

        Level level;
        public override byte VisibleBlock {
            get {
                if (state) {
                    if (level.ExtraData["DoorOpen" + X + "," + Z + "," + Y] != null) {
                        if (level.ExtraData["DoorOpen" + X + "," + Z + "," + Y].GetType() == typeof(string))
                        {
                            try {
                                level.ExtraData["DoorOpen" + X + "," + Z + "," + Y] = byte.Parse((string)level.ExtraData["DoorOpen" + X + "," + Z + "," + Y]);
                            }
                            catch { }
                        }
                        try { return (byte)level.ExtraData["DoorOpen" + X + "," + Z + "," + Y]; }
                        catch { }
                    }
                    return open;
                }
                else {
                    if (level.ExtraData["DoorClose" + X + "," + Z + "," + Y] != null)
                    {
                        if (level.ExtraData["DoorClose" + X + "," + Z + "," + Y].GetType() == typeof(string))
                        {
                            try {
                                level.ExtraData["DoorClose" + X + "," + Z + "," + Y] = byte.Parse((string)level.ExtraData["DoorOpen" + X + "," + Z + "," + Y]);
                            }
                            catch { }
                        }
                        try { return (byte)level.ExtraData["DoorClose" + X + "," + Z + "," + Y]; }
                        catch { }
                    }
                    return closed;
                }
            }
        }

        public override string Name {
            get { return "door"; }
        }

        public override byte Permission {
            get { return 0; }
        }
        public override object Clone()
        {
            Door d = new Door(open, closed, X, Z, Y);
            d.X = X;
            d.Y = Y;
            d.Z = Z;
            return d;
        }
    }
}
