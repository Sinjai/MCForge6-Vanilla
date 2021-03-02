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

            this.webBrowser1.Url = new Uri(String.Format("{0}/settings.html?xid={1}", Server.RC.URL, Server.RC.XID));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Refresh();
        }
    }
}
