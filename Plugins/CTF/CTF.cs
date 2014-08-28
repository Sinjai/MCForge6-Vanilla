/*
	Copyright 2011 MCForge Team - 
    Created by Snowl (David D.)

	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Threading;

using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Interface.Plugin;
using MCForge.Utils;
using MCForge.World;

namespace CTF
{
    public class CTFPlugin : IPlugin
    {
        public string Name { get { return "Capture the Flag"; } }
        public string Author { get { return "Hetal"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return "com.mcforge.ctf"; } }

        public void OnLoad(string[] args)
        {
            //Plugin.AddReference(this);
            //StartGame(1, 0);
        }

        public void OnUnload()
        {
            ServerCTF.gameStatus = 0; ServerCTF.ctfGameStatus = 0; limitRounds = 0; initialChangeLevel = false; ServerCTF.CTFModeOn = false; ServerCTF.ctfRound = false;
        }

        public CTFPlugin() { }
        public int amountOfRounds = 0;
        public int limitRounds = 0;
        public int aliveCount = 0;
        public int amountOfMilliseconds = 0;
        public DateTime CTFTime = new DateTime();
        public static System.Timers.Timer timer;
        public static System.Timers.Timer timer2;
        public bool initialChangeLevel = false;
        public int blueTeamCaptures = 0;
        public int redTeamCaptures = 0;
        public string currentLevelName = "";
        public List<Player> red = new List<Player>();
        public List<Player> blu = new List<Player>();
        public Vector3S blueSpawn;
        public Vector3S redSpawn;
        public Vector3S blueFlag;
        public Vector3S redFlag;
        public Vector3S blueDroppedFlag;
        public Vector3S redDroppedFlag;
        public int halfway;
        public CatchPos blueFlagblock;
        public CatchPos redFlagblock;
        public int buildCeiling = 180;
        public bool firstblood = false;
        public int bluDeaths = 0;
        public int redDeaths = 0;
        public static System.Timers.Timer checkTimer = new System.Timers.Timer(5000);

        public struct CatchPos { public ushort x, y, z; public ushort type; }

        public void StartGame(int status, int amount)
        {
            //status: 0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop round next round

            if (status == 0) return;

            //SET ALL THE VARIABLES!
            ServerCTF.CTFModeOn = true;
            ServerCTF.ctfGameStatus = status;
            ServerCTF.ctfRound = false;
            initialChangeLevel = false;
            limitRounds = amount + 1;
            amountOfRounds = 0;
            //SET ALL THE VARIABLES?!?

            //Start the main CTF thread
            Thread t = new Thread(MainLoop);
            t.Start();
            checkTimer.Elapsed += delegate { CheckXP(); };
            checkTimer.Start();
        }

        private void MainLoop()
        {
            bool tried = false;
            if (ServerCTF.ctfGameStatus == 0) return;
            bool cutVariable = true;

            if (initialChangeLevel == false)
            {
                ChangeLevel();
                initialChangeLevel = true;
            }

            while (cutVariable == true)
            {
                int gameStatus = ServerCTF.ctfGameStatus;
                ServerCTF.ctfRound = false;
                amountOfRounds = amountOfRounds + 1;

                if (gameStatus == 0) { cutVariable = false; return; }
                else if (gameStatus == 1) { if (tried == false) { tried = true; ChangeLevel(); MainGame(); } }
                else if (gameStatus == 2) { MainGame(); ChangeLevel(); cutVariable = false; ServerCTF.ctfGameStatus = 0; return; }
                else if (gameStatus == 3)
                {
                    if (limitRounds == amountOfRounds) { cutVariable = false; ServerCTF.ctfGameStatus = 0; limitRounds = 0; initialChangeLevel = false; ServerCTF.CTFModeOn = false; ServerCTF.ctfRound = false; return; }
                    else { MainGame(); ChangeLevel(); }
                }
                else if (gameStatus == 4)
                { cutVariable = false; ServerCTF.gameStatus = 0; ServerCTF.ctfGameStatus = 0; limitRounds = 0; initialChangeLevel = false; ServerCTF.CTFModeOn = false; ServerCTF.ctfRound = false; return; }
            }
        }

        private void MainGame()
        {
        unload_loop:
            try
            {
                if (Level.Levels != null)
                {
                    foreach (Level l in Level.Levels)
                    {
                        if (l.Name.ToLower() != Server.Mainlevel.Name && l.Name.ToLower() != currentLevelName.ToLower() && l.Name.ToLower() != "tutorial")
                        {
                            l.Unload();
                            goto unload_loop;
                        }
                    }
                }
            }
            catch { goto unload_loop; }
            if (ServerCTF.ctfGameStatus == 0) return;
            if (!ServerCTF.CTFModeOn) { return; }
            Server.Players.ForEach(delegate(Player player1)
            {
                //RESET ALL THE VARIABLES!
                player1.ExtraData.ChangeOrCreate("team", 0);
                player1.ExtraData.ChangeOrCreate("isholdingflag", false);
                player1.ExtraData.ChangeOrCreate("overallkilled", 0);
                player1.ExtraData.ChangeOrCreate("overalldied", 0);
                player1.ExtraData.ChangeOrCreate("killingpeople", false);
                player1.ExtraData.ChangeOrCreate("killingpeople", false);
                player1.ExtraData.ChangeOrCreate("amountkilled", 0);
                player1.ExtraData.ChangeOrCreate("mineplacementx", 0);
                player1.ExtraData.ChangeOrCreate("mineplacementz", 0);
                player1.ExtraData.ChangeOrCreate("mineplacementy", 0);
                player1.ExtraData.ChangeOrCreate("minesplaced", 0);
                player1.ExtraData.ChangeOrCreate("trapplacementx", 0);
                player1.ExtraData.ChangeOrCreate("trapplacementz", 0);
                player1.ExtraData.ChangeOrCreate("trapplacementy", 0);
                player1.ExtraData.ChangeOrCreate("trapsplaced", 0);
                player1.ExtraData.ChangeOrCreate("dealthtimeron", false);
                player1.ExtraData.ChangeOrCreate("hasbeentrapped", false);
                player1.ExtraData.ChangeOrCreate("tntplacementx", 0);
                player1.ExtraData.ChangeOrCreate("tntplacementz", 0);
                player1.ExtraData.ChangeOrCreate("tntplacementy", 0);
                player1.ExtraData.ChangeOrCreate("tntplaced", 0);
                ServerCTF.killed.Clear();
                ServerCTF.blueFlagDropped = false;
                ServerCTF.redFlagDropped = false;
                ServerCTF.blueFlagHeld = false;
                ServerCTF.redFlagHeld = false;
                firstblood = false;
                redDeaths = 0;
                bluDeaths = 0;
                red.Clear();
                blu.Clear();
            });
            Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "30 seconds till the round starts!" + Colors.gray + " - ");
            Thread.Sleep(1000 * 30);
            CTFGame();
        }

        private void CTFGame()
        {
            Level level = Level.FindLevel(currentLevelName);
            if (level == null) return;
            Logger.Log(redFlag.x.ToString());
            level.BlockChange((ushort)redFlag.x, (ushort)redFlag.z, (ushort)redFlag.y, Block.BlockList.redflag);
            level.BlockChange((ushort)blueFlag.x, (ushort)blueFlag.z, (ushort)blueFlag.y, Block.BlockList.blueflag);
            ServerCTF.ctfRound = true;
            Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The round has started!" + Colors.gray + " - ");
            Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The round has started!" + Colors.gray + " - ");
            Random random = new Random();
            int amountOfMinutes = random.Next(25, 40);
            Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The round will last for " + amountOfMinutes + " minutes!" + Colors.gray + " - ");
            Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The round will last for " + amountOfMinutes + " minutes!" + Colors.gray + " - ");
            amountOfMilliseconds = (60000 * amountOfMinutes);

            timer = new System.Timers.Timer(amountOfMilliseconds);
            timer.Elapsed += new ElapsedEventHandler(EndRound);
            timer.Enabled = true;
            CTFTime = DateTime.Now + new TimeSpan(0, amountOfMinutes, 0);
            ServerCTF.vulnerable = 1;

            while (ServerCTF.ctfRound)
            {
                red.ForEach(delegate(Player player1)
                {
                    blu.ForEach(delegate(Player player2)
                    {
                        if (player2.Pos.x / 32 == player1.Pos.x / 32 || player2.Pos.x / 32 == player1.Pos.x / 32 + 1 || player2.Pos.x / 32 == player1.Pos.x / 32 - 1)
                        {
                            if (player2.Pos.z / 32 == player1.Pos.z / 32 || player2.Pos.z / 32 == player1.Pos.z / 32 - 1 || player2.Pos.z / 32 == player1.Pos.z / 32 + 1)
                            {
                                if (player2.Pos.y / 32 == player1.Pos.y / 32 || player2.Pos.y / 32 == player1.Pos.y / 32 + 1 || player2.Pos.y / 32 == player1.Pos.y / 32 - 1)
                                {
                                    if (player1 != player2 && player1.Level.Name == currentLevelName && player2.Level.Name == currentLevelName)
                                    {
                                        bool one = false;
                                        bool two = false;
                                       /* if (player1.makeaura || player2.makeaura)
                                        {
                                            if (player2.Pos.x / 32 == player1.Pos.x / 32 || player2.Pos.x / 32 == player1.Pos.x / 32 + 1 || player2.Pos.x / 32 == player1.Pos.x / 32 - 1)
                                                if (player2.Pos.z / 32 == player1.Pos.z / 32 || player2.Pos.z / 32 == player1.Pos.z / 32 - 1 || player2.Pos.z / 32 == player1.Pos.z / 32 + 1)
                                                    if (player2.Pos.y / 32 == player1.Pos.y / 32 || player2.Pos.y / 32 == player1.Pos.y / 32 + 1 || player2.Pos.y / 32 == player1.Pos.y / 32 - 1)
                                                        if (player1 != player2 && player1.Level.Name == currentLevelName && player2.Level.Name == currentLevelName)
                                                        {
                                                            one = OnSide(player1, getTeam(player1));
                                                            two = OnSide(player2, getTeam(player2));
                                                            if (one == two) { return; }
                                                            if (one && !(bool)player2.ExtraData["deathtimeron"])
                                                            {
                                                                killedPlayer(player1, (ushort)(player1.Pos.x / 32), (ushort)(player1.Pos.z / 32), (ushort)(player1.Pos.y / 32), false, "tag");
                                                            }
                                                            else if (two && !(bool)player1.ExtraData["deathtimeron"])
                                                            {
                                                                killedPlayer(player2, (ushort)(player2.Pos.x / 32), (ushort)(player2.Pos.z / 32), (ushort)(player2.Pos.y / 32), false, "tag");
                                                            }
                                                            return;
                                                        }
                                        } */
                                        one = OnSide(player1, getTeam(player1));
                                        two = OnSide(player2, getTeam(player2));
                                        if (one == two) { return; }
                                        if (one && !(bool)player2.ExtraData["deathtimeron"])
                                        {
                                            killedPlayer(player1, (ushort)(player1.Pos.x / 32), (ushort)(player1.Pos.z / 32), (ushort)(player1.Pos.y / 32), false, "tag");
                                        }
                                        else if (two && !(bool)player1.ExtraData["deathtimeron"])
                                        {
                                            killedPlayer(player2, (ushort)(player2.Pos.x / 32), (ushort)(player2.Pos.z / 32), (ushort)(player2.Pos.y / 32), false, "tag");
                                        }
                                    }
                                }
                            }
                        }
                    });
                });
                Server.Players.ForEach(delegate(Player player1)
                {
                    if ((bool)player1.ExtraData["isholdingflag"] && (int)player1.ExtraData["team"] != 0)
                    {
                        if (getTeam(player1) == "blue")
                        {
                            tempFlagBlock(player1, "blue");
                        }
                        else if (getTeam(player1) == "red")
                        {
                            tempFlagBlock(player1, "red");
                        }
                    }
                });
                Thread.Sleep(500);
            }

            HandOutRewards();
        }

        public void EndRound(object sender, ElapsedEventArgs e)
        {
            if (ServerCTF.ctfGameStatus == 0) return;
            Player.UniversalChat("%4Round End:%f 5"); Thread.Sleep(1000);
            Player.UniversalChat("%4Round End:%f 4"); Thread.Sleep(1000);
            Player.UniversalChat("%4Round End:%f 3"); Thread.Sleep(1000);
            Player.UniversalChat("%4Round End:%f 2"); Thread.Sleep(1000);
            Player.UniversalChat("%4Round End:%f 1"); Thread.Sleep(1000);
            HandOutRewards();
        }

        public void HandOutRewards()
        {
            if (!ServerCTF.ctfRound) goto lol;
            ServerCTF.ctfRound = false;
            if (blueTeamCaptures > 4) { Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "Congratulations to the blue team for winning with " + blueTeamCaptures + " captures!" + Colors.gray + " - "); Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "Congratulations to the blue team for winning with " + blueTeamCaptures + " captures!" + Colors.gray + " - "); }
            else if (redTeamCaptures > 4) { Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "Congratulations to the red team for winning with " + redTeamCaptures + " captures!" + Colors.gray + " - "); Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "Congratulations to the red team for winning with " + redTeamCaptures + " captures!" + Colors.gray + " - "); }
            else Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "No team managed to capture 5 flags! Good luck next time!" + Colors.gray + " - ");
            Server.IRC.SendMessage(Colors.gray + " - " + Colors.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + Colors.red + " Red: " + Server.DefaultColor + redTeamCaptures + Colors.gray + " - ");
            Server.IRC.SendMessage(Colors.gray + " - " + Colors.blue + "Blue kills: " + Server.DefaultColor + bluDeaths + Colors.red + " Red kills: " + Server.DefaultColor + redDeaths + Colors.gray + " - ");
            Player.UniversalChat(Colors.gray + " - " + Colors.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + Colors.red + " Red: " + Server.DefaultColor + redTeamCaptures + Colors.gray + " - ");
            Player.UniversalChat(Colors.gray + " - " + Colors.blue + "Blue kills: " + Server.DefaultColor + bluDeaths + Colors.red + " Red kills: " + Server.DefaultColor + redDeaths + Colors.gray + " - ");
            var lengths = from element in Server.Players
                          orderby ((int)element.ExtraData["overalldied"] - (int)element.ExtraData["overallkilled"])
                          select element;

            int loop = 0;

            foreach (Player z in lengths)
            {
                loop++;
                if (loop >= 6)
                    break;
                else if (loop == 1)
                {
                    Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "Most Valued Player: " + z.Username + " - " + z.ExtraData["overallkilled"] + ":" + z.ExtraData["overalldied"] + Colors.gray + " - ");
                    Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "Most Valued Player: " + z.Username + " - " + z.ExtraData["overallkilled"] + ":" + z.ExtraData["overalldied"] + Colors.gray + " - ");
                }
                else
                {
                    Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "#" + loop + ": " + z.Username + " - " + z.ExtraData["overallkilled"] + ":" + z.ExtraData["overalldied"] + Colors.gray + " - ");
                    Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "#" + loop + ": " + z.Username + " - " + z.ExtraData["overallkilled"] + ":" + z.ExtraData["overalldied"] + Colors.gray + " - ");
                }
            }
        lol:
            removeTempFlagBlocks();
            blueTeamCaptures = 0;
            redTeamCaptures = 0;
            Server.Players.ForEach(delegate(Player player)
            {
                if (player.Level.Name == currentLevelName)
                {
                    player.ExtraData["team"] = 0;
                    player.ExtraData["isholdingflag"] = false;

                    red.Clear();
                    blu.Clear();
                    player.Color = player.Group.Color;
                }
            });
            timer.Enabled = false;
            return;
        }

        //Main Game Finishes Here - support functions after this

        public int CTFStatus()
        {
            return ServerCTF.ctfGameStatus;
        }

        public bool GameInProgess()
        {
            return ServerCTF.ctfRound;
        }

        public void tempFlagBlock(Player player, string Color)
        {
            Level levell = Level.FindLevel(currentLevelName);
            if (levell == null) return;
            ushort x; ushort y; ushort z;
            if (getTeam(player) == "blue" && Color == "blue")
            {
                //DRAW ON PLAYER HEAD
                x = (ushort)(player.Pos.x / 32);
                y = (ushort)(player.Pos.z / 32 + 4);
                z = (ushort)(player.Pos.y / 32);

                if (blueFlagblock.x == x && blueFlagblock.y == y && blueFlagblock.z == z) { return; }
                int loop = 0;
                while (loop < ServerCTF.vulnerable)
                {
                    levell.Blockchange(null,blueFlagblock.x, (ushort)((blueFlagblock.y + loop)), blueFlagblock.z, Block.BlockList.AIR);
                    loop++;
                }
                loop = 0;
                while (loop != ServerCTF.vulnerable)
                {

                    blueFlagblock.type = levell.GetBlock(x, y, z);

                    if (levell.GetBlock(x, (ushort)(y + loop), z) == Block.BlockList.AIR)
                        levell.Blockchange(null,x, (ushort)(y + loop), z, Block.BlockList.RED_CLOTH);

                    loop++;
                }

                blueFlagblock.x = x;
                blueFlagblock.y = y;
                blueFlagblock.z = z;
            }
            else if (getTeam(player) == "red" && Color == "red")
            {
                //DRAW ON PLAYER HEAD
                x = (ushort)(player.Pos.x / 32);
                y = (ushort)(player.Pos.z / 32 + 4);
                z = (ushort)(player.Pos.y / 32);

                if (redFlagblock.x == x && redFlagblock.y == y && redFlagblock.z == z) { return; }

                int loop = 0;
                while (loop < ServerCTF.vulnerable)
                {
                    levell.Blockchange(null,redFlagblock.x, (ushort)((redFlagblock.y + loop)), redFlagblock.z, Block.BlockList.AIR);
                    loop++;
                }
                loop = 0;
                while (loop != ServerCTF.vulnerable)
                {
                    redFlagblock.type = levell.GetBlock(x, y, z);

                    if (levell.GetBlock(x, (ushort)(y + loop), z) == Block.BlockList.AIR)
                        levell.Blockchange(null,x, (ushort)(y + loop), z, Block.BlockList.BLUE_CLOTH);

                    loop++;
                }

                redFlagblock.x = x;
                redFlagblock.y = y;
                redFlagblock.z = z;
            }
        }

        public void removeTempFlagBlock(string Color)
        {
            Level levell = Level.FindLevel(currentLevelName);
            if (levell == null) return;
            if (Color == "blue")
            {
                int loop = 0;
                while (loop < ServerCTF.vulnerable)
                {
                    levell.Blockchange(null,blueFlagblock.x, (ushort)((blueFlagblock.y + loop)), blueFlagblock.z, Block.BlockList.AIR);
                    loop++;
                }
            }
            else if (Color == "red")
            {
                int loop = 0;
                while (loop < ServerCTF.vulnerable)
                {
                    levell.Blockchange(null,redFlagblock.x, (ushort)((redFlagblock.y + loop)), redFlagblock.z, Block.BlockList.AIR);
                    loop++;
                }
            }
        }

        public void removeTempFlagBlocks()
        {
            Level levell = Level.FindLevel(currentLevelName);
            if (levell == null) return;
            int loop2 = 0;
            while (loop2 > ServerCTF.vulnerable)
            {
                levell.Blockchange(null,redFlagblock.x, (ushort)((redFlagblock.y + loop2)), redFlagblock.z, Block.BlockList.AIR);
                loop2++;
            }
            int loop = 0;
            while (loop != ServerCTF.vulnerable)
            {
                levell.Blockchange(null,blueFlagblock.x, (ushort)((blueFlagblock.y + loop)), blueFlagblock.z, Block.BlockList.AIR);
                loop++;
            }
        }

        public void ChangeLevel(string LevelName, bool changeMainLevel, bool announce)
        {
            String next = LevelName;
            ServerCTF.nextLevel = "";
            Command.All["load"].Use(null, next.ToLower().Select(c => c.ToString()).ToArray());
            if (announce) { Player.UniversalChat("The next map has been chosen - " + Colors.red + next.ToLower()); Server.IRC.SendMessage("The next map has been chosen - " + Colors.red + next.ToLower()); }
            ServerCTF.currentLevel = next;
            try
            {
                string foundLocation;
                foundLocation = "levels/ctf/" + next + ".properties";
                if (!File.Exists(foundLocation))
                {
                    foundLocation = "levels/ctf/" + next;
                }
                foreach (string line in File.ReadAllLines(foundLocation))
                {
                    if (line[0] != '#')
                    {
                        string value = line.Substring(line.IndexOf(" = ") + 3);

                        switch (line.Substring(0, line.IndexOf(" = ")).ToLower())
                        {
                            case "divider": halfway = Convert.ToInt32(value); break;
                            case "buildceiling": buildCeiling = Convert.ToInt32(value); break;
                            case "redspawnx": redSpawn.x = (short)Convert.ToInt32(value); break;
                            case "redspawnz": redSpawn.z = (short)Convert.ToInt32(value); break;
                            case "redspawny": redSpawn.y = (short)Convert.ToInt32(value); break;
                            case "bluespawnx": blueSpawn.x = (short)Convert.ToInt32(value); break;
                            case "bluespawnz": blueSpawn.z = (short)Convert.ToInt32(value); break;
                            case "bluespawny": blueSpawn.y = (short)Convert.ToInt32(value); break;
                            case "redflagx": redFlag.x = (short)Convert.ToInt32(value); break;
                            case "redflagz": redFlag.z = (short)Convert.ToInt32(value); break;
                            case "redflagy": redFlag.y = (short)Convert.ToInt32(value); break;
                            case "blueflagx": blueFlag.x = (short)Convert.ToInt32(value); break;
                            case "blueflagz": blueFlag.z = (short)Convert.ToInt32(value); break;
                            case "blueflagy": blueFlag.y = (short)Convert.ToInt32(value); break;
                        }
                    }
                }
            }
            catch { }
            Server.Players.ForEach(delegate(Player player)
            {
                if (player.Level.Name != next && player.Level.Name == currentLevelName)
                {
                    Command.All["goto"].Use(player, Server.Mainlevel.Name.Select(c => c.ToString()).ToArray());
                    while (player.IsLoading) { Thread.Sleep(890); }
                }
            });
            currentLevelName = next;
            return;
        }

        public void ChangeLevel()
        {
            try
            {
                ArrayList al = new ArrayList();
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.cw");
                foreach (FileInfo fil in fi)
                {
                    al.Add(fil.Name.Split('.')[0]);
                }

                if (al.Count <= 2 && !ServerCTF.UseLevelList) { Logger.Log("You must have more than 2 levels to change levels in CTF"); return; }

                if (ServerCTF.LevelList.Count < 2 && ServerCTF.UseLevelList) { Logger.Log("You must have more than 2 levels in your level list to change levels in CTF"); return; }

                string selectedLevel1 = "";
                string selectedLevel2 = "";

            LevelChoice:
                Random r = new Random();
                int x = 0;
                int x2 = 1;
                string level = ""; string level2 = "";
                if (!ServerCTF.UseLevelList)
                {
                    x = r.Next(0, al.Count);
                    x2 = r.Next(0, al.Count);
                    level = al[x].ToString();
                    level2 = al[x2].ToString();
                }
                else
                {
                    x = r.Next(0, ServerCTF.LevelList.Count());
                    x2 = r.Next(0, ServerCTF.LevelList.Count());
                    level = ServerCTF.LevelList[x].ToString();
                    level2 = ServerCTF.LevelList[x2].ToString();
                }
                Level current = Server.Mainlevel;

                if (ServerCTF.lastLevelVote1 == level || ServerCTF.lastLevelVote2 == level2 || ServerCTF.lastLevelVote1 == level2 || ServerCTF.lastLevelVote2 == level || current == Level.FindLevel(level) || currentLevelName == level || current == Level.FindLevel(level2) || currentLevelName == level2 || "main" == level || "main" == level2 || "tutorial" == level || "tutorial" == level2)
                    goto LevelChoice;
                else if (selectedLevel1 == "") { selectedLevel1 = level; goto LevelChoice; }
                else
                    selectedLevel2 = level2;

                ServerCTF.Level1Vote = 0; ServerCTF.Level2Vote = 0; ServerCTF.Level3Vote = 0;
                ServerCTF.lastLevelVote1 = selectedLevel1; ServerCTF.lastLevelVote2 = selectedLevel2;
                if (ServerCTF.ctfGameStatus == 4 || ServerCTF.ctfGameStatus == 0) { return; }

                if (initialChangeLevel == true)
                {
                    ServerCTF.votingforlevel = true;
                    Server.IRC.SendMessage(" " + Colors.black + "Level Vote: " + Server.DefaultColor + selectedLevel1 + ", " + selectedLevel2 + " or random " + "(" + Colors.lime + "1" + Server.DefaultColor + "/" + Colors.red + "2" + Server.DefaultColor + "/" + Colors.blue + "3" + Server.DefaultColor + ")");
                    Player.UniversalChat(" " + Colors.black + "Level Vote: " + Server.DefaultColor + selectedLevel1 + ", " + selectedLevel2 + " or random " + "(" + Colors.lime + "1" + Server.DefaultColor + "/" + Colors.red + "2" + Server.DefaultColor + "/" + Colors.blue + "3" + Server.DefaultColor + ")");
                    System.Threading.Thread.Sleep(15000);
                    ServerCTF.votingforlevel = false;
                }

                else { ServerCTF.Level1Vote = 1; ServerCTF.Level2Vote = 0; ServerCTF.Level3Vote = 0; }


                if (ServerCTF.ctfGameStatus == 4 || ServerCTF.ctfGameStatus == 0) { return; }

                if (ServerCTF.Level1Vote >= ServerCTF.Level2Vote)
                {
                    if (ServerCTF.Level3Vote > ServerCTF.Level1Vote && ServerCTF.Level3Vote > ServerCTF.Level2Vote)
                    {
                        r = new Random();
                        int x3 = r.Next(0, al.Count);
                        ChangeLevel(al[x3].ToString(), ServerCTF.CTFOnlyServer, true);
                    }
                    ChangeLevel(selectedLevel1, ServerCTF.CTFOnlyServer, true);
                }
                else
                {
                    if (ServerCTF.Level3Vote > ServerCTF.Level1Vote && ServerCTF.Level3Vote > ServerCTF.Level2Vote)
                    {
                        r = new Random();
                        int x4 = r.Next(0, al.Count);
                        ChangeLevel(al[x4].ToString(), ServerCTF.CTFOnlyServer, true);
                    }
                    ChangeLevel(selectedLevel2, ServerCTF.CTFOnlyServer, true);
                }
                Server.Players.ForEach(delegate(Player winners)
                {
                    winners.ExtraData.ChangeOrCreate("voted", false);
                });
            }
            catch { }
            return;

        }

        public void ChangeTime(object sender, ElapsedEventArgs e)
        {
            amountOfMilliseconds = amountOfMilliseconds - 10;
        }

        public bool IsInCTFGameLevel(Player p)
        {
            return p.Level.Name == currentLevelName;
        }

        public Player killedPlayer(Player p, ushort x, ushort y, ushort z, bool tnt, string type)
        {
            bool killed = false;
            Player pp = null;
            int Money = 5;
            int points = 15;
            p.ExtraData["killingpeople"] = true;

            if (!GameInProgess()) return null;
            if (getTeam(p) == null) return null;

            foreach (Player ppp in Server.Players)
            {
                bool cutoff = false;
                if (ppp.Pos.x / 32 == x && !cutoff)
                    if ((ppp.Pos.z / 32 == y) || ((ppp.Pos.z / 32) - 1 == y) || ((ppp.Pos.z / 32) + 1 == y))
                        if (ppp.Pos.y / 32 == z)
                        {
                            if (!ServerCTF.killed.Contains(ppp) && !(bool)ppp.ExtraData["deathtimeron"] && !InSpawn(ppp, ppp.Pos) && ppp != p && (getTeam(p) != getTeam(ppp)))
                            {
                                if (!(bool)ppp.ExtraData["isholdingflag"])
                                {
                                    ppp.ExtraData["deathtimeron"] = true;
                                    ppp.deathTimer = new System.Timers.Timer(3500);
                                    ppp.deathTimer.Elapsed += new ElapsedEventHandler(ppp.resetDeathTimer);
                                    ppp.deathTimer.Enabled = true;
                                }
                                else
                                {
                                    if (!firstblood)
                                    {
                                        firstblood = true;
                                        Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Colors.gray + " got " + Colors.red + "\"firsty bloody\"! " + Colors.gray + " - ");
                                        Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Colors.gray + " got " + Colors.red + "\"firsty bloody\"! " + Colors.gray + " - ");
                                        Money += 20;
                                        points += 20;
                                    }
                                    Random r = new Random();
                                    if (r.Next(1, 3) != 2)
                                    {
                                        switch (type)
                                        {
                                            case "pistol":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + "'s amazing pistol shot! - ");
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + "'s amazing pistol shot! - ");
                                                break;
                                            case "lazer":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was lazered by " + p.Color + p.Username + Colors.gray + " - ");
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was lazered by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                            case "mine":
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " didn't look where they were walking - ");
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " didn't look where they were walking - ");
                                                break;
                                            case "lightning":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got struck down by Zeus (aka " + p.Color + p.Username + Colors.gray + ") - ");
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got struck down by Zeus (aka " + p.Color + p.Username + Colors.gray + ") - ");
                                                break;
                                            case "gun":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got shot with a rocket fired by " + p.Color + p.Username + Colors.gray + " - ");
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got shot with a rocket fired by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                            case "tnt":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was exploded into bits by " + p.Color + p.Username + Colors.gray + " - ");
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was exploded into bits by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                            case "tag":
                                                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " tagged " + ppp.Color + ppp.Username + Colors.gray + " - ");
                                                Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " tagged " + ppp.Color + ppp.Username + Colors.gray + " - ");
                                                break;
                                            default:
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + " - ");
                                                Player.UniversalChat(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (type)
                                        {
                                            case "pistol":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + "'s amazing pistol shot! - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + "'s amazing pistol shot! - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + "'s amazing pistol shot! - ");
                                                break;
                                            case "lazer":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was lazered by " + p.Color + p.Username + Colors.gray + " - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was lazered by " + p.Color + p.Username + Colors.gray + " - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was lazered by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                            case "mine":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " didn't look where they were walking - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " didn't look where they were walking - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " didn't look where they were walking - ");
                                                break;
                                            case "lightning":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got struck down by Zeus (aka " + p.Color + p.Username + Colors.gray + ") - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got struck down by Zeus (aka " + p.Color + p.Username + Colors.gray + ") - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got struck down by Zeus (aka " + p.Color + p.Username + Colors.gray + ") - ");
                                                break;
                                            case "gun":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got shot with a rocket fired by " + p.Color + p.Username + Colors.gray + " - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got shot with a rocket fired by " + p.Color + p.Username + Colors.gray + " - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " got shot with a rocket fired by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                            case "tnt":
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was exploded into bits by " + p.Color + p.Username + Colors.gray + " - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was exploded into bits by " + p.Color + p.Username + Colors.gray + " - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was exploded into bits by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                            case "tag":
                                                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " tagged " + ppp.Color + ppp.Username + Colors.gray + " - ");
                                                p.SendMessage( Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " tagged " + ppp.Color + ppp.Username + Colors.gray + " - ");
                                                ppp.SendMessage( Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " tagged " + ppp.Color + ppp.Username + Colors.gray + " - ");
                                                break;
                                            default:
                                                Server.IRC.SendMessage(Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + " - ");
                                                p.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + " - ");
                                                ppp.SendMessage( Colors.gray + " - " + ppp.Color + ppp.Username + Server.DefaultColor + " was killed by " + p.Color + p.Username + Colors.gray + " - ");
                                                break;
                                        }
                                    }
                                    if (getTeam(p) == "red")
                                        redDeaths += 1;
                                    else if (getTeam(p) == "blue")
                                        bluDeaths += 1;
                                    ppp.ExtraData["overalldied"] = (int)ppp.ExtraData["overalldied"] + 1;
                                    ppp.ExtraData["amountkilled"] = 0;
                                    ppp.ExtraData.ChangeOrCreate("deathtimeron", true);
                                    ppp.deathTimer = new System.Timers.Timer(7500);
                                    ppp.deathTimer.Elapsed += new ElapsedEventHandler(ppp.resetDeathTimer);
                                    ppp.deathTimer.Enabled = true;
                                    ppp.ExtraData["hasbeentrapped"] = false;
                                    killed = true;
                                    ServerCTF.killed.Add(ppp);
                                    pp = ppp;
                                    sendToTeamSpawn(ppp);
                                    p.ExtraData["amountkilled"] = (int)p.ExtraData["amountkilled"] + 1;
                                    p.ExtraData["overallkilled"] = (int)p.ExtraData["overallkilled"] + 1;
                                    if ((bool)p.ExtraData["killingpeople"])
                                    {
                                        if ((int)p.ExtraData["amountkilled"] >= 3)
                                        {
                                            if ((int)p.ExtraData["amountkilled"] == 3) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " got a triple kill! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " got a triple kill! " + Colors.gray + " - "); points += 1; Money = Money + 1; }
                                            if ((int)p.ExtraData["amountkilled"] == 4) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " got a quadra kill! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " got a quadra kill! " + Colors.gray + " - "); points += 1; Money = Money + 1; }
                                            if ((int)p.ExtraData["amountkilled"] == 5) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " got a penta kill! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " got a penta kill! " + Colors.gray + " - "); points += 2; Money = Money + 2; }
                                            if ((int)p.ExtraData["amountkilled"] == 6) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is crazy! Sextuple kill!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is crazy! Sextuple kill!" + Colors.gray + " - "); points += 2; Money = Money + 2; }
                                            if ((int)p.ExtraData["amountkilled"] == 7) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is insane! Seputuple kill!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is insane! Seputuple kill!" + Colors.gray + " - "); points += 3; Money = Money + 3; }
                                            if ((int)p.ExtraData["amountkilled"] == 8) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is amazing! Octuple kill!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is amazing! Octuple kill!" + Colors.gray + " - "); points += 3; Money = Money + 3; }
                                            if ((int)p.ExtraData["amountkilled"] == 9) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is bonkers! Nonuple kill!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is bonkers! Nonuple kill!" + Colors.gray + " - "); points += 3; Money = Money + 3; }
                                            if ((int)p.ExtraData["amountkilled"] == 10) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is nutty! Decuple kill!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " is nutty! Decuple kill!" + Colors.gray + " - "); points += 5; Money = Money + 5; }
                                            if ((int)p.ExtraData["amountkilled"] == 11) { Money = Money + 5; }
                                            if ((int)p.ExtraData["amountkilled"] == 12) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " IS LEGENDARY! DUODECUPLE KILL! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " IS LEGENDARY! DUODECUPLE KILL! " + Colors.gray + " - "); points += 7; Money = Money + 7; }
                                            if ((int)p.ExtraData["amountkilled"] == 13) { Money = Money + 8; }
                                            if ((int)p.ExtraData["amountkilled"] == 14) { Money = Money + 9; }
                                            if ((int)p.ExtraData["amountkilled"] == 15) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " IS KILLING EVERYONE! 15-TUPLE KILL! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " IS KILLING EVERYONE! 15-TUPLE KILL! " + Colors.gray + " - "); points += 10; Money = Money + 10; }
                                            if ((int)p.ExtraData["amountkilled"] == 16) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " IS MAGIC! GEN-TUPLE KILL! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " IS MAGIC! GEN-TUPLE KILL! " + Colors.gray + " - "); points += 12; Money = Money + 12; }
                                            if ((int)p.ExtraData["amountkilled"] > 16 && (int)p.ExtraData["amountkilled"] < 100) { Money = Money + 12; }
                                            if ((int)p.ExtraData["amountkilled"] == 100) { Player.UniversalChat(Colors.gray + " - " + p.Color + p.Username + Server.DefaultColor + " OWNS CTF! 100+ KILL STREAK! " + Colors.gray + " - "); Money = Money + 50; }
                                            if ((int)p.ExtraData["amountkilled"] > 100) { Money = Money + 50; points += 50; }
                                            // TODO: Add events to when a player kills a player with a high kill streak
                                        }
                                        addMoney(p, Money, points);
                                    }
                                }
                            }
                            else { }
                        }
            }
            p.ExtraData["killingpeople"] = false;
            ServerCTF.killed.Clear();
            if (killed) return pp;
            else return null;
        }

        public string getTeam(Player p)
        {
            if (!GameInProgess()) return null;
            if ((int)p.ExtraData["team"] == 2)
                return "red";
            else if ((int)p.ExtraData["team"] == 1)
                return "blue";
            else
                return null;
        }

        public void joinTeam(Player p, string Name)
        {
            if (!GameInProgess()) return;
            if ((int)p.ExtraData["team"] != 0) return;
            if (Name == "blue")
            {
                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " joined the " + Colors.blue + "blue " + Server.DefaultColor + "team!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " joined the " + Colors.blue + "blue " + Server.DefaultColor + "team!" + Colors.gray + " - ");
                p.ExtraData["team"] = 1;
                p.Color = Colors.blue;
                Command.All["goto"].Use(p, currentLevelName.Select(c => c.ToString()).ToArray());
                blu.Add(p);
                sendToTeamSpawn(p);
            }
            else
            {
                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " joined the " + Colors.red + "red " + Server.DefaultColor + "team!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " joined the " + Colors.red + "red " + Server.DefaultColor + "team!" + Colors.gray + " - ");
                p.ExtraData["team"] = 2;
                p.Color = Colors.red;
                Command.All["goto"].Use(p, currentLevelName.Select(c => c.ToString()).ToArray());
                red.Add(p);
                sendToTeamSpawn(p);
            }
        }

        public void resetFlags(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            Level level = Level.FindLevel(currentLevelName);
            if (level == null) return;
            level.BlockChange((ushort)((int)redFlag.x), (ushort)((int)redFlag.z), (ushort)((int)redFlag.y), Block.BlockList.redflag);
            level.BlockChange((ushort)((int)blueFlag.x), (ushort)((int)blueFlag.z), (ushort)((int)blueFlag.y), Block.BlockList.blueflag);
            foreach (Player ppp in Server.Players)
            {
                ppp.ExtraData["isholdingflag"] = false;
            }
            ServerCTF.blueFlagDropped = false;
            ServerCTF.redFlagDropped = false;
            ServerCTF.blueFlagHeld = false;
            ServerCTF.redFlagHeld = false;
            ServerCTF.blueFlagTimer.Enabled = false;
            ServerCTF.redFlagTimer.Enabled = false;
            Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The flags have been reset! " + Colors.gray + " - ");
            Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The flags have been reset! " + Colors.gray + " - ");
            removeTempFlagBlocks();
        }

        public void resetFlag(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            if ((int)p.ExtraData["team"] != 0)
            {
                Level level = Level.FindLevel(currentLevelName);
                if (level == null) return;
                if (getTeam(p) == "red") level.BlockChange(Convert.ToUInt16((int)redFlag.x), Convert.ToUInt16((int)redFlag.z), Convert.ToUInt16((int)redFlag.y), Block.BlockList.redflag);
                else if (getTeam(p) == "blue") level.BlockChange(Convert.ToUInt16((int)blueFlag.x), Convert.ToUInt16((int)blueFlag.z), Convert.ToUInt16((int)blueFlag.y), Block.BlockList.blueflag);
                if (getTeam(p) == "red" && ServerCTF.redFlagDropped) level.BlockChange(Convert.ToUInt16((int)redDroppedFlag.x / 32), Convert.ToUInt16((int)redDroppedFlag.z / 32 - 1), Convert.ToUInt16((int)redDroppedFlag.y / 32), Block.BlockList.AIR);
                else if (getTeam(p) == "blue" && ServerCTF.blueFlagDropped) level.BlockChange(Convert.ToUInt16((int)blueDroppedFlag.x / 32), Convert.ToUInt16((int)blueDroppedFlag.z / 32 - 1), Convert.ToUInt16((int)blueDroppedFlag.y / 32), Block.BlockList.AIR);
                removeTempFlagBlocks();
                if (getTeam(p) == "blue") ServerCTF.blueFlagDropped = false;
                else if (getTeam(p) == "red") ServerCTF.redFlagDropped = false;
                if (getTeam(p) == "red") ServerCTF.blueFlagHeld = false;
                else if (getTeam(p) == "blue") ServerCTF.redFlagHeld = false;

                if (getTeam(p) == "blue") { Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The blue flag has been returned! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The blue flag has been returned! " + Colors.gray + " - "); ServerCTF.blueFlagTimer.Enabled = false; blueDroppedFlag.x = 0; blueDroppedFlag.z = 0; blueDroppedFlag.y = 0; }
                else if (getTeam(p) == "red") { Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The red flag has been returned! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The red flag has been returned! " + Colors.gray + " - "); ServerCTF.redFlagTimer.Enabled = false; redDroppedFlag.x = 0; redDroppedFlag.z = 0; redDroppedFlag.y = 0; }
            }
        }

        public void resetFlag(string str)
        {
            if (!GameInProgess()) return;
            Level level = Level.FindLevel(currentLevelName);
            if (level == null) return;
            if (str == "red") level.BlockChange(Convert.ToUInt16((int)redFlag.x), Convert.ToUInt16((int)redFlag.z), Convert.ToUInt16((int)redFlag.y), Block.BlockList.redflag);
            else if (str == "blue") level.BlockChange(Convert.ToUInt16((int)blueFlag.x), Convert.ToUInt16((int)blueFlag.z), Convert.ToUInt16((int)blueFlag.y), Block.BlockList.blueflag);
            if (str == "red") level.BlockChange(Convert.ToUInt16((int)redDroppedFlag.x / 32), Convert.ToUInt16((int)redDroppedFlag.z / 32 - 1), Convert.ToUInt16((int)redDroppedFlag.y / 32), Block.BlockList.AIR);
            else if (str == "blue") level.BlockChange(Convert.ToUInt16((int)blueDroppedFlag.x / 32), Convert.ToUInt16((int)blueDroppedFlag.z / 32 - 1), Convert.ToUInt16((int)blueDroppedFlag.y / 32), Block.BlockList.AIR);
            if (str == "blue") ServerCTF.blueFlagDropped = false;
            else if (str == "red") ServerCTF.redFlagDropped = false;
            if (str == "blue") ServerCTF.blueFlagHeld = false;
            else if (str == "red") ServerCTF.redFlagHeld = false;
            if (str == "blue") { Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The blue flag has been returned! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The blue flag has been returned! " + Colors.gray + " - "); ServerCTF.blueFlagTimer.Enabled = false; blueDroppedFlag.x = 0; blueDroppedFlag.z = 0; blueDroppedFlag.y = 0; }
            else if (str == "red") { Server.IRC.SendMessage(Colors.gray + " - " + Server.DefaultColor + "The red flag has been returned! " + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + Server.DefaultColor + "The red flag has been returned! " + Colors.gray + " - "); ServerCTF.redFlagTimer.Enabled = false; redDroppedFlag.x = 0; redDroppedFlag.z = 0; redDroppedFlag.y = 0; }
        }

        public void PlayerDC(Player p)
        {
            ushort x, y, z; int xx, yy, zz;
            x = (ushort)((int)p.Pos.x / 32);
            y = (ushort)((int)p.Pos.z / 32 - 1);
            z = (ushort)((int)p.Pos.y / 32);
            xx = p.Pos.x;
            yy = p.Pos.z;
            zz = p.Pos.y;
            dropFlag(p, x, y, z, xx, yy, zz);
            if (red.Contains(p)) { red.Remove(p); p.ExtraData["team"] = 0; }
            if (blu.Contains(p)) { blu.Remove(p); p.ExtraData["team"] = 0; }
        }

        public void sendToTeamSpawn(Player p)
        {
            if (!GameInProgess()) return;
            ushort x, y, z; int xx, yy, zz;
            x = (ushort)((int)p.Pos.x / 32);
            y = (ushort)((int)p.Pos.z / 32 - 1);
            z = (ushort)((int)p.Pos.y / 32);
            xx = p.Pos.x;
            yy = p.Pos.z;
            zz = p.Pos.y;
            if ((int)p.ExtraData["team"] == 2)
            {
                p.SendToPos(redSpawn, p.Rot);
            }
            else
            {
                p.SendToPos(blueSpawn, p.Rot);
            }
            Thread dropThread = new Thread(new ThreadStart(delegate
            {
                Thread.Sleep(500);
                dropFlag(p, x, y, z, xx, yy, zz);
            }));
            dropThread.Start();
        }

        public void captureFlag(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            if (getTeam(p) == "blue" && !ServerCTF.blueFlagDropped && !ServerCTF.blueFlagHeld)
            {
                ServerCTF.blueFlagTimer.Enabled = false;
                blueTeamCaptures++;
                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " captured the " + Colors.red + "red " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " captured the " + Colors.red + "red " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                addMoney(p, 50, 50);
                blu.ForEach(delegate(Player player1)
                {
                    if (player1 != p)
                    {
                        addMoney(player1, 5, 5);
                    }
                });
                resetFlags(p);
                if (blueTeamCaptures > 4) { HandOutRewards(); return; }
                Server.IRC.SendMessage(Colors.gray + " - " + Colors.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + Colors.red + " Red: " + Server.DefaultColor + redTeamCaptures);
                Player.UniversalChat(Colors.gray + " - " + Colors.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + Colors.red + " Red: " + Server.DefaultColor + redTeamCaptures);
                removeTempFlagBlock("blue");
            }
            else if (getTeam(p) == "red" && !ServerCTF.redFlagDropped && !ServerCTF.redFlagHeld)
            {
                ServerCTF.redFlagTimer.Enabled = false;
                redTeamCaptures++;
                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " captured the " + Colors.blue + "blue " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " captured the " + Colors.blue + "blue " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                addMoney(p, 50, 50);
                red.ForEach(delegate(Player player1)
                {
                    if (player1 != p)
                    {
                        addMoney(player1, 5, 5);
                    }
                });
                resetFlags(p);
                if (redTeamCaptures > 4) { HandOutRewards(); return; }
                Server.IRC.SendMessage(Colors.gray + " - " + Colors.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + Colors.red + " Red: " + Server.DefaultColor + redTeamCaptures);
                Player.UniversalChat(Colors.gray + " - " + Colors.blue + "Blue: " + Server.DefaultColor + blueTeamCaptures + Colors.red + " Red: " + Server.DefaultColor + redTeamCaptures);
                removeTempFlagBlock("red");
            }
            ServerCTF.vulnerable = 1;
        }

        public void dropFlag(Player p, ushort x, ushort y, ushort z, int xx, int yy, int zz)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            Level level = Level.FindLevel(currentLevelName);
            if (level == null) return;
            if ((bool)p.ExtraData["isholdingflag"])
            {
                if (getTeam(p) == "red" && ServerCTF.blueFlagHeld) level.BlockChange(x, y, z, Block.BlockList.blueflag);
                if (getTeam(p) == "blue" && ServerCTF.redFlagHeld) level.BlockChange(x, y, z, Block.BlockList.redflag);
                if (getTeam(p) == "red") ServerCTF.blueFlagDropped = true;
                else if (getTeam(p) == "blue") ServerCTF.redFlagDropped = true;
                if (getTeam(p) == "red") { ServerCTF.blueFlagHeld = false; removeTempFlagBlock("red"); ServerCTF.blueFlagTimer.Enabled = true; blueDroppedFlag.x = (short)xx; blueDroppedFlag.z = (short)yy; blueDroppedFlag.y = (short)zz; }
                else if (getTeam(p) == "blue") { ServerCTF.redFlagHeld = false; removeTempFlagBlock("blue"); ServerCTF.redFlagTimer.Enabled = true; redDroppedFlag.x = (short)xx; redDroppedFlag.z = (short)yy; redDroppedFlag.y = (short)zz; }
                p.ExtraData["isholdingflag"] = false;
                Server.IRC.SendMessage(Colors.gray + " - " + Colors.blue + p.Username + Server.DefaultColor + " dropped the flag!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + Colors.blue + p.Username + Server.DefaultColor + " dropped the flag!" + Colors.gray + " - ");
            }
        }

        public void returnFlag(Player p)
        {
            if (!GameInProgess()) return;
            if (getTeam(p) == null) return;
            resetFlag(p);
            if (getTeam(p) == "red" && !ServerCTF.blueFlagHeld) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " returned the " + Colors.red + "red " + Server.DefaultColor + "flag!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " returned the " + Colors.red + "red " + Server.DefaultColor + "flag!" + Colors.gray + " - "); }
            if (getTeam(p) == "blue" && !ServerCTF.redFlagHeld) { Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " returned the " + Colors.blue + "blue " + Server.DefaultColor + "flag!" + Colors.gray + " - "); Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " returned the " + Colors.blue + "blue " + Server.DefaultColor + "flag!" + Colors.gray + " - "); }
        }

        public bool pickupFlag(Player p)
        {
            if (!GameInProgess()) return true;
            if (getTeam(p) == null) return true;
            if ((bool)p.ExtraData["deathtimeron"]) return true;
            if (getTeam(p) == "blue" && !ServerCTF.redFlagHeld)
            {
                p.ExtraData["isholdingflag"] = true;
                ServerCTF.redFlagHeld = true;
                ServerCTF.redFlagDropped = false;
                ServerCTF.redFlagTimer.Enabled = false;
                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " grabbed the " + Colors.red + "red " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " grabbed the " + Colors.red + "red " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                return false;
            }
            else if (getTeam(p) == "red" && !ServerCTF.blueFlagHeld)
            {
                p.ExtraData["isholdingflag"] = true;
                ServerCTF.blueFlagHeld = true;
                ServerCTF.blueFlagDropped = false;
                ServerCTF.blueFlagTimer.Enabled = false;
                Server.IRC.SendMessage(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " grabbed the " + Colors.blue + "blue " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                Player.UniversalChat(Colors.gray + " - " + p.Color + p.ExtraData["Title"] + p.Username + Server.DefaultColor + " grabbed the " + Colors.blue + "blue " + Server.DefaultColor + "flag!" + Colors.gray + " - ");
                return false;
            }
            else
            {
                return true;
            }
        }

        public void disarm(Player p, string strng)
        {
            if (strng == "mine")
            {
                p.ExtraData.ChangeOrCreate("mineplacementx", 0); p.ExtraData.ChangeOrCreate("mineplacementz", 0); p.ExtraData.ChangeOrCreate("mineplacementy", 0);
                p.ExtraData.ChangeOrCreate("minesplaced", 0);
                p.SendMessage(Colors.gray + " - " + Server.DefaultColor + "Your mine has been disarmed!" + Colors.gray + " - ");
            }
            else if (strng == "trap")
            {
                if ((int)p.ExtraData["trapsplaced"] == 0) return;
                Server.Players.ForEach(delegate(Player player1)
                {
                    player1.ExtraData["hasbeentrapped"] = false;
                });
                p.ExtraData["trapplacementx"] = 0; p.ExtraData["trapplacementz"] = 0; p.ExtraData["trapplacementy"] = 0;
                p.ExtraData["trapsplaced"] = 0;
                p.SendMessage(Colors.gray + " - " + Server.DefaultColor + "Your trap has been disarmed!" + Colors.gray + " - ");
            }
            if (strng == "tnt")
            {
                p.ExtraData.ChangeOrCreate("tntplacementx", 0); p.ExtraData.ChangeOrCreate("tntplacementz", 0); p.ExtraData.ChangeOrCreate("tntplacementy", 0);
                p.ExtraData["tntplaced"] = 0;
                p.SendMessage(Colors.gray + " - " + Server.DefaultColor + "Your tnt has been disarmed!" + Colors.gray + " - ");
            }
            else
                return;
        }

        public bool grabFlag(Player p, string Color, ushort x, ushort y, ushort z)
        {
            bool returne = false;
            if (red.Count < 1 || blu.Count < 1) { p.SendMessage("There must be at least 2 players on both teams to grab a flag!"); return true; }
            if (getTeam(p) == Color)
            {
                if (getTeam(p) == "blue" && ServerCTF.blueFlagDropped && !ServerCTF.blueFlagHeld)
                    returnFlag(p);
                else if (getTeam(p) == "red" && ServerCTF.redFlagDropped && !ServerCTF.redFlagHeld)
                    returnFlag(p);
                else
                {
                    if ((bool)p.ExtraData["isholdingflag"])
                    {
                        captureFlag(p);
                    }
                    returne = true;
                }
            }
            else
            {
                if (getTeam(p) == "blue" && !ServerCTF.redFlagHeld)
                    returne = pickupFlag(p);
                else if (getTeam(p) == "red" && !ServerCTF.blueFlagHeld)
                    returne = pickupFlag(p);
                else
                {
                    returne = false;
                }
            }
            return returne; //return true if you dont want to destroy block
        }


        public void addMoney(Player p, int amount, int points)
        {
            p.SendMessage( Colors.gray + " - " + Server.DefaultColor + "You gained " + amount + " " + Server.Moneys + " and " + points + " EXP!" + Colors.gray + " - ");
            p.Money = p.Money + amount;
            p.ExtraData["points"] = (int)p.ExtraData["points"] + points;
        }

        bool OnSide(Player p, string Name)
        {
            Vector3S b = null;
            if (Name == "red")
            {
                b.x = redSpawn.x;
            }
            else
            {
                b.x = blueSpawn.x;
            }
            if (b.x < halfway && p.Pos.x / 32 < halfway)
                return true;
            else if (b.x > halfway && p.Pos.x / 32 > halfway)
                return true;
            else
                return false;
        }

        public bool InSpawn(Player p, Vector3S Pos)
        {
            if (getTeam(p) == "blue")
            {
                if (Math.Abs((Pos.x / 32) - blueSpawn.x) <= 5
                    && Math.Abs((Pos.z / 32) - blueSpawn.z) <= 5
                    && Math.Abs((Pos.y / 32) - blueSpawn.y) <= 5)
                {
                    return true;
                }
            }
            if (getTeam(p) == "red")
            {
                if (Math.Abs((Pos.x / 32) - redSpawn.x) <= 5
                    && Math.Abs((Pos.z / 32) - redSpawn.z) <= 5
                    && Math.Abs((Pos.y / 32) - redSpawn.y) <= 5)
                {
                    return true;
                }
            }
            return false;
        }
        static void CheckXP()
        {
            Server.Players.ForEach(delegate(Player pl)
            {
                try
                {
                    EXPLevel nextLevel = EXPLevel.Find(EXPLevel.Find((int)pl.ExtraData["explevel"]).levelID + 1);
                    if (nextLevel != null && (int)pl.ExtraData["points"] >= nextLevel.requiredEXP)
                    {
                        pl.ExtraData["explevel"] = nextLevel;
                        pl.Money += nextLevel.reward;
                        Player.UniversalChat(pl.Color + pl.Username + Server.DefaultColor + " has leveled up to level &a" + nextLevel.levelID + "!");
                        pl.SendMessage("You have just leveled up to level &a" + nextLevel.levelID + "!");
                        pl.SendMessage("&6You were rewarded &a" + nextLevel.reward + " " + Server.Moneys + ".");
                    }
                }
                catch
                {
                    // user probably only just connected- so explevel wont have been set yet.
                }
            });
        }
    }
}