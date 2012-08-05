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
using System.Collections.Generic;
using System.Threading;
using System;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.Utils;
using MCForge.World;
using MCForge.Utils.Settings;
namespace MCForge.Commands {
    public class CmdFly : ICommand {
        public string Name { get { return "Fly"; } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "Gamemakergm, ninedrafted"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }
        public void Use(Player p, string[] args) {
            string IsFlying = (string)p.ExtraData["IsFlying"];
            if (IsFlying != null) {
                p.OnPlayerBigMove.Normal -= OnPlayerBigMove_Normal;
                p.OnPlayerBlockChange.Normal -= OnPlayerBlockChange_Normal;
                if (IsFlying == "+" && glassesFlatAndMiddle != null) {
                    p.ResendBlockChange(glassesFlatAndMiddle, (Vector3S)p.ExtraData["FlyLastPos"]);
                }
                else if (IsFlying == "water" && fluidCube != null) {
                    p.ResendBlockChange(fluidCube, (Vector3S)p.ExtraData["FlyLastPos"]);
                }
                else {
                    p.ResendBlockChange(glassesFlat, (Vector3S)p.ExtraData["FlyLastPos"]);
                }
                p.ResendBlockChange(glassesFlat, (Vector3S)p.ExtraData["FlyLastPos"]);
                p.ExtraData["IsFlying"] = null;
                p.SendMessage(stopFlyMessage);
                return;
            }
            Vector3S belowBlock = p.belowBlock;
            if (args.Length > 0) {
                if (args[0] == "+" && glassesFlatAndMiddle != null) {
                    p.ExtraData["IsFlying"] = "+";
                    p.SendBlockChangeWhereAir(glassesFlatAndMiddle, belowBlock, 20);
                    p.ExtraData["FlyLastPos"] = belowBlock;
                }
                else if (args[0].ToLower() == "water" && fluidCube != null) {
                    p.ExtraData["IsFlying"] = "water";
                    Vector3S tmp = belowBlock;
                    tmp.y++;
                    p.SendBlockChangeWhereAir(fluidCube, tmp, 9);
                    p.ExtraData["FlyLastPos"] = tmp;
                }
                else {
                    p.ExtraData["IsFlying"] = "normal";
                    p.SendBlockChangeWhereAir(glassesFlat, belowBlock, 20);
                    p.ExtraData["FlyLastPos"] = belowBlock;
                }
            }
            else {
                p.ExtraData["IsFlying"] = "normal";
                p.SendBlockChangeWhereAir(glassesFlat, belowBlock, 20);
                p.ExtraData["FlyLastPos"] = belowBlock;
            }
            p.OnPlayerBigMove.Normal += OnPlayerBigMove_Normal;
            p.OnPlayerBlockChange.Normal += OnPlayerBlockChange_Normal;
            p.SendMessage(startFlyMessage);
        }
        static string startFlyMessage = "";
        static string stopFlyMessage = "";
        void OnPlayerBlockChange_Normal(Player sender, API.Events.BlockChangeEventArgs args) {
            if (args.Action == API.Events.ActionType.Delete && args.Current == 0) {
                args.Cancel();
            }
        }

