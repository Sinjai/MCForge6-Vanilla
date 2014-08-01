using System;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils;
using MCForge.Core.RelayChat;

namespace MCForge.Commands.Misc
{
    public class CmdGC : ICommand
    {
        public string Name { get { return "GlobalChat";  } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "UclCommander";  } }
        public int Version { get { return 1; } }
        public string CUD { get { return "";  } }
        public byte Permission { get { return 0; } }

        public void Use(Player p, string[] args)
        {
            if(args.Length > 0)
            {
                string msg = String.Join(" ", args);

                GlobalChat.SendMessage(p, msg);
                Player.UniversalChat(String.Format("[GC] {0}: {1}", p.Username, msg));
                return;
            }

            p.ExtraData.CreateIfNotExist("GlobalChat", true);
            if (!(bool)p.ExtraData["GlobalChat"])
            {
                p.SendMessage("GlobalChat activated. All messages will be sent to GC!");
                p.ExtraData["GlobalChat"] = true;
            }
            else
            {
                p.SendMessage("GlobalChat off!");
                p.ExtraData["GlobalChat"] = false;
            } 
        }

        public void Help(Player p)
        {
            p.SendMessage("/gc [message]");
        }

        public void Initialize()
        {
            Command.AddReference(this, "gc");
        }
    }
}
