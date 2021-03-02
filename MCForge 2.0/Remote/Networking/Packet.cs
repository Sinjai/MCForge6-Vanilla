/*
Copyright 2012 MCForge
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
using System.Text;
using System.Net;
using System.IO;
using MCForge.Remote.Packets;
using MCForge.Remote.Networking;

namespace MCForge.Remote {
    public abstract class Packet {
        //Credits to Sircmpwn for most of the packet code
        #region Types
        /// <summary>
        /// A full list of packets.
        /// </summary>
        private static Type[] Packets = new Type[] {
            typeof(PacketLogin), // 0x00
            typeof(PacketHandShake), // 0x01
            //typeof(PacketRequest), // 0x02
            typeof(PacketMessage), // 0x03
            typeof(PacketPing), // 0x04
            typeof(PacketDisconnect), // 0x05
            //typeof(PacketEdit), // 0x06
            //typeof(PacketCrypto), // 0x07
            typeof(PacketInvalid), // 0x08
            typeof(PacketInvalid), // 0x09
            typeof(PacketInvalid), // 0x0a
            typeof(PacketInvalid), // 0x0b
            typeof(PacketInvalid), // 0x0c
            typeof(PacketInvalid), // 0x0d
            typeof(PacketInvalid), // 0x0e
            typeof(PacketInvalid), // 0x0f
            typeof(PacketInvalid), // 0x21
            typeof(PacketInvalid), // 0x22
            typeof(PacketInvalid), // 0x23
            typeof(PacketInvalid), // 0x24
            typeof(PacketInvalid), // 0x25
            typeof(PacketInvalid), // 0x26
            typeof(PacketInvalid), // 0x27
            typeof(PacketInvalid), // 0x28
            typeof(PacketInvalid), // 0x29
            typeof(PacketInvalid), // 0x2a
            typeof(PacketInvalid), // 0x2b
            typeof(PacketInvalid), // 0x2c
            typeof(PacketInvalid), // 0x2d
            typeof(PacketInvalid), // 0x2e
            typeof(PacketInvalid), // 0x2f
            typeof(PacketInvalid), // 0x30
            typeof(PacketInvalid), // 0x31
            typeof(PacketInvalid), // 0x32
            typeof(PacketInvalid), // 0x33
            typeof(PacketInvalid), // 0x34
            typeof(PacketInvalid), // 0x35
            typeof(PacketInvalid), // 0x36
            typeof(PacketInvalid), // 0x37
            typeof(PacketInvalid), // 0x38
            typeof(PacketInvalid), // 0x39
            typeof(PacketInvalid), // 0x3a
            typeof(PacketInvalid), // 0x3b
            typeof(PacketInvalid), // 0x3c
            typeof(PacketInvalid), // 0x3d
            typeof(PacketInvalid), // 0x3e
            typeof(PacketInvalid), // 0x3f
            typeof(PacketInvalid), // 0x40
            typeof(PacketInvalid), // 0x41
            typeof(PacketInvalid), // 0x42
            typeof(PacketInvalid), // 0x43
            typeof(PacketInvalid), // 0x44
            typeof(PacketInvalid), // 0x45
            typeof(PacketInvalid), // 0x46
            typeof(PacketInvalid), // 0x47
            typeof(PacketInvalid), // 0x48
            typeof(PacketInvalid), // 0x49
            typeof(PacketInvalid), // 0x4a
            typeof(PacketInvalid), // 0x4b
            typeof(PacketInvalid), // 0x4c
            typeof(PacketInvalid), // 0x4d
            typeof(PacketInvalid), // 0x4e
            typeof(PacketInvalid), // 0x4f
            typeof(PacketInvalid), // 0x50
            typeof(PacketInvalid), // 0x51
            typeof(PacketInvalid), // 0x52
            typeof(PacketInvalid), // 0x53
            typeof(PacketInvalid), // 0x54
            typeof(PacketInvalid), // 0x55
            typeof(PacketInvalid), // 0x56
            typeof(PacketInvalid), // 0x57
            typeof(PacketInvalid), // 0x58
            typeof(PacketInvalid), // 0x59
            typeof(PacketInvalid), // 0x5a
            typeof(PacketInvalid), // 0x5b
            typeof(PacketInvalid), // 0x5c
            typeof(PacketInvalid), // 0x5d
            typeof(PacketInvalid), // 0x5e
            typeof(PacketInvalid), // 0x5f
            typeof(PacketInvalid), // 0x60
            typeof(PacketInvalid), // 0x61
            typeof(PacketInvalid), // 0x62
            typeof(PacketInvalid), // 0x63
            typeof(PacketInvalid), // 0x64
            typeof(PacketInvalid), // 0x65
            typeof(PacketInvalid), // 0x66
            typeof(PacketInvalid), // 0x67
            typeof(PacketInvalid), // 0x68
            typeof(PacketInvalid), // 0x69
            typeof(PacketInvalid), // 0x6a
            typeof(PacketInvalid), // 0x6b
            typeof(PacketInvalid), // 0x6c
            typeof(PacketInvalid), // 0x6d
            typeof(PacketInvalid), // 0x6e
            typeof(PacketInvalid), // 0x6f
            typeof(PacketInvalid), // 0x70
            typeof(PacketInvalid), // 0x71
            typeof(PacketInvalid), // 0x72
            typeof(PacketInvalid), // 0x73
            typeof(PacketInvalid), // 0x74
            typeof(PacketInvalid), // 0x75
            typeof(PacketInvalid), // 0x76
            typeof(PacketInvalid), // 0x77
            typeof(PacketInvalid), // 0x78
            typeof(PacketInvalid), // 0x79
            typeof(PacketInvalid), // 0x7a
            typeof(PacketInvalid), // 0x7b
            typeof(PacketInvalid), // 0x7c
            typeof(PacketInvalid), // 0x7d
            typeof(PacketInvalid), // 0x7e
            typeof(PacketInvalid), // 0x7f
            typeof(PacketInvalid), // 0x80
            typeof(PacketInvalid), // 0x81
            typeof(PacketInvalid), // 0x82
            typeof(PacketInvalid), // 0x83
            typeof(PacketInvalid), // 0x84
            typeof(PacketInvalid), // 0x85
            typeof(PacketInvalid), // 0x86
            typeof(PacketInvalid), // 0x87
            typeof(PacketInvalid), // 0x88
            typeof(PacketInvalid), // 0x89
            typeof(PacketInvalid), // 0x8a
            typeof(PacketInvalid), // 0x8b
            typeof(PacketInvalid), // 0x8c
            typeof(PacketInvalid), // 0x8d
            typeof(PacketInvalid), // 0x8e
            typeof(PacketInvalid), // 0x8f
            typeof(PacketInvalid), // 0x90
            typeof(PacketInvalid), // 0x91
            typeof(PacketInvalid), // 0x92
            typeof(PacketInvalid), // 0x93
            typeof(PacketInvalid), // 0x94
            typeof(PacketInvalid), // 0x95
            typeof(PacketInvalid), // 0x96
            typeof(PacketInvalid), // 0x97
            typeof(PacketInvalid), // 0x98
            typeof(PacketInvalid), // 0x99
            typeof(PacketInvalid), // 0x9a
            typeof(PacketInvalid), // 0x9b
            typeof(PacketInvalid), // 0x9c
            typeof(PacketInvalid), // 0x9d
            typeof(PacketInvalid), // 0x9e
            typeof(PacketInvalid), // 0x9f
            typeof(PacketInvalid), // 0xa0
            typeof(PacketInvalid), // 0xa1
            typeof(PacketInvalid), // 0xa2
            typeof(PacketInvalid), // 0xa3
            typeof(PacketInvalid), // 0xa4
            typeof(PacketInvalid), // 0xa5
            typeof(PacketInvalid), // 0xa6
            typeof(PacketInvalid), // 0xa7
            typeof(PacketInvalid), // 0xa8
            typeof(PacketInvalid), // 0xa9
            typeof(PacketInvalid), // 0xaa
            typeof(PacketInvalid), // 0xab
            typeof(PacketInvalid), // 0xac
            typeof(PacketInvalid), // 0xad
            typeof(PacketInvalid), // 0xae
            typeof(PacketInvalid), // 0xaf
            typeof(PacketInvalid), // 0xb0
            typeof(PacketInvalid), // 0xb1
            typeof(PacketInvalid), // 0xb2
            typeof(PacketInvalid), // 0xb3
            typeof(PacketInvalid), // 0xb4
            typeof(PacketInvalid), // 0xb5
            typeof(PacketInvalid), // 0xb6
            typeof(PacketInvalid), // 0xb7
            typeof(PacketInvalid), // 0xb8
            typeof(PacketInvalid), // 0xb9
            typeof(PacketInvalid), // 0xba
            typeof(PacketInvalid), // 0xbb
            typeof(PacketInvalid), // 0xbc
            typeof(PacketInvalid), // 0xbd
            typeof(PacketInvalid), // 0xbe
            typeof(PacketInvalid), // 0xbf
            typeof(PacketInvalid), // 0xc0
            typeof(PacketInvalid), // 0xc1
            typeof(PacketInvalid), // 0xc2
            typeof(PacketInvalid), // 0xc3
            typeof(PacketInvalid), // 0xc4
            typeof(PacketInvalid), // 0xc5
            typeof(PacketInvalid), // 0xc6
            typeof(PacketInvalid), // 0xc7
            typeof(PacketInvalid), // 0xc8
            typeof(PacketInvalid), // 0xc9
            typeof(PacketInvalid), // 0xca
            typeof(PacketInvalid), // 0xcb
            typeof(PacketInvalid), // 0xcc
            typeof(PacketInvalid), // 0xcd
            typeof(PacketInvalid), // 0xce
            typeof(PacketInvalid), // 0xcf
            typeof(PacketInvalid), // 0xd0
            typeof(PacketInvalid), // 0xd1
            typeof(PacketInvalid), // 0xd2
            typeof(PacketInvalid), // 0xd3
            typeof(PacketInvalid), // 0xd4
            typeof(PacketInvalid), // 0xd5
            typeof(PacketInvalid), // 0xd6
            typeof(PacketInvalid), // 0xd7
            typeof(PacketInvalid), // 0xd8
            typeof(PacketInvalid), // 0xd9
            typeof(PacketInvalid), // 0xda
            typeof(PacketInvalid), // 0xdb
            typeof(PacketInvalid), // 0xdc
            typeof(PacketInvalid), // 0xdd
            typeof(PacketInvalid), // 0xde
            typeof(PacketInvalid), // 0xdf
            typeof(PacketInvalid), // 0xe0
            typeof(PacketInvalid), // 0xe1
            typeof(PacketInvalid), // 0xe2
            typeof(PacketInvalid), // 0xe3
            typeof(PacketInvalid), // 0xe4
            typeof(PacketInvalid), // 0xe5
            typeof(PacketInvalid), // 0xe6
            typeof(PacketInvalid), // 0xe7
            typeof(PacketInvalid), // 0xe8
            typeof(PacketInvalid), // 0xe9
            typeof(PacketInvalid), // 0xea
            typeof(PacketInvalid), // 0xeb
            typeof(PacketInvalid), // 0xec
            typeof(PacketInvalid), // 0xed
            typeof(PacketInvalid), // 0xee
            typeof(PacketInvalid), // 0xef
            typeof(PacketInvalid), // 0xf0
            typeof(PacketInvalid), // 0xf1
            typeof(PacketInvalid), // 0xf2
            typeof(PacketInvalid), // 0xf3
            typeof(PacketInvalid), // 0xf4
            typeof(PacketInvalid), // 0xf5
            typeof(PacketInvalid), // 0xf6
            typeof(PacketInvalid), // 0xf7
            typeof(PacketInvalid), // 0xf8
            typeof(PacketInvalid), // 0xf9
            typeof(PacketInvalid), // 0xfa
            typeof(PacketInvalid), // 0xfb
            typeof(PacketInvalid), // 0xfc
            typeof(PacketInvalid), // 0xfd
            typeof(PacketInvalid), // 0xfe
            typeof(PacketInvalid), // 0xff
        };

        #endregion

        
        public static Packet GetPacket ( PacketID pId  ) {
            byte id = ( byte ) pId;
            Type t = Packets[ id ];
            if ( t == typeof( PacketInvalid ) ) {
                return null;
            }
            Packet p = ( Packet ) Activator.CreateInstance( Packets[ ( byte ) pId ] );
            return p;
        }

        private IRemote remote;

        public Packet(IRemote remote) {
            this.remote = remote;
        }

        #region Inherited
        public abstract PacketID PacketID { get; }
        public abstract void ReadPacket ( PacketData data );
        public abstract PacketData WritePacket ();
        #endregion

    }
}

