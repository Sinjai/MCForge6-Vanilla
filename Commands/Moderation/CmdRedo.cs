using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.World;
using MCForge.Utils;

namespace MCForge.Commands.Moderation {
    public class CmdRedo :ICommand{
        public string Name {
            get { return "Redo"; }
        }

        public CommandTypes Type {
            get { return CommandTypes.Mod; }
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
            get { return 0; }
        }


        public void Use(Entity.Player p, string[] args) {
            int time = 30;
            long uid = -1;
            Level where = null;
            if (args.Length == 1) {
                try { time = int.Parse(args[0]); }
                catch { p.SendMessage("The time was incorrect, using 30 seconds instead"); }
            }
            if (args.Length == 2) {
                uid = Player.GetUID(args[0]);
                if (uid == -1) {
                    p.SendMessage("Player not found");
                    return;
                }
                try { time = int.Parse(args[1]); }
                catch { p.SendMessage("The time was incorrect, using 30 seconds instead"); }
            }
            if (args.Length == 3) {
                uid = Player.GetUID(args[0]);
                if (uid == -1) {
                    p.SendMessage("Player not found");
                    return;
                }
                try { time = int.Parse(args[1]); }
                catch { p.SendMessage("The time was incorrect, using 30 seconds instead"); }
                where = Level.FindLevel(args[2]);
                if (where == null) {
                    p.SendMessage("Level " + args[2] + " does not exist or is not loaded");
                    return;
                }
            }
            if (uid == -1) uid = p.UID;
            if(where==null)where=p.Level;
            int count = 0;
            foreach (var ch in RedoHistory.Redo((uint)uid, where.Name, DateTime.Now.AddSeconds(-time).Ticks)) {
                where.BlockChange(ch.Item1, ch.Item2, ch.Item3, ch.Item4, p);
                count++;
            }
            p.SendMessage("&e" + count + MCForge.Core.Server.DefaultColor + " Blocks changed");
        }

        public void Help(Entity.Player p) {
            p.SendMessage("/redo [time] redos undid changes whitin the last [time] seconds on the current map for you");
            p.SendMessage("/redo [name] [time] [map] redos changes for another player on selected map");
            p.SendMessage("/redo [name] [time] redos changes for another player on your current map");
            p.SendMessage("Note: The new blocks will look like you built them, not the other player");
        }

        public void Initialize() {
            Command.AddReference(this, this.Name.ToLower());
        }
    }
}
