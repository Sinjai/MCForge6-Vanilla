using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdStartRound : ICommand
    {
        public string Name { get { return "StartRound"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 50; } }

        public void Use(Player p, string[] args)
        {
            if (ZombiePlugin.ZombiePlugin.ZombieRoundEnabled) {
                p.SendMessage("You cannot use this right now!");
                return;
            }
            p.SendMessage("[Zombie Survival]: " + Colors.red + "Starting the round!");
            WOM.GlobalSendAlert(p.Username + " started the round");
            ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed = 1;
            ZombiePlugin.ZombiePlugin.AmountOfSecondsElapsed = 59;
        }

        public void Help(Player p)
        {
            p.SendMessage("/startround - starts the round early");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "startround" });
        }
    }
}