using System;
using MCForge;
using MCForge.Utils;
using MCForge.Entity;

namespace ZombiePlugin
{
    public class ExtraPlayerData
    {
        public const int SpamLimit = 20;

        public Player Player = null;
        public bool Infected = false;
        public bool Referee = false;
        public bool Survivor = false;
        public bool Aka = false;
        public bool SentUserType = false;
        public int TheTypeOfNotification = 0; //0 for none, 1 for bedrock, 2 for spleef
        public int AmountOfBlocksLeft = 0;
        public int WarnAmount = 0;
        public int AmountInfected = 0;
        public int AmountMoved = 0;

        //Anti-hack
        public int AmountOfTimesInAir = 0;
        public int AmountOfNoClips = 0;
        public int Spam = 0;
        public DateTime SpamTime;
        public int LastYValue = 0;
        public int AmountOfPillars = 0;
        public Vector3S LastLoc = new Vector3S(0, 0, 0);
        public TriBool Spawning = false;

        public long DateTimeAtStartOfRef;
        public ExtraPlayerData(Player p)
        {
            Player = p;
            SpamTime = DateTime.Now;
        }
    }
}