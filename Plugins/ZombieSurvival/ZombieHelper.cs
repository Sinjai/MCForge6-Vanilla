using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Drawing;
using ZombiePlugin;
using MCForge.Entity;
using MCForge.Core;
using MCForge.Utils;
using MCForge.World;
using MCForge.Robot;
using MCForge.SQL;
using System.Text.RegularExpressions;

namespace ZombiePlugin
{
    public static class ZombieHelper
    {
        public static Random Random = new Random();
        public static List<string> InfectMessages = new List<string>();
        public static List<string> DisinfectMessages = new List<string>();
        public static List<string> NoBotLevels = new List<string>();
        public static List<String> SwearFilter = new List<string>();
        public static Dictionary<string, string> RegexReduce;

        public static void InfectPlayer(ExtraPlayerData Infected, ExtraPlayerData Alive)
        {
            Infected.AmountInfected++;
            InfectPlayer(Infected.Player, Alive);
        }

        public static void InfectPlayer(Player Infected, ExtraPlayerData Alive)
        {
            InfectPlayer(Alive);
            try
            {
                if (!ZombiePlugin.CureOn)
                    ZombieHelper.DisplayInfectMessage(Alive.Player, Infected);
                else
                    ZombieHelper.DisplayDisinfectMessage(Alive.Player, Infected);
            }
            catch { }
        }

