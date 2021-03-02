using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;

namespace CTF
{
    public class CmdJoin : ICommand
    {
        public string Name { get { return "join"; } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "Hetal"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 80; } }
        public void Use(Player p, string[] message)
        {
            if (ServerCTF.CTFModeOn)
            {
                if (p == null) { p.SendMessage("This command can only be used in-game!"); return; }
                if (ServerCTF.ctfGameStatus == 0) { p.SendMessage("There is no CTF game currently in progress."); return; }
                if (!ServerCTF.ctfRound) { p.SendMessage("The current ctf round hasn't started yet!"); return; }
                int diff = ServerCTF.ctf.red.Count() - ServerCTF.ctf.blu.Count();
                bool unbalanced = false;
                Random random = new Random();
                if (message[0] == "blue")
                {
                    if (p.Level.Name == ServerCTF.ctf.currentLevelName) return;
                    if ((int)p.ExtraData["team"] == 0)
                    {
                        int a = random.Next(-2, -1);
                        if (diff <= a)
                            unbalanced = true;
                        if (unbalanced)
                        {
                            p.SendMessage(Colors.gray + " - " + Server.DefaultColor + "You have been autobalanced!" + Colors.gray + " - ");
                            ServerCTF.ctf.joinTeam(p, "red");
                        }
                        ServerCTF.ctf.joinTeam(p, "blue");
                    }
                }
                else
                {
                    if (p.Level.Name == ServerCTF.ctf.currentLevelName) return;
                    if ((int)p.ExtraData["team"] == 0)
                    {
                        int a = random.Next(1, 2);
                        if (diff >= a)
                            unbalanced = true;
                        if (unbalanced)
                        {
                            p.SendMessage(Colors.gray + " - " + Server.DefaultColor + "You have been autobalanced!" + Colors.gray + " - ");
                            ServerCTF.ctf.joinTeam(p, "blue");
                        }
                        ServerCTF.ctf.joinTeam(p, "red");
                    }
                }
            }
        }
        public void Help(Player p)
        {
            if (ServerCTF.CTFModeOn)
            {
                p.SendMessage("Joins a CTF Team, valid options are red and blue");
            }
        }
        public void Initialize()
        {
            Command.AddReference(this, new string[1] { "join" });
        }
    }
}
