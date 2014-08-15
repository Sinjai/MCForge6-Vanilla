using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MCForge.Entity;
using MCForge.Utils;

namespace CTF
{
    public class EXPDB
    {
        public static bool Load(Player p)
        {
            if (File.Exists("players/" + p.Username + "DB.txt"))
            {
                foreach (string line in File.ReadAllLines("players/" + p.Username + "DB.txt"))
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
                                    p.ExtraData["points"] = int.Parse(value);
                                    section = key;
                                    break;
                            }

                            EXPLevel currLevel = null;
                            foreach (EXPLevel lvl in EXPLevel.levels)
                            {
                                if (lvl.requiredEXP <= (int)p.ExtraData["points"])
                                {
                                    currLevel = lvl;
                                }
                            }

                            if (currLevel != null)
                            {
                                p.ExtraData["explevel"] = currLevel;
                            }
                            else
                            {
                                p.ExtraData["explevel"] = EXPLevel.levels[0];
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log("Loading " + p.Username + "'s EXP database failed at section: " + section);
                            Logger.LogError(e);
                        }
                    }
                }

                return true;
            }
            else
            {
                p.ExtraData.ChangeOrCreate("points", 0);
                p.ExtraData.ChangeOrCreate("explevel", 0);
                Save(p);
                return false;
            }
        }

        public static void Save(Player p)
        {
            StreamWriter sw = new StreamWriter(File.Create("players/" + p.Username + "DB.txt"));
            sw.WriteLine("Points = " + p.ExtraData["points"]);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
