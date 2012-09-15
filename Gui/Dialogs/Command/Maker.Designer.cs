namespace MCForge.Gui.Dialogs
{
    partial class CommandMaker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.makerText = new MCForge.Gui.Components.ColoredTextBox();
            this.aeroButton1 = new MCForge.Gui.Components.AeroButton(this.components);
            this.SuspendLayout();
            // 
            // makerText
            // 
            this.makerText.Location = new System.Drawing.Point(12, 12);
            this.makerText.Name = "makerText";
            this.makerText.Size = new System.Drawing.Size(460, 384);
            this.makerText.TabIndex = 0;
            this.makerText.Text = "";
            // 
            // aeroButton1
            // 
            this.aeroButton1.Location = new System.Drawing.Point(12, 402);
            this.aeroButton1.Name = "aeroButton1";
            this.aeroButton1.Size = new System.Drawing.Size(104, 23);
            this.aeroButton1.TabIndex = 1;
            this.aeroButton1.Text = "CreateCommand";
            this.aeroButton1.UseVisualStyleBackColor = true;
            this.aeroButton1.Click += new System.EventHandler(this.aeroButton1_Click);
            // 
            // CommandMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 435);
            this.Controls.Add(this.aeroButton1);
            this.Controls.Add(this.makerText);
            this.Name = "CommandMaker";
            this.Text = "Code Maker by Gamemakergm";
            this.Load += new System.EventHandler(this.CommandMaker_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.ColoredTextBox makerText;
        private Components.AeroButton aeroButton1;

    }
}