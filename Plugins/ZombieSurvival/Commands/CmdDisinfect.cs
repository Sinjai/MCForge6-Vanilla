using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;
using MCForge.Utils;

namespace MCForge.Commands
{
    public class CmdDisinfect : ICommand
    {
        public string Name { get { return "Disinfect"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 50; } }

        public void Use(Player p, string[] args)
        {
            if (!ZombiePlugin.ZombiePlugin.ZombieRoundEnabled || ZombiePlugin.ZombiePlugin.AmountInfected() <= 1 || ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed - 1 > 8)
            {
                p.SendMessage("You cannot use this right now!");
                return;
            }
            if (args.Length == 0 && ZombiePlugin.ZombiePlugin.AmountInfected() > 1)
            {
                ZombieHelper.DisinfectPlayer(ZombiePlugin.ZombiePlugin.FindPlayer(p));
                WOM.GlobalSendAlert(p.Username + " force disinfected themselves");
                return;
            }
            else if (args.Length == 1 && ZombiePlugin.ZombiePlugin.AmountInfected() > 1)
            {
                if (args[0] == "random")
                {
                    ZombieHelper.DisinfectRandomPlayer(ZombiePlugin.ZombiePlugin.ExtraPlayerData);
                    WOM.GlobalSendAlert(p.Username + " force disinfected a random player");
                    return;
                }
                else
                {
                    ExtraPlayerData z = ZombiePlugin.ZombiePlugin.FindPlayer(args[0]);
                    if (z != null && z.Infected)
                    {
                        ZombieHelper.DisinfectPlayer(z);
                        ZombieHelper.DisplayDisinfectMessage(z.Player, null);
                        WOM.GlobalSendAlert(p.Username + " force disinfected " + args[0]);
                    }
                    else if (!z.Infected)
                    {
                        p.SendMessage("You cannot use this right now!");
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
            p.SendMessage("/disinfect - Disinfects yourself");
            p.SendMessage("/disinfect <player> - Disinfects player");
            p.SendMessage("/disinfect random - Disinfects a random player");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "disinfect" });
        }
    }
}