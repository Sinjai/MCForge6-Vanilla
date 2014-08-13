using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdInfected : ICommand
    {
        public string Name { get { return "Infected"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }

        public void Use(Player p, string[] args)
        {
            if (!ZombiePlugin.ZombiePlugin.ZombieRoundEnabled)
            {
                p.SendMessage("You cannot use this right now!");
                return;
            }
            string bla = "";
            foreach (ExtraPlayerData d in ZombiePlugin.ZombiePlugin.ExtraPlayerData.ToArray())
            {
                if (d.Infected)
                    bla = bla + Colors.red + d.Player.Username + ", ";
            }
            if (bla.Length < 2)
            {
                p.SendMessage("No one is infected!");
                return;
            }
            p.SendMessage("Players who are infected: " + bla.Remove(bla.Length - 2, 2));
        }

        public void Help(Player p)
        {
            p.SendMessage("/infected - Shows players who are infected");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "infected" });
        }
    }
}