using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using MCForge.Utils.Settings;
using ZombiePlugin;
using System;
using MCForge.Utils;
using System.IO;

namespace MCForge.Commands
{
    public class CmdReferee : ICommand
    {
        public string Name { get { return "Referee"; } }
        public CommandTypes Type { get { return CommandTypes.Custom; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 40; } }

        public void Use(Player p, string[] args)
        {
            ExtraPlayerData temp = ZombiePlugin.ZombiePlugin.FindPlayer(p);
            temp.Referee = !temp.Referee;
            if (temp.Referee)
                temp.DateTimeAtStartOfRef = DateTime.Now.Ticks;
            p.SendMessage("Referee mode is now " + temp.Referee.ToString().Replace("True", "on!").Replace("False", "off!"));
            if (!p.IsHidden)
                p.GlobalDie();

            p.IsHidden = !p.IsHidden;

            if (!p.IsHidden)
                p.SpawnThisPlayerToOtherPlayers();
            ZombieHelper.SendUserType(temp);
        }

        public void Help(Player p)
        {
            p.SendMessage("/referee - Enter referee mode");
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[2] { "ref", "referee" });
        }
    }
}