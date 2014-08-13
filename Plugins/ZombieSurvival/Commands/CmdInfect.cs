using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdInfect : ICommand
    {
        public string Name { get { return "Infect"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 50; } }

        public void Use(Player p, string[] args)
        {
            if (!ZombiePlugin.ZombiePlugin.ZombieRoundEnabled)
            {
                p.SendMessage("You cannot use this right now!");
                return;
            }
            if (args.Length == 0)
            {
                ZombieHelper.InfectPlayer(ZombiePlugin.ZombiePlugin.FindPlayer(p));
                WOM.GlobalSendAlert(p.Username + " force infected themselves");
                return;
            }
            else if (args.Length == 1)
            {
                if (args[0] == "random")
                {
                    ZombieHelper.InfectRandomPlayer(ZombiePlugin.ZombiePlugin.ExtraPlayerData);
                    WOM.GlobalSendAlert(p.Username + " force infected a random player");
                    return;
                }
                else
                {
                    ExtraPlayerData z = ZombiePlugin.ZombiePlugin.FindPlayer(args[0]);
                    if (z != null)
                    {
                        ZombieHelper.InfectPlayer(z);
                        ZombieHelper.DisplayInfectMessage(z.Player, null);
                        WOM.GlobalSendAlert(p.Username + " force infected " + args[0]);
                    }
                    else
                    {
                        p.SendMessage("Player is not online!");
                    }
                    return;
                }
            }
            p.SendMessage("You need to specify a player, random or no player!");
        }

        public void Help(Player p)
        {
            p.SendMessage("/infect - Infects yourself");
            p.SendMessage("/infect <player> - Infects player");
            p.SendMessage("/infect random - Infects a random player");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "infect" });
        }
    }
}