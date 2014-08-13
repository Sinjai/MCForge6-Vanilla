using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Core;

namespace MCForge.Gui.Dialogs
{
    public partial class ServerSettingsDialog : Form
    {
        public ServerSettingsDialog()
        {
            InitializeComponent();

            this.webBrowser1.Url = new Uri("http://localhost:6969/settings.html?xid=" + Server.RC.XID);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Refresh();
        }
    }
}
