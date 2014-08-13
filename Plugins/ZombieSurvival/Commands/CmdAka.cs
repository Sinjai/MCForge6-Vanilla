using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using MCForge.Utils.Settings;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdAka : ICommand
    {
        public string Name { get { return "Aka"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }

        public void Use(Player p, string[] args)
        {
            ExtraPlayerData z = ZombiePlugin.ZombiePlugin.FindPlayer(p);
            z.Aka = !z.Aka;
            foreach (Player e in Server.Players.ToArray())
            {
                Packet pa = new Packet(new byte[2] { (byte)Packet.Types.SendDie, e.ID });
                if (p != e)
                {
                    p.SendPacket(pa);
                }
            }
            p.SpawnOtherPlayersForThisPlayer();
            p.SendMessage("Aka mode is now " + z.Aka.ToString().Replace("True", "on!").Replace("False", "off!"));
        }

        public void Help(Player p)
        {
            p.SendMessage("/aka - Converts Undeaad to players usernames");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "aka" });
        }
    }
}