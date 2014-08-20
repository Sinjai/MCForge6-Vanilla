/*
    Copyright 2014 UclCommander
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
using System.Collections.Generic;
using System.Text;

namespace MCForge.Core.RelayChat.Core
{
    public class IrcHelper
    {
        public const String HEADER_CHAR = "\u0001";

        /// <summary>
        /// Converts the IRC line to something useable.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<object> Parse(string cmd, Connection connection)
        {
            char[] data = cmd.ToCharArray();
            List<string> parsed = new List<string> { };
            List<string> userinfo = new List<string> { };
            string buffer = "";
            bool inString = false;
            bool userStr = false;

            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0 && data[i] == ':')
                {
                    userStr = true;
                    continue;
                }

                if ((data[i] == '!' || data[i] == '@' || data[i] == ' ') && userStr)
                {
                    userinfo.Add(buffer);
                    buffer = "";

                    if (data[i] == ' ')
                        userStr = false;

                    continue;
                }

                if (data[i] == ':' && !inString)
                {
                    inString = true;
                    continue;
                }

                if (data[i] == ' ' && !inString)
                {
                    parsed.Add(buffer);
                    buffer = "";
                    continue;
                }

                buffer += data[i];
            }

            if (buffer != null)
                parsed.Add(buffer);

            List<object> ret = new List<object> { };
            parsed.RemoveAll(item => String.IsNullOrEmpty(item));

            ret.Add(parsed);

            if (userinfo.Count == 3)
                ret.Add(new User(userinfo[0], userinfo[1], userinfo[2], connection));
            else
                ret.Add(new User());

            return ret;
        }

        /// <summary>
        /// Converts Classic to IRC
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ClassicToIRC(string message)
        {
            if (message == null) throw new ArgumentNullException("message");
            StringBuilder sb = new StringBuilder(message);
            ClassicToIRC(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Converts Classic to IRC
        /// </summary>
        /// <param name="sb"></param>
        public static void ClassicToIRC(StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException("sb");
            sb.Replace("&0", '\x03' + "1");
            sb.Replace("&1", '\x03' + "2");
            sb.Replace("&2", '\x03' + "3");
            sb.Replace("&3", '\x03' + "10");
            sb.Replace("&4", '\x03' + "4");
            sb.Replace("&5", '\x03' + "6");
            sb.Replace("&6", '\x03' + "8");
            sb.Replace("&7", '\x03' + "15");
            sb.Replace("&8", '\x03' + "14");
            sb.Replace("&9", '\x03' + "12");
            sb.Replace("&a", '\x03' + "9");
            sb.Replace("&b", '\x03' + "11");
            sb.Replace("&c", '\x03' + "4");
            sb.Replace("&d", '\x03' + "13");
            sb.Replace("&e", '\x03' + "7");
            sb.Replace("&f", '\x03' + "0");
            sb.Replace("&A", '\x03' + "9");
            sb.Replace("&B", '\x03' + "11");
            sb.Replace("&C", '\x03' + "4");
            sb.Replace("&D", '\x03' + "13");
            sb.Replace("&E", '\x03' + "7");
            sb.Replace("&F", '\x03' + "0");
        }

        /// <summary>
        /// Converts IRC to classic
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string IRCToClassic(string message)
        {
            if (message == null) throw new ArgumentNullException("message");
            StringBuilder sb = new StringBuilder(message);
            IRCToClassic(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Converts IRC to classic
        /// </summary>
        /// <param name="sb"></param>
        public static void IRCToClassic(StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException("sb");
            sb.Replace('\x03' + "1", "&0");
            sb.Replace('\x03' + "2", "&1");
            sb.Replace('\x03' + "3", "&2");
            sb.Replace('\x03' + "10", "&3");
            sb.Replace('\x03' + "4", "&4");
            sb.Replace('\x03' + "6", "&5");
            sb.Replace('\x03' + "8", "&6");
            sb.Replace('\x03' + "15", "&7");
            sb.Replace('\x03' + "14", "&8");
            sb.Replace('\x03' + "12", "&9");
            sb.Replace('\x03' + "9", "&a");
            sb.Replace('\x03' + "11", "&b");
            sb.Replace('\x03' + "4", "&c");
            sb.Replace('\x03' + "13", "&d");
            sb.Replace('\x03' + "7", "&e");
            sb.Replace('\x03' + "0", "&f");
            sb.Replace('\x03' + "9", "&A");
            sb.Replace('\x03' + "11", "&B");
            sb.Replace('\x03' + "4", "&C");
            sb.Replace('\x03' + "13", "&D");
            sb.Replace('\x03' + "7", "&E");
            sb.Replace('\x03' + "0", "&F");
        }
    }
}
