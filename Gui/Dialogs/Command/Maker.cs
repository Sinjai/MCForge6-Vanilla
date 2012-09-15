using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCForge.Gui.Dialogs
{
    public partial class CommandMaker : Form
    {
        public CommandMaker()
        {
            InitializeComponent();
        }

        private void aeroButton1_Click(object sender, EventArgs e)
        {
            using (CommandCreatorDialog dialog = new CommandCreatorDialog()){
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 0 = CommandName
                    // 1 = CommandAuthor
                    // 2 = CommandType
                    // 3 = CommandVersion
                    // 4 = CommandPermission
                    //(byte)Groups.PermissionLevel.Guest
                    string format = "using System;\n" +
                    "using MCForge.Interface.Command;\n" +
                    "   namespace Cmd{1} {{" +
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
        public static string ParseCode(string input)
        {
            // 1 = Dark Blue
            // 3 = Struct/Class Lighter Blue
            // 8 = Gray
            // &2 = Green Comment
            StringBuilder sb = new StringBuilder(input);
            //Comments
            sb.Replace("//", "&2//");

            // MCForge Structs
            sb.Replace("CommandTypes", "&3CommandTypes&8");
            sb.Replace("Player", "&3Player&8");
            sb.Replace("Command", "&3Command&8");

            // System
            sb.Replace("using", "&1using&8");
            sb.Replace("namespace", "&1namespace&8");
            sb.Replace("public", "&1public&8");
            sb.Replace("class", "&1class&3");
            sb.Replace("void", "&1void&3");

            // Variables
            sb.Replace("string", "&1string&8");
            sb.Replace("int", "&1int&8");
            sb.Replace("byte", "&1byte&8");
            // Label
            sb.Replace("get", "&1get&8");
            sb.Replace("return", "&1return&8");
            return sb.ToString();
        }
        private void makerText_TextChanged(object sender, EventArgs e)
        {

        }

        private void CommandMaker_Load(object sender, EventArgs e)
        {
            makerText.DateStamp = false;
        }
    }
}
