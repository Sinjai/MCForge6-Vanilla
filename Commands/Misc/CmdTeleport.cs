﻿/*
Copyright 2011 MCForge
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
http://www.opensource.org/licenses/ecl2.php
http://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
*/
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils.Settings;
using MCForge.Utils;
using System.Threading;

namespace MCForge.Commands
{
    public class CmdTeleport : ICommand
    {
        public string Name { get { return "Teleport"; } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "Gamemakergm"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 80; } }


        public void Use(Player p, string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Vector3S meep = new Vector3S((short)(p.Level.CWMap.SpawnPos.x * 32 + 16), (short)(p.Level.CWMap.SpawnPos.z * 32 + 16), (short)(p.Level.CWMap.SpawnPos.y * 32));
                    p.SendToPos(meep, new byte[2] { (byte)p.Rot.x, (byte)p.Rot.z });
                    break;
                case 1:
                    Player who = Player.Find(args[0]);
                    if (who == null || who.IsHidden)
                    {
                        p.SendMessage("Player: " + args[0] + " not found!");
                        return;
                    }
                    else if (who == p)
                    {
                        p.SendMessage("Why are you trying to teleport yourself to yourself?");
                        return;
                    }
                    else if (!ServerSettings.GetSettingBoolean("AllowHigherRankTp") && p.Group.Permission < who.Group.Permission)
                    {
                        p.SendMessage("You cannot teleport to a player of higher rank!");
                        return;
                    }
                    else
                    {
                        if (p.Level != who.Level)
                        {
                            //Need goto here
                            if (who.IsLoading)
                            {
                                p.SendMessage("Waiting for " + who.Color + who.Username + Server.DefaultColor + " to spawn...");
                                while (who.IsLoading)
                                    Thread.Sleep(5);

                            }
                        }
                    }
                    p.SendToPos(who.Pos, new byte[2] { (byte)who.Rot.x, (byte)who.Rot.z });
                    break;
                case 2:
                    Player one = Player.Find(args[0]);
                    Player two = Player.Find(args[1]);
                    if (one == null || two == null)
                    {
                        //Hehe
                        p.SendMessage((one == null && two == null) ? "Players: " + args[0] + " and " + args[1] + " not found!" : "Player: " + ((one == null) ? args[0] : args[1]) + " not found!");
                        return;
                    }
                    else if (one == p && two == p || one == p)
                    {
                        p.SendMessage((two == p) ? "Why are you trying to teleport yourself to yourself?" : "Why not just use /tp " + args[1] + "?");
                        return;
                    }
                    else if (two == p)
                    {
                        p.SendMessage("Why not just use /summon " + args[0] + "?");
                        return;
                    }
                    else if (p.Group.Permission < one.Group.Permission)
                    {
                        p.SendMessage("You cannot force a player of higher rank to tp to another player!");
                    }
                    else
                    {
                        if (one.Level != two.Level)
                        {
                            //Need goto here
                            if (two.IsLoading)
                            {
                                p.SendMessage("Waiting for " + two.Color + two.Username + Server.DefaultColor + " to spawn...");
                                while (two.IsLoading)
                                {
                                    Thread.Sleep(5);
                                }
                            }
                        }
                    }
                    one.SendToPos(two.Pos);
                    p.SendMessage(one.Username + " has been succesfully teleported to " + two.Username + "!");
                    break;
                case 3: {
                        short x, z, y;
                        bool[] intParse = { short.TryParse(args[0], out x), short.TryParse(args[1], out z), short.TryParse(args[2], out y) };
                        if (intParse.Contains<bool>(false)) {
                            p.SendMessage("One of your coordinates was har.");
                            return;
                        }
                        else {
                            p.SendToPos(new Vector3S(x, z, y), new byte[2] { (byte)p.Rot.x, (byte)p.Rot.z });
                            p.SendMessage(string.Format("Succesfully teleported to {0}, {1}, {2}!", x, z, y));
                        }
                        break;
                    }
                case 4: {
                        short x, z, y;
                        bool[] intParse = { short.TryParse(args[0], out x), short.TryParse(args[1], out z), short.TryParse(args[2], out y) };
                        if (intParse.Contains<bool>(false)) {
                            p.SendMessage("One of your coordinates was har.");
                            return;
                        }
                        else {
                            ICommand g = Command.Find("goto");
                            if (g == null) { p.SendMessage("You can't teleport to another level"); return; }
                            g.Use(p, new string[] { args[3] });
                            p.SendToPos(new Vector3S(x, z, y), new byte[2] { (byte)p.Rot.x, (byte)p.Rot.z });
                            p.SendMessage(string.Format("Succesfully teleported to {0}, {1}, {2}!", x, z, y));
                        }
                        break;
                    }
                default:
                    p.SendMessage("Invalid arguments!");
                    break;
            }
        }

        public void Help(Player p)
        {
            p.SendMessage("/teleport <x> <z> <y> - Telports you to a coordinate.");
            p.SendMessage("/teleport <player1> [player2] - Teleports yourself to a player.");
            p.SendMessage("[player2] is optional, but if present will send player1 to the player2.");
            p.SendMessage("If <player> is blank, you are sent to spawn");
            p.SendMessage("Shortcut: /tp");
        }

        public void Initialize()
        {
            Command.AddReference(this, "tp", "teleport");
        }
    }
}
