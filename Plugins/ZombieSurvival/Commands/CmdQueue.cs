using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using System.IO;
using ZombiePlugin;
using System.Collections;

namespace MCForge.Commands
{
    public class CmdQueue : ICommand
    {
        public string Name { get { return "Queue"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 50; } }

        public void Use(Player p, string[] args)
        {
            if (args.Length < 2 || args.Length > 2)
            {
                p.SendMessage("You need to specify an option and a choice!");
                return;
            }
            if (args[0] == "level")
            {
                ArrayList al = new ArrayList();
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.cw");
                foreach (FileInfo fil in fi)
                {
                    al.Add(fil.Name.Split('.')[0]);
                }
                if (al.Contains(args[1]))
                {
                    ZombiePlugin.ZombiePlugin.LevelNameQueued = args[1];
                    ZombiePlugin.ZombiePlugin.LevelQueued = true;
                    p.SendMessage(args[1] + " was queued (it will silently be chosen next round!)");
                    WOM.GlobalSendAlert(p.Username + " queued the level " + args[1]);
                }
                else
                {
                    p.SendMessage("You need to specify a proper level name!");
                    return;
                }
            }
            else if (args[0] == "zombie")
            {
                if (args[0] == null)
                {
                    p.SendMessage("You need to specify a player!");
                    return;
                }
                ExtraPlayerData s = ZombiePlugin.ZombiePlugin.FindPlayer(args[1]);
                if (s == null)
                {
                    p.SendMessage("Could not find player :(");
                    return;
                }
                ZombiePlugin.ZombiePlugin.ZombieNameQueued = args[1];
                ZombiePlugin.ZombiePlugin.ZombieQueued = true;
                p.SendMessage(s.Player.Username + " was queued!");
                WOM.GlobalSendAlert(p.Username + " queued the zombie " + args[1]);
            }
            else if (args[0] == "gamemode")
            {
                int i = int.Parse(args[1]);
                if (i < 0 || i > 3)
                {
                    p.SendMessage("You need to specify a gamemode!");
                    return;
                }
                ZombiePlugin.ZombiePlugin.GameModeIntQueued = i;
                ZombiePlugin.ZombiePlugin.GameModeQueued = true;
                p.SendMessage(ZombiePlugin.ZombieHelper.GetGamemode(i) + " was queued!");
                WOM.GlobalSendAlert(p.Username + " queued the gamemode " + args[1]);
            }
            else
            {
                p.SendMessage("You must select a valid option!");
            }
        }

        public void Help(Player p)
        {
            p.SendMessage("/queue level [level] - Queues [level] to be selected next");
            p.SendMessage("/queue zombie [zombie] - Queues [zombie] to be selected next (Doesn't apply on Normal gamemode)");
            p.SendMessage("/queue gamemode [gamemode] - Queues [gamemode] to be selected next");
            p.SendMessage("0 for Normal, 1 for Classic, 2 for Classic Happy, 3 for cure");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[2] { "queue", "que" });
        }
    }
}