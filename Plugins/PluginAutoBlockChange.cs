using System;
using System.Collections.Generic;
using System.Timers;
using MCForge.Interface.Plugin;
using MCForge.Entity;
using MCForge.API.Events;
using MCForge.Core;
using MCForge.World;
using MCForge.Utils.Settings;
using System.IO;


namespace Plugins {
    public class PluginAutoBlockChange : IPlugin {
        public string Name {
            get { return "AutoBlockChange"; }
        }

        public string Author {
            get { return "ninedrafted"; }
        }

        public int Version {
            get { return 1; }
        }

        public string CUD {
            get { return ""; }
        }

        public void OnLoad(string[] args) {
            //usin Low priority allows commands to do their work with the original blocks first
            Player.OnAllPlayersBlockChange.Low += OnAllPlayersBlockChange_Low;
            AutoBlockChangeSettings settings = new AutoBlockChangeSettings();
            //TODO: proper settings class implementation and proper usage
            string str = settings.GetSetting("AutoGrass");
            try {
                autoGrass = bool.Parse(str);
            }
            catch { }
            str = settings.GetSetting("AutoDirt");
            try { autoDirt = bool.Parse(str); }
            catch { }
            str = settings.GetSetting("AutoDoubleStair");
            try { autoDoubleStair = bool.Parse(str); }
            catch { }
            settings = null;

        }
        bool autoGrass = true;
        bool autoDirt = true;
        bool autoDoubleStair = true;
        void OnAllPlayersBlockChange_Low(Player sender, BlockChangeEventArgs args) {
            if (args.Canceled) return;
            if (args.Action == ActionType.Place) {
                #region Dirt/Grass
                if (autoGrass && args.Holding == Block.BlockList.DIRT && Block.CanPassLight(sender.Level.GetBlock(args.X, args.Z, args.Y + 1))) args.Holding = Block.BlockList.GRASS;
                if (autoDirt && args.Holding != Block.BlockList.AIR && sender.Level.GetBlock(args.X, args.Z, args.Y - 1) == Block.BlockList.GRASS)
                    sender.Level.BlockChange(args.X, args.Z, (ushort)(args.Y - 1), Block.BlockList.DIRT, sender);
                #endregion

                if (autoDoubleStair && args.Holding == Block.BlockList.STAIR && sender.Level.GetBlock(args.X, args.Z, args.Y - 1) == Block.BlockList.STAIR) {
                    args.Cancel();
                    sender.Level.BlockChange(args.X, args.Z, (ushort)(args.Y - 1), Block.BlockList.DOUBLE_STAIR, sender);
                }
            }
            else {
                if (autoGrass && sender.Level.GetBlock(args.X, args.Z, args.Y - 1) == Block.BlockList.DIRT)
                    sender.Level.BlockChange(args.X, args.Z, (ushort)(args.Y - 1), Block.BlockList.GRASS, sender);
            }
        }

        public void OnUnload() {
            Player.OnAllPlayersBlockChange.Low -= OnAllPlayersBlockChange_Low;
        }

        private class AutoBlockChangeSettings : ExtraSettings {
            public override string SettingsName {
                get { return this.GetType().Name; }
            }

            private List<SettingNode> values = new List<SettingNode>();
            public override List<SettingNode> Values {
                get { return values; }
            }

            public override void OnLoad() {

            }

            public override void Save() {

            }

            public override string PropertiesPath {
                get { return this.SettingsName; }
            }
        }
    }
}
