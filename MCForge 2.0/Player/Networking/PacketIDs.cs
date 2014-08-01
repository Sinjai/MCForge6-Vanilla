using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Networking {
    public enum PacketIDs : byte {
        Identification = 0x00,
        Ping = 0x01,
        LevelInitialize = 0x02,
        LevelDataChunk = 0x03,
        LevelFinalize = 0x04,
        PlayerSetBlock = 0x05,
        ServerSetBlock = 0x06,
        SpawnPlayer = 0x07,
        PosAndRot = 0x08,
        PosAndRotUpdate = 0x09,
        PosUpdate = 0x0a,
        RotUpdate = 0x0b,
        DespawnPlayer = 0x0c,
        Message = 0x0d,
        KickPlayer = 0x0e,
        UpdateUserType = 0x0f,
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
