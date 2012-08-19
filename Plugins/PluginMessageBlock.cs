using System;
using System.Collections.Generic;
using System.Timers;
using MCForge.Interface.Plugin;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.API.Events;
using MCForge.Core;
using MCForge.World;
using MCForge.Utils.Settings;
using MCForge.Utils;
using System.IO;

namespace Plugins {
    public class PluginMessageBlock : IPlugin {

        public string Name {
            get { return "PluginMessageBlock"; }
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
            CmdMessageBlock mb = new CmdMessageBlock();
            mb.Initialize();
            Level[] lvls = Level.Levels.ToArray();
            Level.OnAllLevelsLoad.Normal += OnAllLevelsLoad_Normal;
            Level.OnAllLevelsUnload.Normal += OnAllLevelsUnload_Normal;
            for (int i = 0; i < lvls.Length; i++)
                OnAllLevelsLoad_Normal(lvls[i], null);
            Player.OnAllPlayersBigMove.Normal += OnAllPlayersBigMove_Normal;
            Player.OnAllPlayersBlockChange.Normal += OnAllPlayersBlockChange_Normal;
            protectBlockType = mb.ProtectBlockType;
            removeCommandOnAir = mb.RemoveCommandOnAir;
            removeMessageOnAir = mb.RemoveMessageOnAir;
        }
        bool protectBlockType = true;
        bool removeCommandOnAir = true;
        bool removeMessageOnAir = true;

        void OnAllPlayersBlockChange_Normal(Player sender, BlockChangeEventArgs args) {
            Vector3S v = new Vector3S(args.X, args.Z, args.Y);
            if (store[sender.Level.Name].Contains(v)) {
                object msg = sender.Level.ExtraData["MessageBlock" + v];
                if (msg != null && msg.GetType() == typeof(string) && ((string)msg).Length > 0) {
                    if (((string)msg).StartsWith("c")) {
                        if (removeCommandOnAir && (args.Action == ActionType.Delete || args.Holding == 0)) {
                            Remove(sender.Level, v);
                        }
                        else if (protectBlockType) {
                            args.Cancel();
                        }
                    }
                    else {
                        if (removeMessageOnAir && (args.Action == ActionType.Delete || args.Holding == 0)) {
                            Remove(sender.Level, v);
                        }
                        else if (protectBlockType) {
                            args.Cancel();
                        }
                    }
                }
            }
        }

