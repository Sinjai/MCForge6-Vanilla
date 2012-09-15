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
