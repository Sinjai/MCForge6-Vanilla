using System;
using System.Collections.Generic;
using System.Linq;

namespace MCForge.Utils {
    public class BlockChangeHistory {
        public static ExtraData<string, MultiChange> history = new ExtraData<string, MultiChange>();
        public static void Add(string level, uint uid, ushort x, ushort z, ushort y, byte newBlock) {
            MultiChange tmp = history[level];
            if (tmp == null) return;
            tmp.Add(x, z, y, uid, newBlock);
        }
        public static IEnumerable<Tuple<ushort, ushort, ushort, byte>> Undo(string level, uint uid, long since) {
            MultiChange tmp = history[level];
            if (tmp == null) yield break;
            else foreach (var ret in tmp.Undo(uid, since)) yield return ret;
        }
        public static void SetLevel(string level, ushort sizeX, ushort sizeZ, ushort sizeY, byte[] originalLevel) {
            history[level] = new MultiChange(sizeX, sizeZ, sizeY, originalLevel);
        }
    }
    struct Time {
        public Time(uint value) {
            this.Value = value;
        }
        public uint Value;
    }
    struct UID {
        public UID(uint value) {
            this.Value = value;
        }
        public uint Value;
    }
    public class SpecialList {
        //TODO: if it never throws exceptions remove them
        List<object> data = new List<object>();
        public void Add(byte type, uint uid) {
            long now = DateTime.Now.Ticks / 50000000; //50'000'000 ns==5s
            bool add;
            if (lastTime < (uint)now) {
                lastTime = (uint)now;
                data.Add(new Time(lastTime));
                add = true;
            }
            else add = false;
            if (lastUID != uid) {
                lastUID = uid;
                data.Add(new UID(lastUID));
                add = true;
            }
            if (add)
                data.Add(type);
            else data[data.Count - 1] = type;
        }
        uint? whenIs(int i) {
            if (i >= data.Count)
                throw new Exception("too high");
            for (; i >= 0 && data[i].GetType() != typeof(Time); i--) ;
            if (i < 0)
                return null;
            return ((Time)data[i]).Value;
        }
        uint? whoIs(int i) {
            if (i >= data.Count)
                throw new Exception("too high");
            for (; i >= 0 && data[i].GetType() != typeof(UID); i--) ;
            if (i < 0)
                return null;
            return ((UID)data[i]).Value;
        }
        /// <summary>
        /// Drops the current UID and its bytes, as well as doubled Times
        /// </summary>
        /// <param name="i">The position of a UID, after execution the position is at the later UID or data.Count</param>
        void drop(ref int i) {
            if (i >= data.Count)
                throw new Exception("too high");
            while (data[i].GetType() == typeof(byte)) i--;
            if (i < 0 || i >= data.Count) throw new Exception("out of bounds");
            if (data[i].GetType() == typeof(UID))
                data.RemoveAt(i);
            for (; i >= 0 && i < data.Count; ) {
                if (data[i].GetType() == typeof(Time)) {
                    i++;
                }
                else if (data[i].GetType() == typeof(UID)) {
                    break;
                }
                else {
                    data.RemoveAt(i);
                }
            }
            while (i >= 2 && data[i - 1].GetType() == typeof(Time) && data[i - 2].GetType() == typeof(Time)) {
                data.RemoveAt(i - 2);
                i--;
            }
            if (i == data.Count && i > 0 && data[i - 1].GetType() == typeof(Time)) {
                data.RemoveAt(i - 1);
                lastTime = 0;
                i--;
            }
            if (i < 0)
                throw new Exception("below range");
            return;
        }
        byte? getPreviousByte(int i) {
            if (i >= data.Count)
                throw new Exception("too high");
            for (; i >= 0 && data[i].GetType() != typeof(byte); i--) ;
            if (i < 0) return null;
            else return (byte)data[i];

        }
        uint lastUID = 0xffffffff;
        uint lastTime = 0;
        public Tuple<byte?, bool> Undo(uint uid, long since) {
            return Undo(uid, (uint)(since / 50000000));
        }

        public int Count { get { return data.Count; } }

