namespace MCForge.Gui.Dialogs {
    partial class NewsDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.htmlReader1 = new MCForge.Gui.Components.HtmlReader();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(330, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 352);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(163, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Don\'t show news on startup?";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // htmlReader1
            // 
            this.htmlReader1.AllowNavigation = false;
            this.htmlReader1.AllowWebBrowserDrop = false;
            this.htmlReader1.IsWebBrowserContextMenuEnabled = false;
            this.htmlReader1.Location = new System.Drawing.Point(12, 12);
            this.htmlReader1.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlReader1.Name = "htmlReader1";
            this.htmlReader1.ScriptErrorsSuppressed = true;
            this.htmlReader1.ScrollBarsEnabled = false;
            this.htmlReader1.Size = new System.Drawing.Size(393, 325);
            this.htmlReader1.TabIndex = 0;
            this.htmlReader1.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // NewsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 383);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.htmlReader1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "NewsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NewsDialog";
            this.Load += new System.EventHandler(this.NewsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Components.HtmlReader htmlReader1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;

    }
}