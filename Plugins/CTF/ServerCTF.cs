using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCForge.Entity;

namespace CTF
{
    public class ServerCTF
    {
        public static CTFPlugin ctf;
        public static int gameStatus = 0;
        public static bool CTFModeOn = false;
        public static bool ctfRound = false;
        public static int ctfGameStatus = 0; //0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop game next round
        public static bool CTFOnlyServer = false;
        public static List<ExtraPlayerData> killed = new List<ExtraPlayerData>();
        public static string currentLevel = "";
        public static bool blueFlagDropped = false;
        public static bool redFlagDropped = false;
        public static bool blueFlagHeld = false;
        public static bool redFlagHeld = false;
        public static System.Timers.Timer redFlagTimer;
        public static System.Timers.Timer blueFlagTimer;
        public static int vulnerable = 1;
        public static string nextLevel = "";
        public static bool voting = false;
        public static bool votingforlevel = false;
        public static int Level1Vote = 0;
        public static int Level2Vote = 0;
        public static int Level3Vote = 0;
        public static bool ChangeLevels = true;
        public static bool UseLevelList = false;
        public static List<String> LevelList = new List<String>();
        public static string lastLevelVote1 = "";
        public static string lastLevelVote2 = "";
    }
}
