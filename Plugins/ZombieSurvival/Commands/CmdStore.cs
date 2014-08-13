using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface;
using MCForge.Interface.Plugin;
using ZombiePlugin;
using MCForge.Utils;
using MCForge.Groups;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;

namespace MCForge.Commands
{
    public class CmdStore : ICommand
    {
        public string Name { get { return "Store"; } }
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
            ExtraPlayerData P = ZombiePlugin.ZombiePlugin.FindPlayer(p);
            p.SendMessage(Colors.red + "-----------------" + Server.DefaultColor + "Zombie Store" + Colors.red + "---------------");
            if (args.Length < 1)
            {
                Help(p);
                p.SendMessage(Colors.red + "-----------------" + Server.DefaultColor + "Zombie Store" + Colors.red + "---------------");
                return;
            }
            
            if (args[0] == "1")
            {
                if ((ZombiePlugin.ZombiePlugin.ZombieRoundEnabled || ZombiePlugin.ZombiePlugin.AmountInfected() > 1 || ZombiePlugin.ZombiePlugin.AmountOfMinutesElapsed - 1 > 8) && P.Infected)
                {
                    if (p.Money >= 25)
                    {
                        ZombieHelper.TakeMoney(p, 25);
                        ZombieHelper.DisinfectPlayer(P);
                    }
                    else
                    {
                        p.SendMessage("You do not have enough money for this!");
                    }
                }
                else
                {
                    p.SendMessage("You can not use this right now!");
                }
            }
            else if (args[0] == "2")
            {
                int message12 = 5;
                try
                {
                    message12 = int.Parse(args[1]);
                }
                catch {}
                if (p.Money >= (Math.Round(0.2 * message12)))
                {
                    if (message12 < 5)
                        message12 = 5;
                    P.AmountOfBlocksLeft += message12;
                    p.Money = p.Money - (Convert.ToInt32(Math.Round(0.2 * message12)));
                    p.SendMessage("You now have " + Colors.maroon + P.AmountOfBlocksLeft + Server.DefaultColor + " blocks!");
                }
                else
                {
                    p.SendMessage("You do not have enough " + Server.Moneys);
                }
            }
            else if (args[0] == "3")
            {
                if(P.Infected)
                {
                    p.SendMessage("&9Zombies may not use this item..");
                    return;
                }
                p.IsHidden = true;
                p.GlobalDie();
                Player.UniversalChat(p.Color + p.Username + "&ahas &0vanished!");
                Thread.Sleep(10000);
                Player.UniversalChat(p.Color + p.Username + "&ahas &0appeared");
                p.IsHidden = false;
                p.SpawnThisPlayerToOtherPlayers();
            }
            else if (args[0] == "4")
            {
                if (args.Length >= 2)
                {
                    if (args[1] == "del")
                    {
                        p.ExtraData.ChangeOrCreate("InfectMessage", "");
                        p.ExtraData.Save(p, "InfectMessage");
                        p.SendMessage("Cleared infect message!");
                    }
                    else if (p.Money >= 400)
                    {
                        var strs = new List<string>(args);
                        strs.RemoveAt(0);
                        ZombieHelper.TakeMoney(p, 400);
                        p.ExtraData.ChangeOrCreate("InfectMessage", String.Join(" ", strs.ToArray()));
                        p.ExtraData.Save(p, "InfectMessage");
                        p.SendMessage("Changed infect message to " + String.Join(" ", strs.ToArray()) + "!");
                    }
                    else
                    {
                        p.SendMessage("You do not have enough money for this!");
                    }
                }
                else
                {
                    Help(p);
                }
            }
            else if (args[0] == "5")
            {
                if (args.Length >= 2)
                {
                    if (args[1] == "del")
                    {
                        p.ExtraData.ChangeOrCreate("DisinfectMessage", "");
                        p.ExtraData.Save(p, "DisinfectMessage");
                        p.SendMessage("Cleared disinfect message!");
                    }
                    else if (p.Money >= 400)
                    {
                        var strs = new List<string>(args);
                        strs.RemoveAt(0);
                        ZombieHelper.TakeMoney(p, 400);
                        p.ExtraData.ChangeOrCreate("DisinfectMessage", String.Join(" ", strs.ToArray()));
                        p.ExtraData.Save(p, "DisinfectMessage");
                        p.SendMessage("Changed disinfect message to " + String.Join(" ", strs.ToArray()) + "!");
                    }
                    else
                    {
                        p.SendMessage("You do not have enough money for this!");
                    }
                }
                else
                {
                    Help(p);
                }
            }
            
            p.SendMessage(Colors.red + "-----------------" + Server.DefaultColor + "Zombie Store" + Colors.red + "---------------");
        }

        public void Help(Player p)
        {
            p.SendMessage("To purchase an item, type /buy [item number] (eg. /buy 4 for a rank)");
            p.SendMessage("Use ! to represent yourself, * to represent dis/infectee");
            p.SendMessage(Colors.blue + "1. " + Server.DefaultColor + "Disinfectant - 135 " + Server.Moneys);
            p.SendMessage(Colors.blue + "2. " + Server.DefaultColor + "Extra Blocks - 0.2 " + Server.Moneys + " Per Block (Rounds to nearest noodle, minimum 5 blocks)");
            p.SendMessage(Colors.blue + "4. " + Server.DefaultColor + "Infect Message - 400 " + Server.Moneys + " (Use del to clear message)");
            p.SendMessage(Colors.blue + "5. " + Server.DefaultColor + "Disinfect Message - 400 " + Server.Moneys + " (Use del to clear message)");
        }

        public int GetRankAmount(Player p, int MediumHeight)
        {
            return (int)(50 * Math.Sin(0.0625 * p.Money) + MediumHeight);
        }

        public void Initialize()
        {
            Command.AddReference(this, new string[2] { "store", "buy" });
        }
    }
}