        void OnPlayerBigMove_Normal(Player sender, API.Events.MoveEventArgs args) {
            string IsFlying = (string)sender.ExtraData["IsFlying"];
            if (IsFlying != null) {
                Vector3S lastPos = (Vector3S)sender.ExtraData["FlyLastPos"];
                Vector3S belowBlock = sender.belowBlock;
                if (IsFlying == "+" && glassesFlatAndMiddle != null) {
                    sender.SendReplaceNecessaryBlocksWhere(glassesFlatAndMiddle, lastPos, belowBlock, 0, 20);
                    sender.ExtraData["FlyLastPos"] = belowBlock;
                }
                else if (IsFlying == "water" && fluidCube != null) {
                    Vector3S tmp = belowBlock;
                    tmp.y++;
                    sender.SendReplaceNecessaryBlocksWhere(fluidCube, lastPos, tmp, 0, 9);
                    sender.ExtraData["FlyLastPos"] = tmp;
                }
                else {
                    sender.SendReplaceNecessaryBlocksWhere(glassesFlat, lastPos, belowBlock, 0, 20);
                    sender.ExtraData["FlyLastPos"] = belowBlock;
                }
            }
        }
        public void Help(Player p) {
            p.SendMessage("/fly - Allows you to fly");
            if (glassesFlatAndMiddle != null) p.SendMessage("/fly + - Allows you to fly faster upwards");
            if (fluidCube != null) p.SendMessage("/fly water - Allows you to swimm in the air");
        }
        static Vector3S[] glassesFlat;
        static Vector3S[] glassesFlatAndMiddle;
        static Vector3S[] fluidCube;
        public void Initialize() {
            string message = ServerSettings.GetSetting("StartFlyMessage");
            if (!String.IsNullOrEmpty(message)) startFlyMessage = message;
            message = ServerSettings.GetSetting("StopFlyMessage");
            if (!String.IsNullOrEmpty(message)) stopFlyMessage = message;
            string flyGlassSize = ServerSettings.GetSetting("FlyGlassSize");
            string flyWaterSize = ServerSettings.GetSetting("FlyWaterSize");
            string[] splitGlass = flyGlassSize.Split(' ', ',', ';', ':');
            string[] splitWater = flyWaterSize.Split(' ', ',', ';', ':');
            int xz = 5;
            int y = 2;
            bool midblock = ServerSettings.GetSettingBoolean("Fly+");
            if (splitGlass.Length == 1 && String.IsNullOrWhiteSpace(splitGlass[0])) {
                try { y = int.Parse(splitGlass[0]); }
                catch { }
            }
            else if (splitGlass.Length > 2) {
                try { xz = int.Parse(splitGlass[0]); }
                catch { }
                try { y = int.Parse(splitGlass[1]); }
                catch { }
            }
            List<Vector3S> blocks = new List<Vector3S>();
            for (int a = -xz / 2; a < xz / 2 + ((xz % 2 != 0) ? 1 : 0); a++) {
                for (int b = -xz / 2; b < xz / 2 + ((xz % 2 != 0) ? 1 : 0); b++) {
                    for (int c = 0; c > -y; c--) {
                        blocks.Add(new Vector3S((short)a, (short)b, (short)c));
                    }
                }
            }
            glassesFlat = blocks.ToArray();
            if (midblock) {
                blocks.Add(new Vector3S(0, 0, 1));
                glassesFlatAndMiddle = blocks.ToArray();
            }
            blocks.Clear();
            if (splitWater.Length == 1) {
                try {
                    xz = int.Parse(splitWater[0]);
                    for (int a = -xz / 2; a < xz / 2 + ((xz % 2 != 0) ? 1 : 0); a++) {
                        for (int b = -xz / 2; b < xz / 2 + ((xz % 2 != 0) ? 1 : 0); b++) {
                            for (int c = -xz / 2; c < xz / 2 + ((xz % 2 != 0) ? 1 : 0); c++) {
                                blocks.Add(new Vector3S((short)a, (short)b, (short)c));
                            }
                        }
                    }
                    fluidCube = blocks.ToArray();
                }
                catch { }
            }
            else if (splitWater.Length > 3) {
                try {
                    int x = int.Parse(splitWater[0]);
                    int z = int.Parse(splitWater[1]);
                    y = int.Parse(splitWater[2]);
                    for (int a = -x / 2; a < x / 2 + ((x % 2 != 0) ? 1 : 0); a++) {
                        for (int b = -z / 2; b < z / 2 + ((z % 2 != 0) ? 1 : 0); b++) {
                            for (int c = -y / 2; c < y / 2 + ((y % 2 != 0) ? 1 : 0); y++) {
                                blocks.Add(new Vector3S((short)a, (short)b, (short)c));
                            }
                        }
                    }
                    fluidCube = blocks.ToArray();
                }
                catch { }

            }
            if (fluidCube == null) Logger.Log("/fly water: disabled");
            if (glassesFlatAndMiddle == null) Logger.Log("/fly +: disabled");
            Command.AddReference(this, "fly");
        }
    }
}
