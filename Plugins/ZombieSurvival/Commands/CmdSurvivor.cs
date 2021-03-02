using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdSurvivor : ICommand
    {
        public string Name { get { return "Survivor"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 20; } }

        public void Use(Player p, string[] args)
        {
            if (!ZombiePlugin.ZombiePlugin.ZombieRoundEnabled)
            {
                p.SendMessage("You cannot use this right now!");
                return;
            }
            if (ZombiePlugin.ZombiePlugin.ZombieRoundEnabled)
            {
                p.SendMessage("[Zombie Survival]: " + Colors.red + "You cannot toggle survivor while a round is in progress!");
                return;
            }
            ExtraPlayerData z = ZombiePlugin.ZombiePlugin.FindPlayer(p);
            z.Survivor = !z.Survivor;
            p.SendMessage("Survivor mode is now " + z.Survivor.ToString().Replace("True", "on!").Replace("False", "off!"));
        }

        public void Help(Player p)
        {
            p.SendMessage("/survivor - Toggles survivor mode ");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "survivor" });
        }
    }
}