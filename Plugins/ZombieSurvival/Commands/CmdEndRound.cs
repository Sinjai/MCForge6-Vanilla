using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdEndRound : ICommand
    {
        public string Name { get { return "EndRound"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 50; } }

        public void Use(Player p, string[] args)
        {
            if (!ZombiePlugin.ZombiePlugin.Voting)
            {
                if (!ZombiePlugin.ZombiePlugin.ZombieRoundEnabled)
                {
                    p.SendMessage("You cannot use this right now!");
                    return;
                }
            }
            p.SendMessage("[Zombie Survival]: " + Colors.red + "Ending the round!");
            WOM.GlobalSendAlert(p.Username + " ended the round");
            if (!ZombiePlugin.ZombiePlugin.Voting)
                ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed = 10;
            if (ZombiePlugin.ZombiePlugin.Voting)
                ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed = 2;
        }

        public void Help(Player p)
        {
            p.SendMessage("/endround - ends the round early");
            p.SendMessage("also ends voting early, if required");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "endround" });
        }
    }
}