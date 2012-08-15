using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;

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
        public static void WriteOut(string level, bool clear) {
            MultiChange tmp = history[level];
            if (tmp != null) {
                FileStream fs = new FileStream("levels/" + level + ".history", FileMode.Create, FileAccess.Write);
                GZipStream gs = new GZipStream(fs, CompressionMode.Compress);
                BinaryWriter bw = new BinaryWriter(gs);
                tmp.Write(bw);
                fs.Flush();
                fs.Close();
                if (clear) history[level] = null;
            }
        }
        /// <summary>
        /// Loads the history data
        /// </summary>
        /// <param name="level">The name of the level</param>
        /// <returns>Whether or not the history could have been loaded.</returns>
        public static bool Load(string level) {
            if (File.Exists("levels/" + level + ".history")) {
                FileStream fs = new FileStream("levels/" + level + ".history", FileMode.Open, FileAccess.Read);
                GZipStream gs = new GZipStream(fs, CompressionMode.Decompress);
                BinaryReader br = new BinaryReader(gs);
                history[level] = MultiChange.Read(br);
                fs.Close();
                return true;
            }
            return false;
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
            byte current = (byte)data[data.Count - 1];
            for (int i = data.Count - 1; i >= 0; i--) {
                if (data[i].GetType() == typeof(byte)) {
                    uint? cTime = whenIs(i);
                    uint? cUid = whoIs(i);
                    if (cTime == null || cTime < since) {
                        if (ret.Item1 == current) return new Tuple<byte?, bool>(null, false);
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
            if (ret.Item1 == current) return new Tuple<byte?, bool>(null, firstUID);
            return new Tuple<byte?, bool>(ret.Item1, firstUID);
        }
        TypeByte determine(int i) {
            if (data[i].GetType() == typeof(UID)) {
                byte count = 0;
                for (; count <= TypeByte.MaxAmount; count++) {
                    if (i + count + 1 < data.Count && data[i + count].GetType() == typeof(UID) && data[i + count + 1].GetType() == typeof(byte))
                        ;
                    else break;
                }
                if (count == 0) throw new Exception("corrupted list?");
                return new TypeByte(count, 0);
            }
            else {
#if DEBUG
                if (data[i].GetType() == typeof(byte)) throw new Exception("corrupted list??");
#endif
                if (i + 1 < data.Count && data[i + 1].GetType() == typeof(byte)) {
                    byte count = 0;
                    for (; count <= TypeByte.MaxAmount; count++) {
                        if (i + count + 1 < data.Count && data[i + count].GetType() == typeof(Time) && data[i + count + 1].GetType() == typeof(byte))
                            ;
                        else break;
                    }
                    if (count == 0) throw new Exception("corrupted list?");
                    return new TypeByte(count, 1);
                }
                else if (i + 2 < data.Count && data[i + 1].GetType() == typeof(UID) && data[i + 2].GetType() == typeof(byte)) {
                    if (i + 3 >= data.Count || data[i + 3].GetType() == typeof(UID)) {
                        byte count = 0;
                        i++;
                        for (; count <= TypeByte.MaxAmount; count++) {
                            if (i + count + 1 < data.Count && data[i + count].GetType() == typeof(UID) && data[i + count + 1].GetType() == typeof(byte))
                                ;
                            else break;
                        }
                        if (count == 0) throw new Exception("corrupted list?");
                        return new TypeByte(count, 2);
                    }
                    else {
                        byte count = 0;
                        for (; count <= TypeByte.MaxAmount; count++) {
                            if (i + count + 3 < data.Count && data[i + count].GetType() == typeof(Time) && data[i + count + 1].GetType() == typeof(UID) && data[i + count + 1].GetType() == typeof(byte))
                                ;
                            else break;
                        }
                        if (count == 0) throw new Exception("corrupted list?");
                        return new TypeByte(count, 3);

                    }
                }
                else throw new Exception("corrupted list?????");
                /*
0x00 multiple uid>byte
0x01 multiple time>byte
0x02 multiple time>0x00 (time>[uid>byte]
0x03 multiple time>uid>byte
                 */
            }
        }
        public void Write(BinaryWriter bw) {
            bw.Write(data.Count);
            for (int i = 0; i < data.Count; ) {
                TypeByte tb = determine(i);
                int amount;
                bw.Write(tb);
                switch (tb.head) {
                    case 0:
                        amount = tb.amount;
                        for (; amount > 0; amount--) {
                            bw.Write((uint)((UID)data[i]).Value);
                            bw.Write((byte)data[i + 1]);
                            i += 2;
                        }
                        break;
                    case 1:
                        amount = tb.amount;
                        for (; amount > 0; amount--) {
                            bw.Write((uint)((Time)data[i]).Value);
                            bw.Write((byte)data[i + 1]);
                            i += 2;
                        }
                        break;
                    case 2:
                        amount = tb.amount;
                        bw.Write((uint)((Time)data[i]).Value);
                        i++;
                        for (; amount > 0; amount--) {
                            bw.Write((uint)((UID)data[i]).Value);
                            bw.Write((byte)data[i + 1]);
                            i += 2;
                        }
                        break;
                    case 3:
                        amount = tb.amount;
                        for (; amount > 0; amount--) {
                            bw.Write((uint)((Time)data[i]).Value);
                            bw.Write((uint)((UID)data[i + 1]).Value);
                            bw.Write((byte)data[i + 2]);
                            i += 3;
                        }
                        break;
                }
            }
        }
        public static SpecialList Read(BinaryReader br) {
            SpecialList ret = new SpecialList();
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++) {
                TypeByte tb = br.ReadByte();
                int amount;
                switch (tb.head) {
                    case 0:
                        amount = tb.amount;
                        for (; amount > 0; amount--) {
                            ret.data.Add(new UID(br.ReadUInt32()));
                            ret.data.Add(br.ReadByte());
                            i += 2;
                        }
                        break;
                    case 1:
                        amount = tb.amount;
                        for (; amount > 0; amount--) {
                            ret.data.Add(new Time(br.ReadUInt32()));
                            ret.data.Add(br.ReadByte());
                            i += 2;
                        }
                        break;
                    case 2:
                        amount = tb.amount;
                        ret.data.Add(new Time(br.ReadUInt32()));
                        i++;
                        for (; amount > 0; amount--) {
                            ret.data.Add(new UID(br.ReadUInt32()));
                            try {
                                ret.data.Add(br.ReadByte());
                            }
                            catch {
                                i = i;
                            }
                            i += 2;
                        }
                        break;
                    case 3:
                        amount = tb.amount;
                        for (; amount > 0; amount--) {
                            ret.data.Add(new Time(br.ReadUInt32()));
                            ret.data.Add(new UID(br.ReadUInt32()));
                            ret.data.Add(br.ReadByte());
                            i += 3;
                        }
                        break;
                }
            }
            return ret;
        }
        struct TypeByte {
            private TypeByte(byte b) {
                this.data = b;
            }
            public TypeByte(byte data, byte head) {
                this.data = (byte)((data << headsize) + ((head << shift) >> shift));
            }
            static TypeByte() {
                shift = 8 - headsize;
                MaxAmount = byte.MaxValue >> headsize;
            }
            byte data;
            public static implicit operator byte(TypeByte tb) {
                return tb.data;
            }
            public static implicit operator TypeByte(byte b) {
                TypeByte tb = new TypeByte(b);
                return tb;
            }
            const int headsize = 2;
            public byte head {
                get {
                    return (byte)(((byte)(data << shift)) >> shift);
                }
                set {
                    data = (byte)(((data >> headsize) << headsize) + ((value << shift) >> shift));
                }
            }
            public byte amount {
                get {
                    return (byte)(data >> headsize);
                }
                set {
                    data = (byte)((value << headsize) + head);
                }
            }

            private static int shift;
            public static int MaxAmount;
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
        long Count() {
            long ret = 0;
            foreach (var a in changes)
                foreach (var b in a.Value)
                    ret += b.Value.Keys.Count;
            return ret;
        }
        public void Write(BinaryWriter bw) {
            ExtraData<ushort, ExtraData<ushort, SpecialList>> xLevel;
            ExtraData<ushort, SpecialList> zLevel;
            SpecialList yLevel;
            bw.Write(originalLevel.Length);
            bw.Write(sizeX);
            bw.Write(sizeZ);
            bw.Write(sizeY);
            bw.Write(originalLevel);
            bw.Write(Count());
            for (int a = 0; a < changes.Keys.Count; a++) {
                ushort x = changes.Keys.ElementAt(a);
                xLevel = changes[x];
                for (int b = 0; b < xLevel.Keys.Count; b++) {
                    ushort z = xLevel.Keys.ElementAt(b);
                    zLevel = xLevel[z];
                    for (int c = 0; c < zLevel.Keys.Count; c++) {
                        ushort y = zLevel.Keys.ElementAt(c);
                        yLevel = zLevel[y];
                        bw.Write(x); bw.Write(z); bw.Write(y);
                        yLevel.Write(bw);
                    }
                }
            }
        }
        public static MultiChange Read(BinaryReader br) {
            long count = br.ReadInt32();
            ushort x = br.ReadUInt16();
            ushort z = br.ReadUInt16();
            ushort y = br.ReadUInt16();
            byte[] origLvl = br.ReadBytes((int)count);
            MultiChange ret = new MultiChange(x, z, y, origLvl);
            count = br.ReadInt64();
            for (; count > 0; count--) {
                x = br.ReadUInt16();
                z = br.ReadUInt16();
                y = br.ReadUInt16();
                ExtraData<ushort, ExtraData<ushort, SpecialList>> xLevel = ret.changes[x];
                ExtraData<ushort, SpecialList> zLevel;
                if (xLevel == null) {
                    xLevel = new ExtraData<ushort, ExtraData<ushort, SpecialList>>();
                    zLevel = new ExtraData<ushort, SpecialList>();
                    zLevel[y] = SpecialList.Read(br);
                    xLevel[z] = zLevel;
                    ret.changes[x] = xLevel;
                }
                else {
                    zLevel = xLevel[z];
                    if (zLevel == null) {
                        zLevel = new ExtraData<ushort, SpecialList>();
                        zLevel[y] = SpecialList.Read(br);
                        xLevel[z] = zLevel;
                    }
                    else {
                        zLevel[y] = SpecialList.Read(br);
                    }
                }
            }
            return ret;
        }
    }
}
