using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.Interface.Command;
using MCForge.Entity;
using MCForge.Utils;
namespace MCForge.Commands.Moderation {
    public class CmdHighlight : ICommand {
        public string Name {
            get { return "Highlight"; }
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
            get {
                return 0;
            }
        }

        public void Use(Entity.Player p, string[] args) {
            long since = 0;
            long uid = -1;
            if (args.Length == 0) uid = p.UID;
            else uid = Player.GetUID(args[0]);
            if (uid == -1) {
                p.SendMessage("Player not found");
                return;
            }
            if (args.Length > 1) {
                try {
                    since = DateTime.Now.AddSeconds(-int.Parse(args[1])).Ticks;
                }
                catch (Exception e) {
                    if (e.GetType() == typeof(FormatException))
                        p.SendMessage("Not supported number format");
                    else if (e.GetType() == typeof(OverflowException))
                        p.SendMessage("Your number was out of range");
                }
            }
            foreach (var block in BlockChangeHistory.GetCurrentIfUID(p.Level.Name, (uint)uid, since)) {
                if (block.Item4 == 0) {
                    p.SendBlockChange(block.Item1, block.Item2, block.Item3, MCForge.World.Block.NameToBlock("red"));
                }
                else {
                    p.SendBlockChange(block.Item1, block.Item2, block.Item3, MCForge.World.Block.NameToBlock("green"));
                }
            }
            p.SendMessage("Highlighting " + ((uid == p.UID) ? p.DisplayName : args[0]));
        }

        public void Help(Entity.Player p) {
            p.SendMessage("/highlight <name> [time]  - highlights changes of a player");
        }

        public void Initialize() {
            Command.AddReference(this, "highlight");
        }
    }
}
