using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;

namespace MCForge.Commands
{
    public class CmdTime : ICommand
    {
        public string Name { get { return "Time"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }

        public void Use(Player p, string[] args)
        {
            if (!ZombiePlugin.ZombiePlugin.ZombieRoundEnabled && !ZombiePlugin.ZombiePlugin.Voting)
            {
                if (ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed - 1 != 2)
                    p.SendMessage("[Zombie Survival]: " + Colors.red + "Round starts in " + -(ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed - 2) + ":" +
                        (-(ZombiePlugin.ZombiePlugin.AmountOfSecondsElapsed % 60 - 60)).ToString("D2") + " minutes [Gamemode: " + ZombieHelper.GetGamemode(ZombiePlugin.ZombiePlugin.Gamemode) + "]");
            }
            else if (!ZombiePlugin.ZombiePlugin.Voting)
            {
                p.SendMessage("[Zombie Survival]: " + Colors.red + "Time elapsed: " + (ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed - 1) + ":" + (ZombiePlugin.ZombiePlugin.AmountOfSecondsElapsed % 60).ToString("D2"));
                p.SendMessage("[Zombie Survival]: " + Colors.red + "Time remaining: " + (9 - (ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed - 1)) + ":" + (60 - (ZombiePlugin.ZombiePlugin.AmountOfSecondsElapsed % 60)).ToString("D2"));
            }
            else
            {
                p.SendMessage("[Zombie Survival]: " + Colors.red + "Voting time remaining: " + "0:" + (60 - (ZombiePlugin.ZombiePlugin.AmountOfSecondsElapsed % 60)).ToString("D2"));
            }
        }

        public void Help(Player p)
        {
            p.SendMessage("/time - Tells you time information about the round/voting");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "time" });
        }
    }
}