        Tuple<byte?, bool> Undo(uint uid, uint since) {
            Tuple<byte?, bool> ret = new Tuple<byte?, bool>(null, true);
            bool firstUID = true;
            for (int i = data.Count - 1; i >= 0; i--) {
                if (data[i].GetType() == typeof(byte)) {
                    uint? cTime = whenIs(i);
                    uint? cUid = whoIs(i);
                    if (cTime == null || cTime < since) {
                        return ret;
                    }
                    if (cUid != null && cUid == uid) {
                        if (firstUID) {
                            ret = new Tuple<byte?, bool>(getPreviousByte(i - 1), false);
                        }
                        drop(ref i);
                    }
                    else if (firstUID) firstUID = false;
                }
            }
            return new Tuple<byte?, bool>(ret.Item1, firstUID);
        }
    }
    public class MultiChange {
        public MultiChange(ushort sizeX, ushort sizeZ, ushort sizeY, byte[] originalLevel) {
            this.originalLevel = (byte[])originalLevel.Clone();
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
            this.sizeY = sizeY;
        }
        ExtraData<ushort, ExtraData<ushort, ExtraData<ushort, SpecialList>>> changes = new ExtraData<ushort, ExtraData<ushort, ExtraData<ushort, SpecialList>>>();
        byte[] originalLevel;
        ushort sizeX, sizeZ, sizeY;
        byte GetBlock(ushort x, ushort z, ushort y) {
            return originalLevel[x + z * sizeX + y * sizeX * sizeZ];
        }

        public void Add(ushort x, ushort z, ushort y, uint uid, byte type) {
            ExtraData<ushort, ExtraData<ushort, SpecialList>> xLevel = changes[x];
            ExtraData<ushort, SpecialList> zLevel;
            SpecialList yLevel;
            if (xLevel == null) {
                xLevel = new ExtraData<ushort, ExtraData<ushort, SpecialList>>();
                zLevel = new ExtraData<ushort, SpecialList>();
                yLevel = new SpecialList();
                yLevel.Add(type, uid);
                zLevel[y] = yLevel;
                xLevel[z] = zLevel;
                changes[x] = xLevel;
            }
            else {
                zLevel = xLevel[z];
                if (zLevel == null) {
                    zLevel = new ExtraData<ushort, SpecialList>();
                    yLevel = new SpecialList();
                    yLevel.Add(type, uid);
                    zLevel[y] = yLevel;
                    xLevel[z] = zLevel;
                }
                else {
                    yLevel = zLevel[y];
                    if (yLevel == null) {
                        yLevel = new SpecialList();
                        yLevel.Add(type, uid);
                        zLevel[y] = yLevel;

                    }
                    else {
                        yLevel.Add(type, uid);
                    }
                }
            }
        }

        public IEnumerable<Tuple<ushort, ushort, ushort, byte>> Undo(uint uid, long since) {
            ExtraData<ushort, ExtraData<ushort, SpecialList>> xLevel;
            ExtraData<ushort, SpecialList> zLevel;
            SpecialList yLevel;
            for (int a = 0; a < changes.Keys.Count; a++) {
                ushort x = changes.Keys.ElementAt(a);
                xLevel = changes[x];
                for (int b = 0; b < xLevel.Keys.Count; b++) {
                    ushort z = xLevel.Keys.ElementAt(b);
                    zLevel = xLevel[z];
                    for (int c = 0; c < zLevel.Keys.Count; c++) {
                        ushort y = zLevel.Keys.ElementAt(c);
                        yLevel = zLevel[y];
                        Tuple<byte?, bool> tmp = yLevel.Undo(uid, since);
                        if (tmp.Item2) {
                            yield return new Tuple<ushort, ushort, ushort, byte>(x, z, y, GetBlock(x, z, y));
                        }
                        else if (tmp.Item1 != null) {
                            yield return new Tuple<ushort, ushort, ushort, byte>(x, z, y, (byte)tmp.Item1);
                        }
                        if (yLevel.Count == 0) {
                            zLevel[y] = null;
                            c--;
                        }
                    }
                    if (zLevel.Count == 0) {
                        xLevel[z] = null;
                        b--;
                    }
                }
                if (xLevel.Count == 0) {
                    changes[x] = null;
                    a--;
                }
            }
            yield break;
        }
    }
}
