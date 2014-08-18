/*
Copyright 2012 Snowl
For use only with www.caznowl.net
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Timers;
using MCForge;
using MCForge.Interface.Plugin;
using MCForge.Entity;
using MCForge.API.Events;
using MCForge.API.Events.Robot;
using MCForge.Core;
using MCForge.Utils.Settings;
using MCForge.Robot;
using MCForge.World;
using MCForge.Utils;
using ZombiePlugin;
using System.Net;
using System.Drawing;
using System.IO;
using MCForge.SQL;
using MCForge.Interface.Command;

namespace ZombiePlugin
{
    public class ZombiePlugin : IPlugin
    {
        public string Name { get { return "Zombie Survival"; } }
        public string Author { get { return "Snowl"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return "com.caznowl.zombiesurvival"; } }

        #region Data Storage
        public static List<ExtraPlayerData> ExtraPlayerData;
        private static List<String> WomExempt = new List<string>();
        private static List<String> OmniBan = new List<string>();
        private string[] LevelChoices = { "", "" };
        #endregion

        #region Variables
        public static bool ZombieRoundEnabled = false;
        public static int AmountOfSecondsElapsed = 0;
        public static int AmountOfMinutesElapsed = 1;
        public static bool CureOn = false;
        public static bool Voting = false;
        public static int Gamemode = 0; // 0 is normal, 1 is classic, 2 is classic happy fun mode, 3 is cure
        public static string StreakPlayer = "";
        public static int StreakAmount = 0;

        //Queueing Variables
        public static bool ZombieQueued = false;
        public static string ZombieNameQueued = "";
        public static bool LevelQueued = false;
        public static string LevelNameQueued = "";
        public static bool GameModeQueued = false;
        public static int GameModeIntQueued = 0;

        //Anti-hack consts
        public const int AmountOfChecksInAir = 35;
        public const int AmountOfNoClips = 3;

        //Priv8 vars
        private Level ZombieLevel;
        private Vector3S LevelVotes = new Vector3S(0, 0, 0);
        private static Timer ZombieTimer;
        private static bool Initialized;
        internal static readonly System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        #endregion

        public class ZombieSurvival
        {
            /*
             * players get ghosts
             * /fly doesnt work
             * players can break beadrock
             * refs dont go invisible
             * WoM means auto-ref
             * score doesnt really work
             * players get high amounts of money
             * mcforge high CPU usage
             * classic happy fun mode doesnt work properly still
             * bots whitelist to easily
             */
        }

        public void Initialize()
        {
            Logger.Log("[ZS]: Zombie Survival Version " + Version + " Initializing...", Color.Black, Color.White);
            Plugin.AddReference(this);

            FileUtils.CreateDirIfNotExist("bannedskins");
            FileUtils.CreateDirIfNotExist("stafftime");
            FileUtils.CreateDirIfNotExist("zombietext");

            FileUtils.CreateFileIfNotExist("zombietext/infectmessages.txt", "#Use * for the infectee and ! for the infected\r\n");
            FileUtils.CreateFileIfNotExist("zombietext/disinfectmessages.txt", "#Use * for the disinfectee and ! for the disinfected\r\n");
            FileUtils.CreateFileIfNotExist("zombietext/womexempt.txt");
            FileUtils.CreateFileIfNotExist("zombietext/nobotlevels.txt");
            FileUtils.CreateFileIfNotExist("zombietext/swearfilter.txt");

            #region Load File Contents & Omniban
            string[] allLines = File.ReadAllLines("zombietext/infectmessages.txt");
            foreach (string s in allLines)
            {
                if (!s.StartsWith("#"))
                    ZombieHelper.InfectMessages.Add(s);
            }

            string[] allDisLines = File.ReadAllLines("zombietext/disinfectmessages.txt");
            foreach (string s in allDisLines)
            {
                if (!s.StartsWith("#"))
                    ZombieHelper.DisinfectMessages.Add(s);
            }

            string[] allWomLines = File.ReadAllLines("zombietext/womexempt.txt");
            foreach (string s in allWomLines)
            {
                if (!s.StartsWith("#"))
                    WomExempt.Add(s.ToLower());
            }

            string[] norobotLines = File.ReadAllLines("zombietext/nobotlevels.txt");
            foreach (string s in norobotLines)
            {
                if (!s.StartsWith("#"))
                    ZombieHelper.NoBotLevels.Add(s.ToLower());
            }

            string[] swearFilter = File.ReadAllLines("zombietext/swearfilter.txt");
            foreach (string s in swearFilter)
            {
                if (!s.StartsWith("#"))
                    ZombieHelper.SwearFilter.Add(s.ToLower());
            }

            WebClient client = new WebClient();
            #endregion

            if (ServerSettings.GetSettingInt("WOMPermission") == -1)
            {
                ServerSettings.SetSetting("WOMPermission", "Permission for WorldOfMinecraft", "0");
                ServerSettings.Save();
            }

            #region Event Handling
            Player.OnAllPlayersChat.Normal += OnChat;
            Player.OnAllPlayersConnect.Normal += OnConnect;
            Player.OnAllPlayersDisconnect.Normal += OnDisconnect;
            Player.OnAllPlayersMove.Normal += OnMove;
            Player.OnAllPlayersBlockChange.Normal += OnBlockChange;
            Player.OnAllPlayersSendPacket.Normal += OnSendPacket;
            Player.OnAllPlayersCommand.Normal += OnCommand;
            Bot.OnBotTargetPlayer.Normal += OnBotTargetPlayer;
            #endregion

            ExtraPlayerData = new List<ExtraPlayerData>();
            foreach (Player p in Server.Players.ToArray())
                ExtraPlayerData.Add(new ExtraPlayerData(p));

            foreach (Bot b in Server.Bots.ToArray())
            {
                if (b.Player.Level == ZombieLevel)
                {
                    b.Player.GlobalDie();
                    b.FollowPlayers = false;
                    b.BreakBlocks = false;
                    b.Jumping = false;
                    b.Unload();
                    Server.Bots.Remove(b);
                }
            }

            ZombieTimer = new Timer(1000); //Every second
            ZombieTimer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            ZombieTimer.Enabled = true;

            Gamemode = 1;

            Logger.Log("[ZS]: Zombie Survival Version " + Version + " Initialized!", Color.Black, Color.White);
            Logger.Log("Round starts in 1 minute [Gamemode: " + ZombieHelper.GetGamemode(Gamemode) + "]");
            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Round starts in 1 minutes [Gamemode: " + ZombieHelper.GetGamemode(Gamemode) + "]");
        }

        #region Event Functions

        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            #region Increase Minutes and Seconds
            if (AmountOfSecondsElapsed < 10000)
                AmountOfSecondsElapsed += 1;
            else
            {
                AmountOfMinutesElapsed = 0;
                AmountOfSecondsElapsed = 0;
            }
            if (AmountOfSecondsElapsed % 60 == 0)
                AmountOfMinutesElapsed += 1;
            int TemporaryMinutesElapsed = AmountOfMinutesElapsed - 1;
            #endregion

            if (!ZombieRoundEnabled && !Voting)
            {
                if (TemporaryMinutesElapsed == 1 && AmountOfSecondsElapsed % 60 == 0)
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Round starts in 1 minute [Gamemode: " + ZombieHelper.GetGamemode(Gamemode) + "]");
                    return;
                }
                else if (TemporaryMinutesElapsed == 2 && -(AmountOfSecondsElapsed % 60 - 5) != 0 && -(AmountOfSecondsElapsed % 60 - 5) > 0)
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Round starts in " + -(AmountOfSecondsElapsed % 60 - 5) + " seconds");
                    return;
                }
                else if (TemporaryMinutesElapsed == 2 && -(AmountOfSecondsElapsed % 60 - 5) == 0)
                {
                    if (!Initialized)
                    {
                        ZombieHelper.ClearDatabaseBlocks();
                        ZombieLevel = Server.Mainlevel;
                        Initialized = true;
                    }
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "The round has started! [Gamemode: " + ZombieHelper.GetGamemode(Gamemode) + "]");
                    if (Gamemode == 0 || Server.PlayerCount < 2)
                    {
                        if ((Server.PlayerCount == 0 || Server.PlayerCount == 1) && Gamemode > 0)
                        {
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Game mode defaulting to Normal");
                            Gamemode = 0;
                        }
                        short x = (short)((0.5 + ZombieLevel.SpawnPos.x) * 32);
                        short y = (short)((1 + ZombieLevel.SpawnPos.y) * 32);
                        short z = (short)((0.5 + ZombieLevel.SpawnPos.z) * 32);
                        Bot ZombieBot = new Bot("UndeaadBot", new Vector3S(x, z, y), new byte[] { 0, 0 }, ZombieLevel, false, false, false);
                        ZombieBot.Player.DisplayName = "";
                        ZombieBot.Player.IsHeadFlipped = true;
                        ZombieBot.FollowPlayers = true;
                        ZombieBot.BreakBlocks = true;
                        ZombieBot.Jumping = true;
                    }
                    else
                    {
                        if (!ZombieQueued)
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + ZombieHelper.InfectRandomPlayerReturn(ExtraPlayerData).Player.Username + " was chosen to be infected first!");
                        else
                        {
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + ZombieNameQueued + " was chosen to be infected first!");
                            ExtraPlayerData tes = FindPlayer(ZombieNameQueued);
                            if (tes == null)
                            {
                                Player.UniversalChat("[Zombie Survival]: " + Colors.red + ZombieHelper.InfectRandomPlayerReturn(ExtraPlayerData).Player.Username + " was chosen to be infected first!");
                            }
                            else
                            {
                                ZombieHelper.InfectPlayer(tes);
                                ZombieNameQueued = "";
                                ZombieQueued = false;
                            }
                        }
                    }
                    AmountOfMinutesElapsed = 1;
                    AmountOfSecondsElapsed = 0;
                    ZombieRoundEnabled = true;
                }
            }
            else if (!Voting)
            {
                if (TemporaryMinutesElapsed > 2 && TemporaryMinutesElapsed < 7 && AmountInfected() <= 0 && Server.PlayerCount > 1) 
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "No players are infected... randomly infecting a player!");
                    ExtraPlayerData temp = ZombieHelper.InfectRandomPlayerReturn(ExtraPlayerData);
                    ZombieHelper.DisplayInfectMessage(temp.Player, null);
                    EndGame();
                }
                else if ((TemporaryMinutesElapsed == 3 || TemporaryMinutesElapsed == 5) && AmountOfSecondsElapsed % 60 == 0 && Gamemode == 2) //Happy Fun Mode
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "A slight zombie robot shower is forcast for... right now.");
                    ZombieHelper.RainBots(ZombieLevel);
                }
                else if ((TemporaryMinutesElapsed == 4 || TemporaryMinutesElapsed == 5) && AmountOfSecondsElapsed % 60 == 0 && Gamemode == 3) //Cure
                {
                    ZombieHelper.ToggleCure();
                }
                else if (TemporaryMinutesElapsed >= 9)
                {
                    EndGame(true);
                }
                else if (-(AmountOfSecondsElapsed % 60 - 5) == 0)
                {
                    EndGame();
                }
            }
            else
            {
                if (TemporaryMinutesElapsed >= 1)
                {
                    TemporaryMinutesElapsed = 0;
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Voting has ended!");
                    string winner = "";
                    if (!LevelQueued)
                    {
                        if (LevelVotes.x > LevelVotes.y && LevelVotes.x > LevelVotes.z)
                        {
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + LevelChoices[0] + " has won the vote!");
                            winner = LevelChoices[0];
                        }
                        else if (LevelVotes.y > LevelVotes.x && LevelVotes.y > LevelVotes.z)
                        {
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + LevelChoices[1] + " has won the vote!");
                            winner = LevelChoices[1];
                        }
                        else
                        {
                            winner = ZombieHelper.FindRandomLevel(new string[2] { "main", ZombieLevel.Name });
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "random has won the vote, " + winner + " will be the next level!");
                        }
                    }
                    else
                    {
                        winner = LevelNameQueued;
                        LevelNameQueued = "";
                        LevelQueued = false;
                        if (LevelChoices[0] == winner || LevelChoices[1] == winner)
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + winner + " has won the vote!");
                        else
                            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "random has won the vote, " + winner + " will be the next level!");
                    }

                    Level TempLevel = ZombieLevel;
                    Level isAlreadyLoaded = Level.FindLevel(winner);
                    if (isAlreadyLoaded == null)
                    {
                        ZombieLevel = Level.LoadLevel(winner);
                    }
                    else
                    {
                        ZombieLevel = isAlreadyLoaded;
                    }

                    if (ZombieLevel == null)
                    {
                        ZombieLevel = Server.Mainlevel;
                        Player.UniversalChat("Oops, " + winner + " couldn't be loaded! Please notify the admins");
                    }
                    Level.Levels.Add(ZombieLevel);
                    foreach (ExtraPlayerData z in ExtraPlayerData.ToArray())
                    {
                        try
                        {
                            z.Player.Level = ZombieLevel;
                            z.LastLoc = z.Player.Level.SpawnPos*32;
                            z.Player.oldPos = z.Player.Level.SpawnPos*32;
                            short x = (short) ((0.5 + ZombieLevel.SpawnPos.x)*32);
                            short y = (short) ((1 + ZombieLevel.SpawnPos.y)*32);
                            short m = (short) ((0.5 + ZombieLevel.SpawnPos.z)*32);
                            z.Player.Pos = new Vector3S(x, m, y);
                            z.Player.Rot = ZombieLevel.SpawnRot;
                            z.Player.oldPos = z.Player.Pos;
                            z.Player.oldRot = z.Player.Rot;
                            z.Player.SendSpawn(z.Player);
                            z.Player.SpawnThisPlayerToOtherPlayers();
                            z.Player.SpawnOtherPlayersForThisPlayer();
                            z.Player.SpawnBotsForThisPlayer();
                        }
                        catch {Player.UniversalChat("Couldn't move " + z.Player.Username + "... notify MCForge devs!");
                        Logger.Log("Couldn't move " + z.Player.Username + " to level " + ZombieLevel.Name);}
                    }
                    if (TempLevel != Server.Mainlevel)
                        Level.Levels.Remove(TempLevel);
                    Voting = false;
                    AmountOfMinutesElapsed = 0;
                    AmountOfSecondsElapsed = -1;
                    ZombieHelper.ClearDatabaseBlocks();
                }
            }
            EndGame();
        }

        public void OnChat(Player Player, ChatEventArgs args)
        {
            ExtraPlayerData P = FindPlayer(Player);
            if (Voting && IsNotImportant(args.Message))
            {
                int l = args.Message.Length;
                if (l > 3)
                    l = 3;
                else if (l <= 0)
                    l = 1;
                Player.ExtraData.CreateIfNotExist("Voted", false);
                var didVote = (bool)Player.ExtraData.GetIfExist("Voted");
                if (args.Message.ToLower().Equals(LevelChoices[0].Substring(0, l).ToString().ToLower()) && !didVote)
                {
                    Player.SendMessage("[Zombie Survival]: " + Colors.red + "Thank you for voting for " + LevelChoices[0]);
                    LevelVotes.x += 1;
                    Player.ExtraData["Voted"] = true;
                    args.Cancel();
                }
                else if (args.Message.ToLower().Equals(LevelChoices[1].Substring(0, l).ToString().ToLower()))
                {
                    Player.SendMessage("[Zombie Survival]: " + Colors.red + "Thank you for voting for " + LevelChoices[1]);
                    LevelVotes.y += 1;
                    Player.ExtraData["Voted"] = true;
                    args.Cancel();
                }
                else if (args.Message.ToLower().Equals("random".Substring(0, l).ToString().ToLower()))
                {
                    Player.SendMessage("[Zombie Survival]: " + Colors.red + "Thank you for voting for random");
                    LevelVotes.z += 1;
                    Player.ExtraData["Voted"] = true;
                    args.Cancel();
                }
                else if (!(bool)Player.ExtraData.GetIfExist("Voiced") && !P.Referee)
                {
                    Player.SendMessage("[Zombie Survival]: " + Colors.red + "You may not speak while voting, sorry :(");
                    args.Cancel();
                }
                else
                {
                    args.Message = ZombieHelper.CleanString(Player, args.Message);
                    if (args.Message == "")
                    {
                        args.Cancel();
                        return;
                    }
                    return;
                }
            }
            else if (IsNotImportant(args.Message))
            {
                var voiceString = (string)Player.ExtraData.GetIfExist("VoiceString") ?? "";
                var mColor = Player.Group.Color;
                var mPrefix = (string)Player.ExtraData.GetIfExist("Title") ?? "";
                string msg = voiceString + mPrefix + mColor + Player.Username + ": &f" + args.Message;
                if (P.Referee)
                {
                    msg = Colors.gold + "[Referee] " + msg;
                }
                else if (P.Infected)
                {
                    msg = Colors.red + "[Infected] " + msg;
                }
                else
                {
                    msg = Colors.green + "[Survivor] " + msg;
                }
                string l = ZombieHelper.CleanString(Player, msg);
                if (l == "")
                {
                    args.Cancel();
                    return;
                }
                Player.UniversalChat(l);
                Server.IRC.SendMessage(l);
                args.Cancel();
                return;
            }
            else
            {
                args.Message = ZombieHelper.CleanString(Player, args.Message);
                if (args.Message == "")
                {
                    args.Cancel();
                    return;
                }
                return;
            }
        }

        public void OnConnect(Player Player, ConnectionEventArgs args)
        {
            if (Player.Group.Permission < ServerSettings.GetSettingInt("StaffTimePermission"))
            {
                try
                {
                    string picUri = "http://s3.amazonaws.com/MinecraftSkins/" + Player.Username + ".png";
                    WebRequest requestPic = WebRequest.Create(picUri);
                    WebResponse responsePic = requestPic.GetResponse();
                    Image webImage = Image.FromStream(responsePic.GetResponseStream());

                    foreach (string s in Directory.GetFiles("bannedskins", "*.png"))
                    {
                        if (ZombieHelper.CompareImages(ZombieHelper.GetBitmap(s), (Bitmap)webImage))
                        {
                            Player.Kick("This skin is not allowed to be used on this server!");
                            args.Cancel();
                            return;
                        }
                    }
                }
                catch { } //404 if invalid skin, do nothing
            }

            if (OmniBan.Contains(Player.Username.ToLower()) || OmniBan.Contains(Player.Ip + ""))
            {
                Player.Kick("You have been omnibanned from this server!");
                args.Cancel();
                return;
            }
    
            if (ZombieLevel != null)
                if (Player.Level != ZombieLevel)
                    Player.Level = ZombieLevel;

            ExtraPlayerData l = new ExtraPlayerData(Player);
            ExtraPlayerData.Add(l);
            ZombieHelper.ResetPlayer(l);
            if (!ZombieRoundEnabled && !Voting)
            {
                if (AmountOfMinutesElapsed - 1 != 2)
                    Player.SendMessage("[Zombie Survival]: " + Colors.red + "Round starts in " + -(AmountOfMinutesElapsed - 2) + ":" + (-(AmountOfSecondsElapsed % 60 - 60)).ToString("D2") + " minutes [Gamemode: " + ZombieHelper.GetGamemode(Gamemode) + "]");
            }
            else if (!Voting)
            {
                l.Infected = true;
                l.Player.IsHeadFlipped = true;
                l.Player.DisplayName = Colors.red + "Undeaad";
                Player.SendMessage("[Zombie Survival]: " + Colors.red + "A zombie game is in progress! You are infected as you joined before the game started. [Gamemode: " + ZombieHelper.GetGamemode(Gamemode) + "]");
            }
            else
            {
                Player.SendMessage("[Zombie Survival]: " + Colors.red + "Vote on the next level by typing the level name!");
                Player.SendMessage("[Zombie Survival]: " + Colors.red + "Options: " + LevelChoices[0] + "/" + LevelChoices[1] + "/random");
            }
        }

        public void OnDisconnect(Player Player, ConnectionEventArgs args)
        {
            bool Referee = false;
            long DateTimeAtStartOfRef = 0;
            foreach (ExtraPlayerData e in ExtraPlayerData)
            {
                if (e.Player.Username == Player.Username)
                {
                    if (e.Referee)
                        Referee = true;
                    DateTimeAtStartOfRef = e.DateTimeAtStartOfRef;
                    break;
                }
            }

            try
            {
                if (Referee)
                {
                    long endTick = DateTime.Now.Ticks;
                    long tick = 0;
                    try
                    {
                        tick = endTick - DateTimeAtStartOfRef;
                    }
                    catch { }
                    TextReader tr = new StreamReader("stafftime//" + Player.Username + ".time");
                    long amountOfTime = 0;
                    try
                    {
                        amountOfTime = long.Parse(tr.ReadLine());
                    }
                    catch { tr.Close(); return; }
                    tr.Close();

                    amountOfTime += tick;
                    try
                    {
                        TextWriter tw = new StreamWriter("stafftime//" + Player.Username + ".time", false);
                        tw.WriteLine(amountOfTime);
                        tw.Close();
                    }
                    catch { }
                }
            }
            catch { }

            ExtraPlayerData.Remove(FindPlayer(Player.Username));
            if (AmountInfected() <= 0 && AmountSurvived() > 0 && !Player.IsBot && ZombieRoundEnabled)
            {
                if (Server.Players.Count > 0)
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + ZombieHelper.InfectRandomPlayerReturn(ExtraPlayerData).Player.Username + " was chosen after the last infected disconnected!");
                    EndGame();
                }
                else
                    EndGame(true);
            }
        }

        public void OnMove(Player Player, MoveEventArgs args)
        {
            try
            {
                if (Player.Group.Permission < ServerSettings.GetSettingInt("WOMPermission") &&
                    !WomExempt.Contains(Player.Username.ToLower()))
                {
                    if ((bool) (Player.ExtraData.GetIfExist("UsingWoM")) && !Player.IsBeingKicked)
                    {
                        Player.Kick("WoM is not allowed on this server!");
                        args.Cancel();
                    }
                }

            ExtraPlayerData temp = FindPlayer(Player);
            if (temp == null)
                return;
            if (!temp.SentUserType)
            {
                temp.SentUserType = true;
                ZombieHelper.SendUserType(temp);
            }

            #region Anti-Hack
            Vector3S delta = new Vector3S((short)Math.Abs(Player.Pos.x - Player.oldPos.x),
                (short)Math.Abs(Player.Pos.z - Player.oldPos.z),
                (short)Math.Abs(Player.Pos.y - Player.oldPos.y));

            Vector3S delta32 = new Vector3S((short)(Math.Abs(Player.Pos.x - Player.oldPos.x) / 32),
                (short)(Math.Abs(Player.Pos.z - Player.oldPos.z) / 32),
                (short)(Math.Abs(Player.Pos.y - Player.oldPos.y) / 32));

            bool posChanged = ( delta.x != 0 ) || ( delta.y != 0 ) || ( delta.z != 0 );
            bool pos32Changed = (delta.x != 0) || (delta.y != 0) || (delta.z != 0);
            // skip everything if player hasn't moved
            if (!posChanged) return;
            if (!temp.Referee && !temp.Player.IsLoading && !temp.Player.IsBot)
            {
                if (delta.x >= 160) //Allows 5 blocks a second... more than enough with lag
                {
                    if (temp.Spawning == false)
                    {
                        temp.Spawning = true;
                        Player.SendToPos(Player.oldPos, Player.Rot);
                        return;
                    }
                }
                else if (delta.z >= 160) //Allows 5 blocks a second... more than enough with lag
                {
                    if (temp.Spawning == false)
                    {
                        temp.Spawning = true;
                        Player.SendToPos(Player.oldPos, Player.Rot);
                        return;
                    }
                }
                else if (temp.Spawning == true)
                {
                    temp.Spawning = TriBool.Unknown;
                }
                else if (temp.Spawning == TriBool.Unknown)
                {
                    temp.Spawning = false;
                }

                try
                {
                    if (Player.Pos.y > 64 && !temp.Referee && (Player.Pos.y / 32) < Player.Level.Size.y)
                    {
                        if (Block.CanWalkThrough(Player.Level.GetBlock((Player.Pos.x / 32), (Player.Pos.z / 32), (Player.Pos.y / 32) - 2)) &&
                            Block.CanWalkThrough(Player.Level.GetBlock((Player.Pos.x / 32), (Player.Pos.z / 32), (Player.Pos.y / 32) - 3)) &&
                            !Block.CanEscalate(Player.Level.GetBlock((Player.Pos.x / 32), (Player.Pos.z / 32), (Player.Pos.y / 32) - 2)) &&
                            !Block.CanEscalate(Player.Level.GetBlock((Player.Pos.x / 32), (Player.Pos.z / 32), (Player.Pos.y / 32) - 1)) &&
                            !Block.CanEscalate(Player.Level.GetBlock(Player.Pos / 32)))
                        {
                            if ((Player.Pos.y / 32) - (args.FromPosition.y / 32) == 0)
                            {
                                temp.AmountOfTimesInAir++;
                                if (temp.AmountOfTimesInAir == AmountOfChecksInAir)
                                {
                                    temp.Player.Kick("You are not allowed to fly on this server!");
                                    args.Cancel();
                                    return;
                                }
                            }
                            if ((Player.Pos.y / 32) - (args.FromPosition.y / 32) == -1)
                            {
                                temp.AmountOfTimesInAir--;
                            }
                        }
                        else
                        {
                            temp.AmountOfTimesInAir = 0;
                        }
                    }
                    else
                    {
                        temp.AmountOfTimesInAir = 0;
                    }
                }
                catch { }

                try
                {
                    if ((Player.Pos.y / 32) < Player.Level.Size.y)
                    {
                        if (!Block.CanWalkThrough(Player.Level.GetBlock((Player.Pos.x / 32), (Player.Pos.z / 32), (Player.Pos.y / 32))) && !temp.Referee && !temp.Player.IsLoggedIn && !temp.Player.IsLoading)
                        {
                            temp.AmountOfNoClips++;
                            if (temp.AmountOfNoClips == AmountOfNoClips)
                            {
                                temp.Player.Kick("You are not allowed to no-clip on this server!");
                                args.Cancel();
                                return;
                            }
                        }
                    }
                }
                catch { }
            }
            #endregion

            #region Tagging Code
            try //Positions error when loading, just incase the error code doesn't fix this already
            {
                if (ZombieRoundEnabled)
                {
                    foreach (ExtraPlayerData Player1 in ExtraPlayerData.ToArray())
                    {
                        Vector3S pos1 = Player1.Player.Pos;
                        foreach (ExtraPlayerData Player2 in ExtraPlayerData.ToArray()) //Double ieterate
                        {
                            Vector3S pos2 = Player2.Player.Pos;

                            if (!Player1.Player.IsLoading && !Player2.Player.IsLoading)
                            {
                                if (Math.Abs(pos1.x - pos2.x) <= 32 && Math.Abs(pos1.y - pos2.y) <= 64 && Math.Abs(pos1.z - pos2.z) <= 32
                                    && Player1 != Player2 && !Player1.Referee && !Player2.Referee && ((Player1.Infected && !Player2.Infected && !CureOn) || (!Player1.Infected && Player2.Infected && CureOn))) //Check if intersecting within a 1 block radii
                                {
                                    ZombieHelper.InfectPlayer(Player1, Player2);
                                    if (StreakPlayer == Player1.Player.Username)
                                    {
                                        StreakAmount++;
                                        if (StreakAmount % 3 == 0)
                                        {
                                            Player.UniversalChat("WHOA! " + Player1.Player.Username + " is on a " + StreakAmount + " streak!");
                                            ZombieHelper.GiveMoney(Player1.Player, ZombieHelper.Random.Next(1, 15));
                                        }
                                    }
                                    else
                                    {
                                        StreakAmount = 0;
                                        StreakPlayer = Player1.Player.Username;
                                    }
                                    EndGame();
                                }
                            }
                        }

                        foreach (Bot Robot in Server.Bots.ToArray()) //Robot Iterate
                        {
                            if (!Player1.Player.IsLoading)
                            {
                                if (Math.Abs(pos1.x - Robot.Player.Pos.x) <= 32 && Math.Abs(pos1.y - Robot.Player.Pos.y) <= 64 && Math.Abs(pos1.z - Robot.Player.Pos.z) <= 32
                                    && !Player1.Referee && !Player1.Infected && Robot.FollowPlayers) //Check if intersecting within a 1 block radii
                                {
                                    ZombieHelper.InfectPlayer(Robot.Player, Player1);
                                    EndGame();
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            try
            {
                if (pos32Changed && ZombieRoundEnabled)
                {
                    temp.AmountMoved++;
                }
            }
            catch{}

            #endregion
            }
            catch { }
        }

        public void OnBlockChange(Player Player, BlockChangeEventArgs args)
        {
            ExtraPlayerData p = FindPlayer(Player);

            #region Block Placing Limit
            if (p.AmountOfBlocksLeft <= 0 && ZombieRoundEnabled && args.Action == ActionType.Place)
            {
                Player.SendMessage("[Zombie Survival]: " + Colors.red + "You don't have any more blocks! You can buy more at the /store!");
                args.Cancel();
                return;
            }
            if (ZombieRoundEnabled && args.Action == ActionType.Place && !p.Referee)
            p.AmountOfBlocksLeft -= 1;
            if ((p.AmountOfBlocksLeft % 10 == 0 || p.AmountOfBlocksLeft < 6) && ZombieRoundEnabled && args.Action == ActionType.Place && !p.Referee)
                p.Player.SendMessage("[Zombie Survival]: " + Colors.red + "You have " + p.AmountOfBlocksLeft + " blocks left!");
            #endregion

            #region Anti-spleef/bedrock
            if (args.Action == ActionType.Delete && Player.Level.GetBlock(args.X, args.Z, args.Y) == Block.BlockList.BEDROCK && !p.Referee)
            {
                p.Player.SendMessage("[Zombie Survival]: " + Colors.red + "You cannot break this >:(");
                if (p.TheTypeOfNotification != 1)
                    WOM.GlobalSendAlert(Player.Username + " tried to break bedrock (Hacking?)");
                p.TheTypeOfNotification = 1;
                args.Cancel();
                return;
            }

            if (args.Action == ActionType.Delete && !p.Referee)
            {
                foreach (Player z in Server.Players.ToArray())
                {
                    if (new Vector3S(args.X, args.Z, (ushort)(args.Y + 2)) == (z.Pos / 32) && !z.IsHidden && z != Player)
                    {
                        p.Player.SendMessage("[Zombie Survival]: " + Colors.red + "You cannot spleef another player >:(");
                        if (p.TheTypeOfNotification != 2)
                            WOM.GlobalSendAlert(Player.Username + " spleefed " + z.Username);
                        p.TheTypeOfNotification = 2;
                        args.Cancel();
                        return;
                    }
                }
            }
            #endregion

            #region Anti-pillaring
            if (args.Y - p.LastYValue == 1 && !p.Referee)
            {
                p.AmountOfPillars += 1;
                if (p.AmountOfPillars > 7)
                {
                    p.Player.Kick("You are not allowed to pillar on this server!");
                }
            }
            else
            {
                if (p.AmountOfPillars > 0)
                {
                    p.AmountOfPillars -= 1;
                }
            }
            p.LastYValue = args.Y;
            #endregion
        }

        public void OnSendPacket(Player Player, PacketEventArgs args) //Aka support
        {
            ExtraPlayerData TemporaryPlayer = FindPlayer(Player);
            if (args.Type == Packet.Types.SendSpawn && args.Data[0] != 255)
            {
                if (TemporaryPlayer.Aka)
                {
                    ExtraPlayerData otherPlayer = FindPlayer(args.Data[1]);
                    byte[] b = enc.GetBytes(otherPlayer.Player.Username.PadRight(64));
                    Array.Copy(b, 0, args.Data, 2, b.Length);
                }
            }
        }

        public void OnCommand(Player Player, CommandEventArgs args) //yay for being smart, instead of recoding the command just over-ride it
        {
            ExtraPlayerData TemporaryPlayer = FindPlayer(Player);
            if (args.Command.ToLower() == "tp" || args.Command.ToLower() == "summon" || args.Command.ToLower() == "s")
            {
                if (!TemporaryPlayer.Referee)
                {
                    Player.SendMessage("You need to be a referee to do that!");
                    args.Cancel();
                    return;
                }
            }
            else if (args.Command.ToLower() == "rob" || args.Command.ToLower() == "run" || args.Command.ToLower() == "ascend"
                || args.Command.ToLower() == "bot" || args.Command.ToLower() == "test")
            {
                Player.SendMessage("Unknown command \"" + args.Command + "\"!");
                args.Cancel();
                return;
            }
            else if (args.Command.ToLower() == "spawn")
            {
                TemporaryPlayer.Spawning = true;
                if (!TemporaryPlayer.Infected && ZombieRoundEnabled)
                {
                    ZombieHelper.InfectPlayer(TemporaryPlayer);
                    ZombieHelper.DisplayInfectMessage(TemporaryPlayer.Player, null);
                    EndGame();
                }
            }
            else
            {
                try
                {
                    ICommand cmd = Command.Find(args.Command);
                    if (cmd.Type == CommandTypes.Building)
                    {
                        Player.SendMessage("Unknown command \"" + args.Command + "\"!");
                        args.Cancel();
                        return;
                    }
                }
                catch { }
            }
            
        }

        public void OnBotTargetPlayer(Bot Bot, TargetPlayerArgs args)
        {
            ExtraPlayerData TemporaryPlayer = FindPlayer(args.Player);
            if (TemporaryPlayer == null)
            {
                args.Cancel();
                return;
            }
            else if (TemporaryPlayer.Infected || TemporaryPlayer.Referee)
            {
                args.Cancel();
                return;
            }
        }

        public void OnLoad(string[] args)
        {
            if (ServerSettings.GetSettingBoolean("ZombieSurvival") == true)
            {
                Initialize();
            }
        }

        public void OnUnload()
        {
            if (ServerSettings.GetSettingBoolean("ZombieSurvival") == true)
            {
                ZombieHelper.ResetPlayers(ExtraPlayerData);
                ZombieRoundEnabled = false;
                ZombieTimer.Stop();
                ZombieTimer.Dispose();
                Player.OnAllPlayersChat.Normal -= OnChat;
                Player.OnAllPlayersConnect.Normal -= OnConnect;
                Player.OnAllPlayersDisconnect.Normal -= OnDisconnect;
                Player.OnAllPlayersMove.Normal -= OnMove;
                Player.OnAllPlayersBlockChange.Normal -= OnBlockChange;
                Player.OnAllPlayersSendPacket.Normal -= OnSendPacket;
                Player.OnAllPlayersCommand.Normal -= OnCommand;
                Bot.OnBotTargetPlayer.Normal -= OnBotTargetPlayer;
            }
        }

        #endregion

        public bool EndGame(bool ForceEnd = false)
        {
            if (!ZombieRoundEnabled) return false;
            if (!ForceEnd)
            {
                if (AmountSurvived() > 0 && AmountInfected() > 0 && Gamemode > 0)
                    return false;
                else if (AmountSurvived() > 0 && Gamemode == 0)
                    return false;
            }

            ZombieRoundEnabled = false;

            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "The round has ended!");
            foreach (Bot b in Server.Bots.ToArray())
            {
                if (b.Player.Level == ZombieLevel)
                {
                    b.Player.GlobalDie();
                    b.FollowPlayers = false;
                    b.BreakBlocks = false;
                    b.Jumping = false;
                    b.Unload();
                    Server.Bots.Remove(b);
                }
            }

            ExtraPlayerData[] lengths;

            int divide = ZombieHelper.Random.Next(2, 32);

            if (AmountSurvived() > 0)
                lengths = ExtraPlayerData.ToArray().OrderByDescending(x => (x.AmountMoved * 2) / divide).ToArray();
            else
                lengths = ExtraPlayerData.ToArray().OrderByDescending(x => (x.AmountInfected + x.AmountMoved) / divide).ToArray();

            try
            {
                if (AmountSurvived() > 0)
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.green + "Survivors win the round!");
                    foreach (ExtraPlayerData p in ExtraPlayerData.ToArray())
                    {
                        int AmountAwarded = 0;
                        try
                        {
                            AmountAwarded = ((Server.PlayerCount * ZombieHelper.Random.Next(7, 15)) / AmountSurvived()); //Moar players = moar money
                        }
                        catch { AmountAwarded = 3; } //Divide by zero can occur here
                        if (AmountAwarded > 35)
                            AmountAwarded = 35;
                        if (AmountAwarded <= 0)
                            AmountAwarded = 3;
                        if (Server.PlayerCount <= 1)
                            AmountAwarded = 3;
                        if (p.Survivor)
                            AmountAwarded = AmountAwarded * 2;
                        if (p.Referee && Server.PlayerCount > 1)
                            AmountAwarded = 1;
                        else if (p.Referee)
                            AmountAwarded = 0;

                        ZombieHelper.GiveMoney(p.Player, AmountAwarded);
                    }

                    int loop = 0;
                    foreach (ExtraPlayerData z in lengths)
                    {
                        loop++;
                        if (loop >= 6)
                            break;
                        else if (loop == 1)
                        {
                            if (z.Infected)
                                Player.UniversalChat("Most Valued Player: " + z.Player.Username + " - Score: " + Math.Abs((z.AmountInfected + z.AmountMoved) / divide));
                            else
                                Player.UniversalChat("Most Valued Player: " + z.Player.Username + " - Score: " + Math.Abs((z.AmountMoved * 2) / divide));
                        }
                        else
                        {
                            if (z.Infected)
                                Player.UniversalChat("#" + loop + ": " + z.Player.Username + " - Score: " + Math.Abs((z.AmountInfected + z.AmountMoved) / divide));
                            else
                                Player.UniversalChat("#" + loop + ": " + z.Player.Username + " - Score: " + Math.Abs((z.AmountMoved * 2) / divide));
                        }
                    }
                }
                else
                {
                    Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Infected win the round!");
                    foreach (ExtraPlayerData p in ExtraPlayerData.ToArray())
                    {
                        int AmountAwarded = 0;
                        try
                        {
                            AmountAwarded = ((Server.PlayerCount * ZombieHelper.Random.Next(4, 8)) / (Server.PlayerCount - (AmountInfected() / 2))); //Moar players = moar money
                        }
                        catch { AmountAwarded = 3; } //Divide by zero can occur here
                        if (AmountAwarded > 30)
                            AmountAwarded = 30;
                        if (AmountAwarded <= 0)
                            AmountAwarded = 3;
                        if (p.Referee && Server.PlayerCount > 1)
                            AmountAwarded = 1;
                        else if (p.Referee)
                            AmountAwarded = 0;
                        ZombieHelper.GiveMoney(p.Player, AmountAwarded);
                    }

                    int loop = 0;

                    foreach (ExtraPlayerData z in lengths)
                    {
                        loop++;
                        if (loop >= 6)
                            break;
                        else if (loop == 1)
                        {
                            if (z.Infected)
                                Player.UniversalChat("Most Valued Player: " + z.Player.Username + " - Score: " + Math.Abs((z.AmountInfected + z.AmountMoved) / divide));
                            else
                                Player.UniversalChat("Most Valued Player: " + z.Player.Username + " - Score: " + Math.Abs((z.AmountMoved * 2) / divide));
                        }
                        else
                        {
                            if (z.Infected)
                                Player.UniversalChat("#" + loop + ": " + z.Player.Username + " - Score: " + Math.Abs((z.AmountInfected + z.AmountMoved) / divide));
                            else
                                Player.UniversalChat("#" + loop + ": " + z.Player.Username + " - Score: " + Math.Abs((z.AmountMoved * 2) / divide));
                        }
                    }
                }
            }
            catch { }
            ZombieHelper.ResetPlayers(ExtraPlayerData);
            foreach (Player p in Server.Players.ToArray())
            {
                p.Save();
            }
            Voting = true;
            if (!GameModeQueued)
                Gamemode = ZombieHelper.GenerateGamemode();
            else
            {
                Gamemode = GameModeIntQueued;
                GameModeIntQueued = 0;
                GameModeQueued = false;
            }
            LevelChoices[0] = ZombieHelper.FindRandomLevel(new string[2] { Server.Mainlevel.Name, ZombieLevel.Name });
            LevelChoices[1] = ZombieHelper.FindRandomLevel(new string[3] { LevelChoices[0], Server.Mainlevel.Name, ZombieLevel.Name });
            if (LevelChoices[0] == null || LevelChoices[1] == null) //Security feature... this should NEVER happen
            {
                Player.UniversalChat("[Zombie Survival]: " + Colors.red + "SERVER RESTARTING IN 5 SECONDS - NO LEVELS ARE FOUND!");
                System.Threading.Thread.Sleep(5000);
                Server.Restart();
            }
            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Vote on the next level by typing the level name!");
            Player.UniversalChat("[Zombie Survival]: " + Colors.red + "Options: " + LevelChoices[0] + "/" + LevelChoices[1] + "/random");
            AmountOfMinutesElapsed = 0;
            AmountOfSecondsElapsed = -1;
            ZombieHelper.ClearDatabaseBlocks();
            return true;
        }

        #region Helper Functions
        public string GetMD5Hash(string input)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            var s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static Player PickRandomPlayer(List<Player> PlayerList)
        {
            var rnd = new Random();
            int r = rnd.Next(PlayerList.Count);
            return PlayerList[r];
        }

        public static ExtraPlayerData FindPlayer(string s)
        {
            try
            {
                return ExtraPlayerData.Find(e =>
                {
                    return e.Player.Username.ToLower() == s.ToLower();
                });
            }
            catch { }
            return null;
        }

        public static ExtraPlayerData FindPlayer(int id)
        {
            try
            {
                return ExtraPlayerData.Find(e =>
                {
                    return e.Player.ID == id;
                });
            }
            catch { }
            return null;
        }

        public static ExtraPlayerData FindPlayer(Player p)
        {
            try
            {
                return ExtraPlayerData.Find(e =>
                {
                    return e.Player == p;
                });
            }
            catch { }
            return null;
        }

        public bool IsNotImportant(string args)
        {
            if (args.StartsWith("#") ||
                args.StartsWith("*") ||
                args.StartsWith("!") ||
                args.StartsWith("+") ||
                args.StartsWith("@"))
                return false;
            else
                return true;
        }

        public static string Rand(string s)
        {
            return s;
        }

        public static int AmountInfected()
        {
            int loop = 0;
            foreach (ExtraPlayerData p in ExtraPlayerData.ToArray())
                if (p.Infected && !p.Referee)
                    loop++;
            return loop;
        }

        public static int AmountSurvived()
        {
            int loop = 0;
            foreach (ExtraPlayerData p in ExtraPlayerData.ToArray())
                if (!p.Infected && !p.Referee)
                    loop++;
            return loop;
        }
        #endregion
    }
}