using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MCForge.Utils;

namespace CTF
{
    public class EXPDB
    {
        public static bool Load(ExtraPlayerData p)
        {
            if (File.Exists("players/" + p.player.Username + "DB.txt"))
            {
                foreach (string line in File.ReadAllLines("players/" + p.player.Username + "DB.txt"))
                {
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                    {
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();
                        string section = "nowhere yet...";

                        try
                        {
                            switch (key.ToLower())
                            {
                                case "points":
                                    p.points = int.Parse(value);
                                    section = key;
                                    break;
                            }

                            EXPLevel currLevel = null;
                            foreach (EXPLevel lvl in EXPLevel.levels)
                            {
                                if (lvl.requiredEXP <= p.points)
                                {
                                    currLevel = lvl;
                                }
                            }

                            if (currLevel != null)
                            {
                                p.explevel = currLevel;
                            }
                            else
                            {
                                p.explevel = EXPLevel.levels[0];
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log("Loading " + p.player.Username + "'s EXP database failed at section: " + section);
                            Logger.LogError(e);
                        }
                    }
                }

                return true;
            }
            else
            {
                p.points = 0;
                p.explevel = EXPLevel.levels[0];
                Save(p);
                return false;
            }
        }

        public static void Save(ExtraPlayerData p)
        {
            StreamWriter sw = new StreamWriter(File.Create("players/" + p.player.Username + "DB.txt"));
            sw.WriteLine("Points = " + p.points);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