        public static ExtraPlayerData InfectPlayer(ExtraPlayerData Alive)
        {
            if (!ZombiePlugin.CureOn && !Alive.Infected)
            {
                if (Alive.Survivor)
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + Alive.Player.Username + " failed the Survivor challenge!");
                    int AmountTaken = 0;
                    try
                    {
                        AmountTaken = (int)Math.Round(Alive.Player.Money / (Random.NextDouble() * 100));
                    }
                    catch { AmountTaken = Alive.Player.Money / 2; }//Divide by zero can occur here
                    TakeMoney(Alive.Player, AmountTaken); //Exponential function, 1 lets them have nearly all their money, 0.01 removes all their money
                    //Is also less tough on players with lower money, while more tougher on players with moar money
                }
                Alive.Infected = true;
                Alive.Player.IsHeadFlipped = true;
                Alive.Player.DisplayName = Colors.red + "Undeaad";
                return Alive;
            }
            else if (ZombiePlugin.CureOn && Alive.Infected)
            {
                DisinfectPlayer(Alive);
            }
            return null;
        }

        public static ExtraPlayerData DisinfectPlayer(ExtraPlayerData Alive)
        {
            if (Alive.Infected)
            {
                Alive.Infected = false;
                Alive.Player.IsHeadFlipped = false;
                Alive.Player.DisplayName = Alive.Player.Username;
            }
            return Alive;
        }

        public static void InfectRandomPlayer(List<ExtraPlayerData> ExtraPlayerData)
        {
            InfectPlayer(ExtraPlayerData[Random.Next(0, ExtraPlayerData.Count())]);
        }

        public static ExtraPlayerData InfectRandomPlayerReturn(List<ExtraPlayerData> ExtraPlayerData)
        {
            return InfectPlayer(ExtraPlayerData[Random.Next(0, ExtraPlayerData.Count())]);
        }

        public static void DisinfectRandomPlayer(List<ExtraPlayerData> ExtraPlayerData)
        {
            
            ExtraPlayerData e = ExtraPlayerData[Random.Next(0, ExtraPlayerData.Count())];
            if (e.Infected)
            {
                DisinfectPlayer(e);
            }
        }

        public static void DisplayInfectMessage(Player Alive, Player Infected)
        {
            if (InfectMessages.Count < 2)
            {
                if (Infected == null)
                {
                    Player.UniversalChat(Alive.Username + " was infected!");
                }
                else
                {
                    Player.UniversalChat(Alive.Username + " was infected by " + Infected.Username);
                }
            }
            else
            {
                if (Infected == null)
                {
                    List<string> NoInfectedList = new List<string>();
                    foreach (string s in InfectMessages)
                    {
                        if (s.Contains('*') && !s.Contains('*'))
                        {
                            NoInfectedList.Add(s);
                        }
                    }
                    if (NoInfectedList.Count < 2)
                    {
                        Player.UniversalChat(Alive.Username + " was infected!");
                    }
                    else
                    {
                        Player.UniversalChat(NoInfectedList[Random.Next(0, NoInfectedList.Count - 1)].Replace("*", Alive.Username));
                    }
                }
                else
                {
                    if (Infected.ExtraData.ContainsKey("InfectMessage"))
                    {
                        string s = (string)(Infected.ExtraData["InfectMessage"]);
                        Player.UniversalChat(s.Replace("*", Alive.Username).Replace("!", Infected.Username));
                    }
                    else
                    {
                        Player.UniversalChat(InfectMessages[Random.Next(0, InfectMessages.Count - 1)].Replace("*", Alive.Username).Replace("!", Infected.Username));
                    }
                }
            }
        }

        public static void DisplayDisinfectMessage(Player Alive, Player Infected)
        {
            if (DisinfectMessages.Count < 2)
            {
                if (Infected == null)
                {
                    Player.UniversalChat(Alive.Username + " was disinfected!");
                }
                else
                {
                    Player.UniversalChat(Alive.Username + " was disinfected by " + Infected.Username);
                }
            }
            else
            {
                if (Infected == null)
                {
                    List<string> NoInfectedList = new List<string>();
                    foreach (string s in DisinfectMessages)
                    {
                        if (s.Contains('*') && !s.Contains('*'))
                        {
                            NoInfectedList.Add(s);
                        }
                    }
                    if (NoInfectedList.Count < 2)
                    {
                        Player.UniversalChat(Alive.Username + " was disinfected!");
                    }
                    else
                    {
                        Player.UniversalChat(NoInfectedList[Random.Next(0, NoInfectedList.Count - 1)].Replace("*", Alive.Username));
                    }
                }
                else
                {
                    if (Infected.ExtraData.ContainsKey("DisinfectMessage"))
                    {
                        string s = (string)(Infected.ExtraData["DisinfectMessage"]);
                        Player.UniversalChat(s.Replace("*", Alive.Username).Replace("!", Infected.Username));
                    }
                    else
                    {
                        Player.UniversalChat(DisinfectMessages[Random.Next(0, DisinfectMessages.Count - 1)].Replace("*", Alive.Username).Replace("!", Infected.Username));
                    }
                }
            }
        }

        public static void ToggleCure()
        {
            ZombiePlugin.CureOn = !ZombiePlugin.CureOn;
            if (ZombiePlugin.CureOn)
                Player.UniversalChat("[Zombie Survival]: " + Colors.red + "A cure has been found for zombies! Humans can run into other zombies to un-infect them.");
            else
                Player.UniversalChat("[Zombie Survival]: " + Colors.red + "The virus has mutated... the cure doesn't work anymore :(");
        }

        public static void RainBots(Level Level)
        {
            for (int i = 0; i < Random.Next(5,9); i++)
            {
                short x = (short)Random.Next(32, Level.Size.x * 32);
                short y = (short)((Level.Size.y - 4) * 32);
                short z = (short)Random.Next(32, Level.Size.z * 32);
                Bot ZombieBot = new Bot("UndeaadBot" + i, new Vector3S(x, z, y), new byte[] { 0, 0 }, Level, false, false, false);
                ZombieBot.Player.DisplayName = "";
                ZombieBot.Player.IsHeadFlipped = true;
                ZombieBot.FollowPlayers = true;
                ZombieBot.BreakBlocks = true;
                ZombieBot.Jumping = true;
            }
        }

        public static string FindRandomLevel(string[] IgnoreLevel)
        {
            try
            {
                ArrayList al = new ArrayList();
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.lvl");
                foreach (FileInfo fil in fi)
                {
                    if (!IgnoreLevel.Contains(fil.Name.Split('.')[0].ToLower()))
                    {
                        if (ZombiePlugin.Gamemode == 0)
                        {
                            if (!NoBotLevels.Contains(fil.Name.Split('.')[0].ToLower()))
                            {
                                al.Add(fil.Name.Split('.')[0]);
                            }
                        }
                        else
                        {
                            al.Add(fil.Name.Split('.')[0]);
                        }
                    }
                }
                if (al.Count < 3)
                {
                    return null;
                }
                Random r = new Random();
                int x = r.Next(0, al.Count);
                return al[x].ToString();
            }
            catch { }
            return null;
        }

        public static void ResetPlayers(List<ExtraPlayerData> ExtraPlayerData)
        {
            foreach (ExtraPlayerData p in ExtraPlayerData)
            {
                ResetPlayer(p);
            }
        }

        public static void ResetPlayer(ExtraPlayerData p)
        {
            p.Player.ExtraData.CreateIfNotExist("Voted", false);
            p.Player.ExtraData["Voted"] = false;
            p.Player.IsHeadFlipped = false;
            p.Infected = false;
            p.AmountOfBlocksLeft = Random.Next(40, 80);
            p.Player.DisplayName = p.Player.Color + p.Player.Username;
            p.Survivor = false;
            p.AmountInfected = 0;
            p.AmountMoved = 0;
        }

        public static int GenerateGamemode()
        {
            if (Random.NextDouble() < 0.12)
                return 2;
            else if (Random.NextDouble() < 0.24)
                return 3;
            else if (Random.NextDouble() < 0.39)
                return 1;
            else
                return 0;
        }

        public static string GetGamemode(int s)
        {
            if (s == 1)
                return "Classic";
            else if (s == 2)
                return "Classic Happy Fun Mode";
            else if (s == 3)
                return "Cure";
            else if (s == 4)
                return "COD Zombies";
            else
                return "New";
        }

        public static void TakeMoney(Player p, int Amount)
        {
            p.Money -= Amount;
            p.SendMessage("[Zombie Survival]: " + Colors.red + "You lost " + Amount + " " + Server.Moneys + "!");
            p.Save();
        }

        public static void GiveMoney(Player p, int Amount)
        {
            p.Money += Amount;
            p.SendMessage("[Zombie Survival]: " + Colors.lime + "You gained " + Amount + " " + Server.Moneys + "!");
            p.Save();
        }

        public static void SendUserType(ExtraPlayerData p)
        {
            /*if( p == null ) return;

            Packet packet = new Packet();
            if (p.Referee)
                packet.bytes = new byte[2] { 15, 100 };
            else
                packet.bytes = new byte[2] { 15, 0 };
            p.Player.SendPacket(packet);*/
        }

        public static void ClearDatabaseBlocks()
        {
            using (var data = Database.fillData("DELETE FROM Blocks"))
            {
            }
        }

        public static string CleanString(Player Player, string Message)
        {
            ExtraPlayerData p = ZombiePlugin.FindPlayer(Player);
            string[] message = Message.Split(' ');  

            #region Caps Filter
            string[] noCapsMessage = new string[message.Length];
            int e = 0;
            bool hitCaps = false;
            foreach (string s in message)
            {
                int count = 0;
                foreach (char c in s)
                {
                    if (Char.IsUpper(c))
                        count++;
                }

                if (count >= (s.Length / 2) && s.Length > 3)
                {
                    noCapsMessage[e] = s.ToLower();
                    if (!hitCaps)
                    {
                        Player.SendMessage("Hey! Stop using caps lock!");
                        hitCaps = true;
                    }
                }
                else
                {
                    noCapsMessage[e] = s;
                }
                e++;
            }
            message = noCapsMessage;
            #endregion

            #region Spam Filter
            double seconds = (DateTime.Now - p.SpamTime).TotalSeconds;
            if (seconds < 5)
            {
                if (p.Spam > 7)
                {
                    p.Player.SendMessage("Please don't spam! You have been muted for 20 seconds...");
                    return "";
                }
                else
                    p.Spam += 1;
            }
            else
            {
                if (p.Spam > 7)
                {
                    p.Player.SendMessage("Please don't spam again :|");
                    p.Spam = 0;
                }
                else if (p.Spam < 12)
                    p.Spam -= 1;
            }
            #endregion

            #region Swear Filter
            string[] noSwearMessage = new string[message.Length];
            int z = 0;
            bool noSwear = false;
            foreach (string s in message)
            {
                string tempS = s;
                tempS = s.ToLower();
                foreach (var pattern in RegexReduce)
                {
                    tempS = Regex.Replace(tempS, pattern.Value, pattern.Key/*, RegexOptions.IgnoreCase*/);
                }
                if (SwearFilter.Contains(tempS))
                {
                    noSwearMessage[z] = GetRandomString().Substring(0, tempS.Length);
                    if (!noSwear)
                    {
                        Player.SendMessage("Hey! Stop swearing!");
                        noSwear = true;
                    }
                }
                else
                {
                    noSwearMessage[z] = s;
                }
                z++;
            }
            message = noSwearMessage;
            #endregion

            p.SpamTime = DateTime.Now;

            return String.Join(" ", message);
        }

        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        public static bool CompareImages(Bitmap Bitmap1, Bitmap Bitmap2)
        {
            string Bitmap1Reference, Bitmap2Reference;
            int AmountSame = 0;
            int AmountDifferent = 0;
            try
            {
                if (Bitmap1.Width == Bitmap2.Width && Bitmap1.Height == Bitmap2.Height)
                {
                    for (int i = 0; i < Bitmap1.Width; i++)
                    {
                        for (int j = 0; j < Bitmap1.Height; j++)
                        {
                            Bitmap1Reference = Bitmap1.GetPixel(i, j).ToString();
                            Bitmap2Reference = Bitmap2.GetPixel(i, j).ToString();
                            if (Bitmap1Reference != Bitmap2Reference)
                            {
                                AmountDifferent++;
                                break;
                            }
                            AmountSame++;
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Log(e.Message); Logger.Log(e.StackTrace); }
            try
            {
                double percent = (double)(AmountDifferent * 100) / AmountSame;
                if (percent < 7)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        public static Bitmap GetBitmap(string filename)
        {
            Bitmap retBitmap = null;
            if (File.Exists(filename))
            {
                try
                {
                    retBitmap = new Bitmap(filename, true);
                }
                catch { }
            }
            return retBitmap;
        }
    }
}