        void OnAllPlayersBigMove_Normal(Player sender, MoveEventArgs args) {
            Vector3S v = sender.belowBlock;
            object msg = sender.Level.ExtraData["MessageBlock" + v];
            if (msg != null && msg.GetType() == typeof(string) && ((string)msg).Length > 1) {
                if (((string)msg)[0] == 'm') {
                    sender.SendMessage(((string)sender.Level.ExtraData["MessageBlock" + v]).Substring(1));
                }
                else {
                    if (((string)msg)[0] == 'c') {
                        int perm = 0;
                        string[] commands = new string[0];
                        try {
                            perm = byte.Parse(((string)msg).Split(':')[1]);
                            commands = ((string)msg).Split(':')[2].FromHexString().Split('/');
                        }
                        catch { }
                        if (perm >= sender.Group.Permission) {
                            for (int i = 0; i < commands.Length; i++) {
                                string cname = commands[i].Trim().Split(' ')[0];
                                if (!string.IsNullOrWhiteSpace(cname)) {
                                    if (cname.Length < commands[i].Length) {
                                        string[] cargs = commands[i].Substring(cname.Length + 1).Split(' ');
                                        ICommand c = Command.Find(cname);
                                        try {
                                            if (c != null) c.Use(sender, cargs);
                                        }
                                        catch (Exception e) { Logger.LogError(e); }
                                    }
                                    else {
                                        ICommand c = Command.Find(cname);
                                        try { if (c != null) c.Use(sender, new string[0]); }
                                        catch (Exception e) { Logger.LogError(e); }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void OnUnload() {
            Level[] lvls = Level.Levels.ToArray();
            Player.OnAllPlayersBigMove.Normal -= OnAllPlayersBigMove_Normal;
            Player.OnAllPlayersBlockChange.Normal -= OnAllPlayersBlockChange_Normal;
            Level.OnAllLevelsUnload.Normal -= OnAllLevelsUnload_Normal;
            Level.OnAllLevelsLoad.Normal -= OnAllLevelsLoad_Normal;
            for (int i = 0; i < lvls.Length; i++)
                OnAllLevelsUnload_Normal(lvls[i], null);
            OnAllLevelsUnload_Normal(Server.Mainlevel, null);
        }
        void OnAllLevelsUnload_Normal(Level sender, LevelLoadEventArgs args) {
            List<string> tmp = store[sender.Name];
            if (tmp != null) {
                string join = "";
                for (int i = 0; i < tmp.Count; i++) {
                    if (sender.ExtraData["MessageBlock" + tmp[i]] != null && sender.ExtraData["MessageBlock" + tmp[i]].GetType() == typeof(string) && !String.IsNullOrWhiteSpace((string)sender.ExtraData["MessageBlock" + tmp[i]]))
                        join += tmp[i].ToHexString() + ";";
                }
                if (join.EndsWith(";")) {
                    sender.ExtraData["MessageBlock"] = join.Substring(0, join.Length - 1);
                }
                else if (!String.IsNullOrEmpty(join)) sender.ExtraData["MessageBlock"] = join;
                store[sender.Name] = null;
            }
        }
        ExtraData<string, List<string>> store = new ExtraData<string, List<string>>();
        void OnAllLevelsLoad_Normal(Level sender, LevelLoadEventArgs args) {
            if (sender.ExtraData["MessageBlock"] == null || sender.ExtraData["MessageBlock"].GetType() != typeof(string)) {
                store[sender.Name] = new List<string>();
            }
            else {
                string[] split = ((string)sender.ExtraData["MessageBlock"]).Split(';');
                List<string> tmp = new List<string>();
                for (int i = 0; i < split.Length; i++) {
                    try {
                        Vector3S v = new Vector3S();
                        v.FromHexString(split[i]);
                        if (sender.ExtraData["MessageBlock" + v] != null)
                            tmp.Add(v);
                    }
                    catch { }
                }
                store[sender.Name] = tmp;
            }
        }
        public string[] GetOverview(Level l) {
            List<string> ret = new List<string>();
            foreach (string v in store[l.Name].ToArray()) {
                ret.Add(v + ": " + l.ExtraData["MessageBlock" + v]);
            }
            return ret.ToArray();
        }
        public int RemoveAll(Level l) {
            int count = 0;
            foreach (string v in store[l.Name].ToArray()) {
                l.ExtraData["MessageBlock" + v] = null;
                store[l.Name].Remove(v);
                count++;
            }
            return count;
        }
        public bool Remove(Level l, Vector3S v) {
            if (store[l.Name].Contains(v)) {
                store[l.Name].Remove(v);
                l.ExtraData["MessageBlock" + v] = null;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Returns false if the message was only updated
        /// </summary>
        /// <param name="l"></param>
        /// <param name="v"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Add(Level l, Vector3S v, string message) {
            bool ret = false;
            if (!store[l.Name].Contains(v)) {
                store[l.Name].Add(v);
                ret = true;
            }
            l.ExtraData["MessageBlock" + v] = message;
            return ret;
        }

        public class CmdMessageBlock : ICommand {
            public string Name {
                get { return "MessageBlock"; }
            }

            public CommandTypes Type {
                get { return CommandTypes.Misc; }
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

            public byte Permission {
                get { return Math.Min(Math.Min(createPermission, Math.Min(viewPermission, removeAllPermission)), commandBlockPermission); }
            }

            public void Use(Player p, string[] args) {
                if (p.Group.Permission < Permission) {
                    p.SendMessage("You're not allowed to use /mb");
                    return;
                }
                PluginMessageBlock pmb = (PluginMessageBlock)Plugin.getByType(typeof(PluginMessageBlock).Name);
                if (pmb == null) {
                    p.SendMessage(typeof(PluginMessageBlock).Name + " is currently not loaded");
                    return;
                }
                if (args.Length > 0) {
                    if (args[0] == "+") {
                        if (args.Length == 1) {
                            Help(p);
                            return;
                        }
                        if (args.Length == 2) {
                            if (args[1] == "view") {
                                if (p.Group.Permission < viewPermission) {
                                    p.SendMessage("Your are not allowed to use /mb + view");
                                    return;
                                }
                                string[] ov = pmb.GetOverview(p.Level);
                                for (int i = 0; i < ov.Length; i++) {
                                    p.SendMessage(ov[i]);
                                }
                                return;
                            }
                            else if (args[1] == "remove") {
                                if (p.Group.Permission < createPermission) {
                                    p.SendMessage("Your are not allowed to use /mb + remove");
                                    return;
                                }
                                p.OnPlayerBlockChange.Normal += remove_Normal;
                                return;
                            }
                        }
                        else if (args.Length == 3 && args[1] == "remove") {
                            if (args[2] == "all") {
                                if (p.Group.Permission < removeAllPermission) {
                                    p.SendMessage("You're not allowed to use /mb + all");
                                    return;
                                }
                                int count = pmb.RemoveAll(p.Level);
                                p.SendMessage(count + " message block" + (count == 1 ? "" : "s") + " removed");
                                return;
                            }
                        }
                        Help(p);
                        return;
                    }
                    else {
                        string message = String.Join(" ", args);
                        p.SetDatapass("MessageBlockMessage", message);
                        p.OnPlayerBlockChange.Normal += add_Normal;
                    }
                }
            }

            void remove_Normal(Player sender, BlockChangeEventArgs args) {
                sender.OnPlayerBlockChange.Normal -= remove_Normal;
                args.Cancel();
                Vector3S v = new Vector3S(args.X, args.Z, args.Y);
                PluginMessageBlock pmb = (PluginMessageBlock)Plugin.getByType(typeof(PluginMessageBlock).Name);
                if (pmb == null) {
                    sender.SendMessage(typeof(PluginMessageBlock).Name + " is currently not loaded");
                    return;
                }
                if (pmb.Remove(sender.Level, v)) {
                    sender.SendMessage("Message block removed");
                }
                else {
                    sender.SendMessage("There was already no message set for this block");
                }
            }

            void add_Normal(Player sender, BlockChangeEventArgs args) {
                sender.OnPlayerBlockChange.Normal -= add_Normal;
                args.Cancel();
                Vector3S v = new Vector3S(args.X, args.Z, args.Y);
                PluginMessageBlock pmb = (PluginMessageBlock)Plugin.getByType(typeof(PluginMessageBlock).Name);
                if (pmb == null) {
                    sender.SendMessage(typeof(PluginMessageBlock).Name + " is currently not loaded");
                    return;
                }
                string message = (string)sender.GetDatapass("MessageBlockMessage");
                if (message.StartsWith("/") && sender.Group.Permission >= commandBlockPermission) {
                    message = "c:" + sender.Group.Permission + ":" + message.ToHexString();
                }
                else {
                    message = "m" + message;
                }
                if (pmb.Add(sender.Level, v, message)) {
                    sender.SendMessage("Message block added");
                }
                else {
                    sender.SendMessage("Message block updated");
                }
            }

            public void Help(Player p) {
                p.SendMessage("/mb message - creates a message block");
                p.SendMessage("/mb + view - shows all message blocks on the current level");
                p.SendMessage("/mb + remove - removes a message block on the current level");
                p.SendMessage("/mb + remove all - removes all message blocks on the current level");
            }
            byte createPermission = 0;
            byte viewPermission = 0;
            byte removeAllPermission = 0;
            byte commandBlockPermission = 0;
            public bool ProtectBlockType = true;
            public bool RemoveCommandOnAir = true;
            public bool RemoveMessageOnAir = true;

            public void Initialize() {
                MessageBlockSettings settings = new MessageBlockSettings();
                int f = settings.GetSettingInt("CreatePermission"); //allows to create and remove message blocks
                if (f >= byte.MinValue && f <= byte.MaxValue)
                    createPermission = (byte)f;
                f = settings.GetSettingInt("ViewPermission");//allows view all message blocks
                if (f >= byte.MinValue && f <= byte.MaxValue)
                    viewPermission = (byte)f;
                f = settings.GetSettingInt("DeleteAllPermission");//allows to delete all message blocks
                if (f >= byte.MinValue && f <= byte.MaxValue)
                    removeAllPermission = (byte)f;
                f = settings.GetSettingInt("CommandBlockPermission");
                if (f >= byte.MinValue && f <= byte.MaxValue)
                    commandBlockPermission = (byte)f;
                ProtectBlockType = settings.GetSettingBoolean("ProtectBlockType");
                RemoveCommandOnAir = settings.GetSettingBoolean("RemoveCommandOnAir");
                RemoveMessageOnAir = settings.GetSettingBoolean("RemoveMessageOnAir");

                settings = null;
                Command.AddReference(this, "messageblock", "mb");
            }
        }
        private class MessageBlockSettings : ExtraSettings {
            public MessageBlockSettings()
                : base(typeof(MessageBlockSettings).Name, new SettingNode[]{
             new SettingNode("CreatePermission","0","Allows to create and remove message blocks"),
             new SettingNode("ViewPermission","0","Allows to view all message blocks as a list"),
             new SettingNode("DeleteAllPermission","0","Allows to delete all message blocks at once"),
             new SettingNode("CommandBlockPermission","0","Allows to use commands in a message block"),
             new SettingNode("ProtectBlockType","true","If set to 'false' the block can be changed without affects to the message"),
             new SettingNode("RemoveCommandOnAir","true","'true' removes the block and the command, when 'false' and 'ProtectBlockType' is 'false' the block gets changed and the message removed if 'ProtectBlockType' is 'true' the block gets rebuild"),
             new SettingNode("RemoveMessageOnAir","true","'true' removes the block and the command, when 'false' and 'ProtectBlockType' is 'false' the block gets changed and the message removed if 'ProtectBlockType' is 'true' the block gets rebuild")
            }) {

            }
        }
    }
}
