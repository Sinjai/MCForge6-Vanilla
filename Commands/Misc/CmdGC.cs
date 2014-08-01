using MCForge.Core.RelayChat;
using MCForge.Entity;
using MCForge.Interface.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands.Misc
{
    class CmdGC : ICommand
    {
        public string Name { get { return "GlobalChat";  } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "UclCommander";  } }
        public int Version { get { return 1; } }
        public string CUD { get { return "";  } }
        public byte Permission { get { return 0; } }

        public void Use(Player p, string[] args)
        {
            GlobalChat.SendMessage(p, String.Join(" ", args));
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
