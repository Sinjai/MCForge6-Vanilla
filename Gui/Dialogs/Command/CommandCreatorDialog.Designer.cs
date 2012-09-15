namespace MCForge.Gui.Dialogs
{
    partial class CommandCreatorDialog
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
            this.aeroButton1 = new MCForge.Gui.Components.AeroButton(this.components);
            this.cmdName = new MCForge.Gui.Components.ColoredTextBox();
            this.cmdType = new System.Windows.Forms.ComboBox();
            this.cmdPermission = new System.Windows.Forms.ComboBox();
            this.cmdAuthor = new MCForge.Gui.Components.ColoredTextBox();
            this.cmdVersion = new MCForge.Gui.Components.ColoredTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // aeroButton1
            // 
            this.aeroButton1.Location = new System.Drawing.Point(231, 59);
            this.aeroButton1.Name = "aeroButton1";
            this.aeroButton1.Size = new System.Drawing.Size(34, 23);
            this.aeroButton1.TabIndex = 0;
            this.aeroButton1.Text = "Ok";
            this.aeroButton1.UseVisualStyleBackColor = true;
            this.aeroButton1.Click += new System.EventHandler(this.aeroButton1_Click);
            // 
            // cmdName
            // 
            this.cmdName.Location = new System.Drawing.Point(48, 6);
            this.cmdName.Name = "cmdName";
            this.cmdName.Size = new System.Drawing.Size(83, 20);
            this.cmdName.TabIndex = 1;
            this.cmdName.Text = "";
            // 
            // cmdType
            // 
            this.cmdType.FormattingEnabled = true;
            this.cmdType.Location = new System.Drawing.Point(48, 30);
            this.cmdType.Name = "cmdType";
            this.cmdType.Size = new System.Drawing.Size(83, 21);
            this.cmdType.TabIndex = 10;
            // 
            // cmdPermission
            // 
            this.cmdPermission.FormattingEnabled = true;
            this.cmdPermission.Location = new System.Drawing.Point(203, 30);
            this.cmdPermission.Name = "cmdPermission";
            this.cmdPermission.Size = new System.Drawing.Size(62, 21);
            this.cmdPermission.TabIndex = 11;
            // 
            // cmdAuthor
            // 
            this.cmdAuthor.Location = new System.Drawing.Point(182, 2);
            this.cmdAuthor.Name = "cmdAuthor";
            this.cmdAuthor.Size = new System.Drawing.Size(83, 20);
            this.cmdAuthor.TabIndex = 12;
            this.cmdAuthor.Text = "";
            // 
            // cmdVersion
            // 
            this.cmdVersion.Location = new System.Drawing.Point(50, 56);
            this.cmdVersion.Name = "cmdVersion";
            this.cmdVersion.Size = new System.Drawing.Size(26, 20);
            this.cmdVersion.TabIndex = 13;
            this.cmdVersion.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Version:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(137, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Author:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(137, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Permission:";
            // 
            // CommandCreatorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 87);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdVersion);
            this.Controls.Add(this.cmdAuthor);
            this.Controls.Add(this.cmdPermission);
            this.Controls.Add(this.cmdType);
            this.Controls.Add(this.cmdName);
            this.Controls.Add(this.aeroButton1);
            this.Name = "CommandCreatorDialog";
            this.Text = "CommandCreatorDialog";
            this.Load += new System.EventHandler(this.CommandCreatorDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Components.ColoredTextBox cmdName;
        public System.Windows.Forms.ComboBox cmdType;
        public System.Windows.Forms.ComboBox cmdPermission;
        public Components.ColoredTextBox cmdVersion;
        public Components.ColoredTextBox cmdAuthor;

        private Components.AeroButton aeroButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;

    }
}