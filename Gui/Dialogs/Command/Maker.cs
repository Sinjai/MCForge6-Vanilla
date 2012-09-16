/*
Copyright 2012 MCForge
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MCForge.Gui.Utils;
using System.Text.RegularExpressions;

namespace MCForge.Gui.Dialogs
{
    public partial class CommandMaker : Form
    {
        public CommandMaker()
        {
            InitializeComponent();
        }
        public static string ParseCode(string input)
        {
            /**
             * &1 = Dark Blue, System.
             * &3 = Lighter Blue, Struct/Class/Interface.
             * &8 = Gray, Normal Text.
             * &2 = Green, Comment.
             * Note: Don't mess with the replace order.
             */
            StringBuilder sb = new StringBuilder(input);
            // MCForge Structs/Interfaces
            sb.Replace("CommandTypes", "&3CommandTypes&8");
            sb.Replace("ICommand", "&3ICommand&8");
            sb.Replace("Player", "&3Player&8");
            sb.Replace("Command", "&3Command&8");
            // Comments
            sb.Replace("//", "&2//");
            // System
            sb.Replace("using", "&1using&8");
            sb.Replace("namespace", "&1namespace&8");
            sb.Replace("public", "&1public&8");
            sb.Replace("void", "&1void&8");
            sb.Replace("get", "&1get&8");
            sb.Replace("return", "&1return&8");
            // Variables
            sb.Replace("string", "&1string&8");
            sb.Replace("int", "&1int&8");
            sb.Replace("byte", "&1byte&8");
            // Silly conflcting color fixes.
            sb.Replace("class", "&1class&3");
            sb.Replace(":", "&8:"); // Fixes class.
            sb.Replace("{", "&8{"); // Fixes class.
            sb.Replace("}", "&8}"); // Fixes comments.
            sb.Replace(".", "&8."); // Fixes using.
            return sb.ToString();
        }

        private void CommandMaker_Load(object sender, EventArgs e)
        {
            makerText.DateStamp = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (CommandCreatorDialog dialog = new CommandCreatorDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    /* 
                     * Format: 0 = CommandName, 1 = CommandAuthor
                     * 2 = CommandType, 3 = CommandVersion, 4 = CommandPermission
                     */
                    string format = "using System;\n" +
                    "using MCForge.Interface.Command;\n" +
                    "   namespace {1} {{\n" +
                    "       public class Cmd{1} : ICommand {{\n" +
                    "           public string Name {{ get {{ return \"{0}\"; }} }} \n" +
                    "           public CommandTypes Type {{ get {{ return \"{2}\"; }} }} \n" +
                    "           public int Version {{ get {{ return {3}; }} }} \n" +
                    "           public string CUD {{ get {{ return \"Unimplemented\"; }} }} \n" +
                    "           public byte Permission {{ get {{ return (byte){4}; }} }} \n" +
                    "           public void Initialize() {{ Command.AddReference(this, \"{0}\", \"shortcut\"); }} // These are the aliases/shorcuts for your command as well as the only way a player can access them.\n" +
                    "           \n" +
                    "           public void Use (Player p, string[] args) {{ \n" +
                    "           //Your code here!\n" +
                    "           }}\n" +
                    "       }}\n" +
                    "   }}\n" +
                    "}}";

                    string a = String.Format(format, dialog.cmdName.Text, dialog.cmdAuthor.Text, dialog.cmdType.Text, dialog.cmdVersion.Text, dialog.cmdPermission.Text);
                    makerText.AppendLog(ParseCode(a));
                }
            }
        }

        private void btnNewFile_Click(object sender, EventArgs e)
        {
            makerText.Clear();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (makerText.CanUndo)
                makerText.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (makerText.CanRedo)
                makerText.Redo();
        }

        private void makerText_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
 
                // Will add all reserved words later.
        public Regex systemKeywords = new Regex("using|namespace|public|void|get|set|return|string|int|byte|");
        public Regex MCForgeKeywords = new Regex("CommandTypes|ICommand|Player|Command");
        public Regex markerKeywords = new Regex("class|struct|:");
        Color system = Color.FromArgb( 255, 0, 0, 161 );
        Color normal = Color.FromArgb( 255, 34, 34, 34 );
        Color highlight = Color.FromArgb( 255, 0, 161, 161 );
        Color comment = Color.FromArgb( 255, 0, 161, 0 );

        private void StringToRegexColor(){
        	int selPos = makerText.SelectionStart;
        	foreach(Match m in MCForgeKeywords.Matches(makerText.Text)){
        		makerText.Select(m.Index, m.Length);
                makerText.SelectionColor = highlight;
                makerText.SelectionStart = selPos;
                makerText.SelectionColor = normal;
        	}
        	foreach(Match m in systemKeywords.Matches(makerText.Text)){
        		makerText.Select(m.Index, m.Length);
                makerText.SelectionColor = system;
                makerText.SelectionStart = selPos;
                makerText.SelectionColor = normal;
        	}
        	foreach(Match m in markerKeywords.Matches(makerText.Text)){
        		makerText.Select(m.Index, m.Length);
        		makerText.SelectionColor = system;
                makerText.SelectionStart = selPos;
                makerText.SelectionColor = highlight;
        	}
        	foreach(Match m in new Regex("//").Matches(makerText.Text)){
        		makerText.Select(m.Index, m.Length);
                makerText.SelectionColor = comment;
        	}
        	
        }
        private void makerText_KeyPress(object sender, KeyPressEventArgs e)
        {
			if(e.KeyChar == (char)Keys.Space){
				StringToRegexColor();
		    }
		}
	}
}
