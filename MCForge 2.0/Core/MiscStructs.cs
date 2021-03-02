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
using MCForge.Entity;
using System.Net;
using System.IO.Compression;

namespace MCForge.Core {

    public static class Colors {
        public const string black = "&0";
        public const string navy = "&1";
        public const string green = "&2";
        public const string teal = "&3";
        public const string maroon = "&4";
        public const string purple = "&5";
        public const string gold = "&6";
        public const string silver = "&7";
        public const string gray = "&8";
        public const string blue = "&9";
        public const string lime = "&a";
        public const string aqua = "&b";
        public const string red = "&c";
        public const string pink = "&d";
        public const string yellow = "&e";
        public const string white = "&f";

        public static string Parse(string str) {
            switch (str.ToLower()) {
                case "black": return black;
                case "navy": return navy;
                case "green": return green;
                case "teal": return teal;
                case "maroon": return maroon;
                case "purple": return purple;
                case "gold": return gold;
                case "silver": return silver;
                case "gray": return gray;
                case "blue": return blue;
                case "lime": return lime;
                case "aqua": return aqua;
                case "red": return red;
                case "pink": return pink;
                case "yellow": return yellow;
                case "white": return white;
                default: return "";
            }
        }
        public static string Name(string str) {
            switch (str) {
                case black: return "black";
                case navy: return "navy";
                case green: return "green";
                case teal: return "teal";
                case maroon: return "maroon";
                case purple: return "purple";
                case gold: return "gold";
                case silver: return "silver";
                case gray: return "gray";
                case blue: return "blue";
                case lime: return "lime";
                case aqua: return "aqua";
                case red: return "red";
                case pink: return "pink";
                case yellow: return "yellow";
                case white: return "white";
                default: return "";
            }
        }
    }

	public struct Packet
	{
		public byte[] bytes;

		#region Constructors
		public Packet(byte[] data)
		{
			bytes = data;
		}
		public Packet(Packet p)
		{
			bytes = p.bytes;
		}
		#endregion
		#region Adds
		public void AddStart(byte[] data)
		{
			byte[] temp = bytes;

			bytes = new byte[temp.Length + data.Length];

			data.CopyTo(bytes, 0);
			temp.CopyTo(bytes, data.Length);
		}

		public void Add(byte[] data)
		{
			if (bytes == null)
			{
				bytes = data;
			}
			else
			{
				byte[] temp = bytes;

				bytes = new byte[temp.Length + data.Length];

				temp.CopyTo(bytes, 0);
				data.CopyTo(bytes, temp.Length);
			}
		}
		public void Add(sbyte a)
		{
			Add(new byte[1] { (byte)a });
		}
		public void Add(byte a)
		{
			Add(new byte[1] { a });
		}
		public void Add(Types a)
		{
			Add((byte)a);
		}
		public void Add(short a)
		{
			Add(HTNO(a));
		}
		public void Add(ushort a)
		{
			Add(HTNO(a));
		}
		public void Add(int a)
		{
			Add(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(a)));
		}
		public void Add(string a)
		{
			Add(a, a.Length);
		}
		public void Add(string a, int size)
		{
			Add(Player.enc.GetBytes(a.PadRight(size).Substring(0, size)));
		}
		#endregion
		#region Sets
		public void Set(int offset, short a)
		{
			HTNO(a).CopyTo(bytes, offset);
		}
		public void Set(int offset, ushort a)
		{
			HTNO(a).CopyTo(bytes, offset);
		}
		public void Set(int offset, string a, int length)
		{
			Player.enc.GetBytes(a.PadRight(length).Substring(0, length)).CopyTo(bytes, offset);
		}
		#endregion

        public void GZip() {
            using (var ms = new System.IO.MemoryStream()) {

                using (var gs = new GZipStream(ms, CompressionMode.Compress, true))
                    gs.Write(bytes, 0, bytes.Length);

                ms.Position = 0;
                bytes = new byte[ms.Length];
                ms.Read(bytes, 0, (int)ms.Length);
            }

        }

        /*public byte[] GetMessage() { //Useless? Just disables packet editing pretty much by removing the first byte :|
            byte[] ret = new byte[bytes.Length - 1];
            Array.Copy(bytes, 1, ret, 0, ret.Length);
            return ret;
        }*/

		#region == Host <> Network ==
		public static byte[] HTNO(ushort x)
		{
			byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
		}
		public static ushort NTHO(byte[] x, int offset)
		{
			byte[] y = new byte[2];
			Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
			return BitConverter.ToUInt16(y, 0);
		}
		public static byte[] HTNO(short x)
		{
			byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
		}
		#endregion

		public enum Types: byte
		{
			Message = 13,
			MOTD = 0,
			MapStart = 2,
			MapData = 3,
			MapEnd = 4,
			SendSpawn = 7,
			SendDie = 12,
			SendBlockchange = 6,
			SendKick = 14,
			SendPing = 1,

			SendPosChange = 10,
			SendRotChange = 11,
			SendPosANDRotChange = 9,
			SendTeleport = 8,
            /// <summary> Extended client/server packet. Initiates CPE negotiation. </summary>
            ExtInfo = 16,

            /// <summary> Extended client/server packet. Lists supported extensions. </summary>
            ExtEntry = 17,

            /// <summary> Extended server packet. Changes player's allowed click distance. </summary>
            SetClickDistance = 18,

            /// <summary> Extended client/server packet. Declares CustomBlocks support level. </summary>
            CustomBlockSupportLevel = 19,

            /// <summary> Extended server packet. Tells client which block to hold. </summary>
            HoldThis = 20,

            /// <summary> Extended server packet. Defines chat macros ties to hotkeys. </summary>
            SetTextHotKey = 21,

            /// <summary> Extended server packet. Adds or updates a name to the player list. </summary>
            ExtAddPlayerName = 22,

            /// <summary> Extended server packet. Adds or updates an entity (replaces AddEntity). </summary>
            ExtAddEntity = 23,

            /// <summary> Extended server packet. Removes a name from the player list. </summary>
            ExtRemovePlayerName = 24,

            /// <summary> Extended server packet. Sets environmental colors (sky/cloud/fog/ambient/diffuse color). </summary>
            EnvSetColor = 25,

            /// <summary> Extended server packet. Adds or updates a selection cuboid. </summary>
            MakeSelection = 26,

            /// <summary> Extended server packet. Removes a selection cuboid. </summary>
            RemoveSelection = 27,

            /// <summary> Extended server packet. Sets permission to place/delete a block type (replaces SetPermission). </summary>
            SetBlockPermission = 28,

            /// <summary> Allows changing the 3D model that entity/player shows up as. </summary>
            ChangeModel = 29,

            /// <summary> This extension allows the server to specify custom terrain textures, and tweak appearance of map edges. </summary>
            EnvMapAppearance = 30,

            /// <summary> This extension allows the server to specify the weather. </summary>
            EnvWeatherType = 31,

            /// <summary> This extension allows the server to specify whichi hacks the player can use. </summary>
            HackControl = 32,
		}
        
	}
    public static class typesHelper {
        public static string ToString(this Packet.Types t) {
            return Enum.GetName(typeof(Packet.Types), t);
        }
    }
}
