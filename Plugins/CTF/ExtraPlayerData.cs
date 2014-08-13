using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Utils;

namespace CTF
{
    public class ExtraPlayerData
    {
        public Player player;
        public bool referee = false;
        public bool voted = false;
        public int points = 0;
        public EXPLevel explevel;
        public static ExtraPlayerData extraplayerdata = new ExtraPlayerData();
        public static explicit operator ExtraPlayerData(Player p)
        {
            return (ExtraPlayerData)extraplayerdata.player;
        }
        //CTF
        public int tntSeconds = 0;
        public int lazorSeconds = 0;
        public int lightSeconds = 0;
        public int pteam = 0;
        public bool isHoldingFlag = false;
        public bool killingPeople = false;
        public int amountKilled = 0;
        public int overallKilled = 0;
        public int overallDied = 0;
        public Vector3S minePlacement;
        public int minesPlaced = 0;
        public Vector3S trapPlacement;
        public int trapsPlaced = 0;
        public Vector3S tntPlacement;
        public int tntPlaced = 0;
        //public static System.Timers.Timer thetimer3;
        public System.Timers.Timer deathTimer;
        public static System.Timers.Timer lazerTimer;
        public System.Timers.Timer freezeTimer;
        public Vector3S lazerPos;
        public bool shotSecondLazer = false;
        public bool deathTimerOn = false;
        public bool hasBeenTrapped = false;
        public bool autoTNT = true;
        public bool ironman = false;
        public bool teamchat = false;
        public bool PlacedNukeThisRound = false;
        public bool BoughtOneUpThisRound = false;
        public Vector3S tripwire1;
        public Vector3S tripwire2;
        public int tripwiresPlaced = 0;
        public int tags = 0;
        public int games = 0;
        public int losses = 0;
        public int wins = 0;

        public void resetDeathTimer(object sender, ElapsedEventArgs e)
        {
            deathTimerOn = false;
            deathTimer.Dispose();
            deathTimer.Enabled = false;
            deathTimer.Stop();
        }

        //items and upgrades
        public int lazers = 0;
        public int lightnings = 0;
        public int traps = 0;
        public int lines = 0;
        public int rockets = 0;
        public int grapple = 0;
        public int bigtnt = 0;
        public int nuke = 0;
        public int jetpack = 0;
        public int tripwire = 0;
        public int knife = 0;
        public int freeze = 0;

        public int lazerUpgrade = 0;
        public int lightningUpgrade = 0;
        public int trapUpgrade = 0;
        public int rocketUpgrade = 0;
        public int tntUpgrade = 0;
        public int pistolUpgrade = 0;
        public int mineUpgrade = 0;
        public int tripwireUpgrade = 0;
        public int knifeUpgrade = 0;

        //buffs
        public bool untouchable = false;
        public bool iceshield = false;
        public bool invinciblee = false;
        public bool makeaura = false;
        public bool clearview = false;
        public bool oneup = false;

    }
}
