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
using System.IO;
using System.Linq;
using System.Xml;
using MCForge.Core;
using MCForge.Entity;
using MCForge.Interface.Command;
using MCForge.Utils;

namespace MCForge.Commands.Misc
{
    public class CmdResetPassword : ICommand
    {
        public string Name { get { return "ResetPassword"; } }
        public CommandTypes Type { get { return CommandTypes.Misc; } }
        public string Author { get { return "Sinjai"; } }
        public int Version { get { return 1; } }
        public string CUD { get { return ""; } }
        public byte Permission { get { return 0; } }
        public void Use(Player p, string[] args)
        {
            if (args[0] == "") { Help(p); return; }
            Player who = Player.Find(args[0]);
            if (p != null && !p.IsOwner) { p.SendMessage("Only the server owner can reset passwords!"); return; }
            //if (!File.Exists("extra/passwords/" + args[0] + ".xml")) { p.SendMessage("The player you specified does not have a password!"); return; }
            if (p != null && !p.IsVerified) { p.SendMessage("You cannot reset passwords until you have verified with &a/pass <password>" + Server.DefaultColor + "!"); return; }
            try
            {
                if (File.Exists("extra/passwords.xml"))
                {
                    var myXmlDocument = new XmlDocument();

                    myXmlDocument.Load("extra/passwords.xml");

                    foreach (XmlNode node in myXmlDocument.ChildNodes)
                    {
                        if (!node.HasChildNodes) continue;
                        if (node.ChildNodes.Cast<XmlNode>().Count(node2 => node2.Name.ToLower() == args[0].ToLower()) == 0)
                        {
                            p.SendMessage("The player you specified does not have a password!"); return;
                        }
                        foreach (var node2 in
                            node.ChildNodes.Cast<XmlNode>().Where(
                                node2 => node2.Name.ToLower() == args[0].ToLower()))
                        {
                            
                                node.RemoveAll(); //make sure there is no duplicates
                        }
                    }
                    myXmlDocument.Save("extra/passwords.xml");
                }
                p.SendMessage("&3" + args[0] + Server.DefaultColor + "'s password has been successfully reset.");
            }
            catch (Exception e)
            {
                p.SendMessage("Password deletion failed. Please manually delete the file, extra/passwords/" + args[0] + ".xml, to reset " + args[0] + "'s password.");
                Logger.LogError(e);
            }
        }
        public void Help(Player p)
        {
            p.SendMessage("/resetpassword <player> - Reset <player>'s password. Can only be used by the server owner.");
            p.SendMessage("Shortcut: /resetpass");
        }
        public void Initialize()
        {
            Command.AddReference(this, new string[] { "resetpass", "resetpassword" });
        }
    }
}
