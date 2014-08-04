using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using MCForge.Utils;
using MCForge.Gui.Utils;

namespace MCForge.Gui.Dialogs {
    public partial class NewsDialog : Form {

        public const string URL = "http://mcforge.org/changelog";
        private BackgroundWorker mNewsFetcher;

        public NewsDialog() {
            InitializeComponent();

            mNewsFetcher = new BackgroundWorker() {
                WorkerSupportsCancellation = true
            };
            mNewsFetcher.DoWork += new DoWorkEventHandler(mNewsFetcher_DoWork);
            mNewsFetcher.RunWorkerCompleted += new RunWorkerCompletedEventHandler(mNewsFetcher_RunWorkerCompleted);
        }

        void mNewsFetcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if ( e.Cancelled || mNewsFetcher.CancellationPending )
                return;

            if ( !htmlReader1.IsDisposed )
                htmlReader1.WriteHtml(InetUtils.GrabWebpage(URL));
        }

        void mNewsFetcher_DoWork(object sender, DoWorkEventArgs e) {
            if ( PageExists() ) {
                e.Result = "Cannot find news :(";
                return;
            }
        }



        private void NewsDialog_Load(object sender, EventArgs e) {
            mNewsFetcher.RunWorkerAsync();

            checkBox1.Checked = !GuiSettings.GetSettingBoolean(GuiSettings.SHOW_NEWS_KEY);
        }

        bool PageExists() {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = WebRequestMethods.Http.Head;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            GuiSettings.SetSetting(GuiSettings.SHOW_NEWS_KEY, null, (!checkBox1.Checked).ToString());
            GuiSettings.Save();
            Close();
        }
    }
}
