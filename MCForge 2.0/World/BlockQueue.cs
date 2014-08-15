/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Timers;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Utils;

namespace MCForge.World
{
    public static class BlockQueue
    {
        public static int time { get { return (int)blocktimer.Interval; } set { blocktimer.Interval = value; } }
        public static int blockupdates = 200;
        static block b = new block();
        static physblock phys = new physblock();
        static System.Timers.Timer blocktimer = new System.Timers.Timer(100);
        static byte started = 0;
        public static void Start()
        {
            blocktimer.Elapsed += delegate
            {
                if (started == 1) return;
                started = 1;
                World.Level.Levels.ForEach((l) =>
                {
                    try
                    {
                        if (l.blockqueue.Count < 1) return;
                        int count;
                        if (l.blockqueue.Count < blockupdates || l.Players.Count == 0) count = l.blockqueue.Count;
                        else count = blockupdates;

                        for (int c = 0; c < count; c++)
                        {
                            l.BlockChange((ushort)l.blockqueue[c].x, (ushort)l.blockqueue[c].z, (ushort)l.blockqueue[c].y, (byte)l.blockqueue[c].type, l.blockqueue[c].p, false);
                        }
                        l.blockqueue.RemoveRange(0, count);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e);
                        Logger.Log(String.Format("Block cache failed for map: {0}. {1} lost.", l.Name, l.blockqueue.Count));
                        l.blockqueue.Clear();
                    }
                    try
                    {
                        if (l.physqueue.Count < 1) return;
                        int count;
                        if (l.physqueue.Count < blockupdates || l.Players.Count == 0) count = l.blockqueue.Count;
                        else count = blockupdates;

                        for (int c = 0; c < count; c++)
                        {
                            l.BlockChange((ushort)l.physqueue[c].x, (ushort)l.physqueue[c].z, (ushort)l.physqueue[c].y, (byte)l.physqueue[c].type, null, false);
                        }
                        l.physqueue.RemoveRange(0, count);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e);
                        Logger.Log(String.Format("Block cache (physics) failed for map: {0}. {1} lost.", l.Name, l.physqueue.Count));
                        l.physqueue.Clear();
                    }
                });
                started = 0;
            };
            blocktimer.Start();
        }
        public static void pause() { blocktimer.Enabled = false; }
        public static void resume() { blocktimer.Enabled = true; }

        public static void Addblock(Player P, ushort X, ushort Y, ushort Z, byte type)
        {
            if (P == null)
            {
                phys.x = X;
                phys.y = Y;
                phys.z = Z;
                phys.type = type;
            }
            else
            {
                b.x = X;
                b.y = Y;
                b.z = Z;
                b.type = type;
                b.p = P;
                P.Level.blockqueue.Add(b);
            }
        }

        public struct block { public Player p; public ushort x; public ushort y; public ushort z; public byte type; }
        public struct physblock { public ushort x; public ushort y; public ushort z; public byte type; }
    }
}
