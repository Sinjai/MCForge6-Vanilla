using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MCForge.Interface.Command;
using MCForge.Groups;

namespace MCForge.Gui.Dialogs
{
    public partial class CommandCreatorDialog : Form
    {
        public CommandCreatorDialog()
        {
            InitializeComponent();
        }

        private void CommandCreatorDialog_Load(object sender, EventArgs e)
        {
            string[] types = Enum.GetNames(typeof(CommandTypes));
            foreach (string t in types)
            {
                cmdType.Items.Add(t);
            }
            types = Enum.GetNames(typeof(Groups.PermissionLevel));
            foreach (string t in types)
            {
                //cmdPermission.AddIfNotExist(Char.Parse(g.Color.Substring(1)), g.Name);
                cmdPermission.Items.Add(t);
            }
        }

        private void aeroButton1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        
    }
}
