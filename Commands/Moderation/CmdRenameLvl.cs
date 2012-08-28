/*
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
using System;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.World;

namespace MCForge.Commands.Moderation
{
    public class CmdRenameLvl : ICommand
    {
        public string Name
        {
            get { return "RenameLevel"; }
        }

        public CommandTypes Type
        {
            get { return CommandTypes.Mod; }
        }

        public string Author
        {
            get { return "cazzar"; }
        }

        public int Version
        {
            get { return 1; }
        }

        public string CUD
        {
            get { return String.Empty; }
        }

        public byte Permission
        {
            get { return 80; }
        }

        public void Use(Player p, string[] args)
        {
            Level l;
            string newName;
            switch (args.Length)
            {
                case 1:
                    l = p.Level;
                    newName = args[0];
                    break;
                case 2:
                    l = Level.FindLevel(args[0]);
                    newName = args[1];
                    break;
                default:
                    Help(p);
                    return;
            }

            if (l == null)
            {
                p.SendMessage("Cannot find the level!");
                return;
            }
            if (l == Server.Mainlevel)
            {
                p.SendMessage("You cannot rename the server's main level!");
                return;
            }
            try
            {
                l.Rename(newName);
            }
            catch (Exception)
            {
                p.SendMessage("There was an error renaming the level");
#if DEBUG
                throw;
#endif
            }
        }

        public void Help(Player p)
        {
            p.SendMessage("/renamelevel <level> [newname] - renames <level> to newname");
            p.SendMessage("if <level> is undefined your current level is selected.");
            p.SendMessage("/renamelvl can also be used.");
        }

        public void Initialize()
        {
            Command.AddReference(this, new[] {"renamelevel", "renamelvl"});
        }
    }
}
