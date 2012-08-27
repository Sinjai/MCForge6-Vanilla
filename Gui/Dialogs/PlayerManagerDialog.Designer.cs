namespace MCForge.Gui.Dialogs {
    partial class PlayerManagerDialog {
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
            this.components = new System.ComponentModel.Container();
            this.grpPlayerList = new System.Windows.Forms.GroupBox();
            this.lstPlayers = new MCForge.Gui.Components.ColoredListBox();
            this.grpInfo = new System.Windows.Forms.GroupBox();
            this.btnTitleColor = new MCForge.Gui.Components.ColorSelectionButton(this.components);
            this.txtUndo = new MCForge.Gui.Components.HintedTextbox();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnBan = new System.Windows.Forms.Button();
            this.btnKick = new System.Windows.Forms.Button();
            this.btnEditRank = new System.Windows.Forms.Button();
            this.txtRank = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnEditMap = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMap = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtChat = new MCForge.Gui.Components.HintedTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnColor = new MCForge.Gui.Components.ColorSelectionButton(this.components);
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpPlayerList.SuspendLayout();
            this.grpInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPlayerList
            // 
            this.grpPlayerList.Controls.Add(this.lstPlayers);
            this.grpPlayerList.Location = new System.Drawing.Point(13, 13);
            this.grpPlayerList.Name = "grpPlayerList";
            this.grpPlayerList.Size = new System.Drawing.Size(243, 512);
            this.grpPlayerList.TabIndex = 0;
            this.grpPlayerList.TabStop = false;
            this.grpPlayerList.Text = "Players";
            // 
            // lstPlayers
            // 
            this.lstPlayers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstPlayers.FormattingEnabled = true;
            this.lstPlayers.Location = new System.Drawing.Point(7, 20);
            this.lstPlayers.Name = "lstPlayers";
            this.lstPlayers.Size = new System.Drawing.Size(230, 485);
            this.lstPlayers.TabIndex = 0;
            this.lstPlayers.SelectedIndexChanged += new System.EventHandler(this.lstPlayers_SelectedIndexChanged);
            // 
            // grpInfo
            // 
            this.grpInfo.Controls.Add(this.btnTitleColor);
            this.grpInfo.Controls.Add(this.txtUndo);
            this.grpInfo.Controls.Add(this.btnUndo);
            this.grpInfo.Controls.Add(this.btnBan);
            this.grpInfo.Controls.Add(this.btnKick);
            this.grpInfo.Controls.Add(this.btnEditRank);
            this.grpInfo.Controls.Add(this.txtRank);
            this.grpInfo.Controls.Add(this.label8);
            this.grpInfo.Controls.Add(this.btnEditMap);
            this.grpInfo.Controls.Add(this.txtTitle);
            this.grpInfo.Controls.Add(this.label7);
            this.grpInfo.Controls.Add(this.txtMap);
            this.grpInfo.Controls.Add(this.label6);
            this.grpInfo.Controls.Add(this.txtIp);
            this.grpInfo.Controls.Add(this.label5);
            this.grpInfo.Controls.Add(this.txtName);
            this.grpInfo.Controls.Add(this.label4);
            this.grpInfo.Controls.Add(this.label3);
            this.grpInfo.Controls.Add(this.txtChat);
            this.grpInfo.Controls.Add(this.label2);
            this.grpInfo.Controls.Add(this.btnColor);
            this.grpInfo.Controls.Add(this.txtStatus);
            this.grpInfo.Controls.Add(this.label1);
            this.grpInfo.Enabled = false;
            this.grpInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpInfo.Location = new System.Drawing.Point(263, 13);
            this.grpInfo.Name = "grpInfo";
            this.grpInfo.Size = new System.Drawing.Size(325, 512);
            this.grpInfo.TabIndex = 1;
            this.grpInfo.TabStop = false;
            this.grpInfo.Text = "Info";
            // 
            // btnTitleColor
            // 
            this.btnTitleColor.Enabled = false;
            this.btnTitleColor.Location = new System.Drawing.Point(220, 188);
            this.btnTitleColor.Name = "btnTitleColor";
            this.btnTitleColor.Relation = null;
            this.btnTitleColor.Size = new System.Drawing.Size(97, 23);
            this.btnTitleColor.TabIndex = 49;
            this.btnTitleColor.Text = "colorSelectionButton1";
            this.btnTitleColor.UseVisualStyleBackColor = true;
            // 
            // txtUndo
            // 
            this.txtUndo.Enabled = false;
            this.txtUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUndo.ForeColor = System.Drawing.Color.Gray;
            this.txtUndo.Hint = "Undo Amount";
            this.txtUndo.HintColor = System.Drawing.Color.Gray;
            this.txtUndo.Location = new System.Drawing.Point(137, 394);
            this.txtUndo.Name = "txtUndo";
            this.txtUndo.Size = new System.Drawing.Size(107, 20);
            this.txtUndo.TabIndex = 48;
            this.txtUndo.Text = "Undo Amount";
            this.txtUndo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnUndo
            // 
            this.btnUndo.Enabled = false;
            this.btnUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUndo.Location = new System.Drawing.Point(250, 392);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 47;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            // 
            // btnBan
            // 
            this.btnBan.Enabled = false;
            this.btnBan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBan.Location = new System.Drawing.Point(64, 392);
            this.btnBan.Name = "btnBan";
            this.btnBan.Size = new System.Drawing.Size(49, 23);
            this.btnBan.TabIndex = 46;
            this.btnBan.Text = "Ban";
            this.btnBan.UseVisualStyleBackColor = true;
            // 
            // btnKick
            // 
            this.btnKick.Enabled = false;
            this.btnKick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKick.Location = new System.Drawing.Point(9, 392);
            this.btnKick.Name = "btnKick";
            this.btnKick.Size = new System.Drawing.Size(49, 23);
            this.btnKick.TabIndex = 45;
            this.btnKick.Text = "Kick";
            this.btnKick.UseVisualStyleBackColor = true;
            // 
            // btnEditRank
            // 
            this.btnEditRank.Enabled = false;
            this.btnEditRank.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditRank.Location = new System.Drawing.Point(220, 131);
            this.btnEditRank.Name = "btnEditRank";
            this.btnEditRank.Size = new System.Drawing.Size(99, 23);
            this.btnEditRank.TabIndex = 44;
            this.btnEditRank.Text = "Edit Rank";
            this.btnEditRank.UseVisualStyleBackColor = true;
            // 
            // txtRank
            // 
            this.txtRank.Enabled = false;
            this.txtRank.Location = new System.Drawing.Point(52, 134);
            this.txtRank.Name = "txtRank";
            this.txtRank.ReadOnly = true;
            this.txtRank.Size = new System.Drawing.Size(162, 20);
            this.txtRank.TabIndex = 43;
            this.txtRank.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 137);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 42;
            this.label8.Text = "Rank:";
            // 
            // btnEditMap
            // 
            this.btnEditMap.Enabled = false;
            this.btnEditMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditMap.Location = new System.Drawing.Point(220, 105);
            this.btnEditMap.Name = "btnEditMap";
            this.btnEditMap.Size = new System.Drawing.Size(99, 23);
            this.btnEditMap.TabIndex = 41;
            this.btnEditMap.Text = "Edit Map";
            this.btnEditMap.UseVisualStyleBackColor = true;
            // 
            // txtTitle
            // 
            this.txtTitle.Enabled = false;
            this.txtTitle.Location = new System.Drawing.Point(55, 188);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(90, 20);
            this.txtTitle.TabIndex = 40;
            this.txtTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Enabled = false;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 39;
            this.label7.Text = "Title: ";
            // 
            // txtMap
            // 
            this.txtMap.Enabled = false;
            this.txtMap.Location = new System.Drawing.Point(53, 107);
            this.txtMap.Name = "txtMap";
            this.txtMap.ReadOnly = true;
            this.txtMap.Size = new System.Drawing.Size(161, 20);
            this.txtMap.TabIndex = 38;
            this.txtMap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Map: ";
            // 
            // txtIp
            // 
            this.txtIp.Enabled = false;
            this.txtIp.Location = new System.Drawing.Point(54, 81);
            this.txtIp.Name = "txtIp";
            this.txtIp.ReadOnly = true;
            this.txtIp.Size = new System.Drawing.Size(264, 20);
            this.txtIp.TabIndex = 36;
            this.txtIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(26, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "IP: ";
            // 
            // txtName
            // 
            this.txtName.Enabled = false;
            this.txtName.Location = new System.Drawing.Point(53, 55);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(264, 20);
            this.txtName.TabIndex = 34;
            this.txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Name: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 477);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Chat:";
            // 
            // txtChat
            // 
            this.txtChat.Enabled = false;
            this.txtChat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChat.ForeColor = System.Drawing.Color.Gray;
            this.txtChat.Hint = "Send message or command";
            this.txtChat.HintColor = System.Drawing.Color.Gray;
            this.txtChat.Location = new System.Drawing.Point(46, 464);
            this.txtChat.Multiline = true;
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(271, 37);
            this.txtChat.TabIndex = 31;
            this.txtChat.Text = "Send message or command";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(177, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Color: ";
            // 
            // btnColor
            // 
            this.btnColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.btnColor.Enabled = false;
            this.btnColor.Location = new System.Drawing.Point(220, 12);
            this.btnColor.Name = "btnColor";
            this.btnColor.Relation = null;
            this.btnColor.Size = new System.Drawing.Size(97, 23);
            this.btnColor.TabIndex = 29;
            this.btnColor.Text = "Purple";
            this.btnColor.UseVisualStyleBackColor = false;
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Control;
            this.txtStatus.Enabled = false;
            this.txtStatus.Location = new System.Drawing.Point(55, 14);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(100, 20);
            this.txtStatus.TabIndex = 28;
            this.txtStatus.Text = "Offline";
            this.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Status: ";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(513, 533);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(432, 533);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // PlayerManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 568);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpInfo);
            this.Controls.Add(this.grpPlayerList);
            this.Name = "PlayerManagerDialog";
            this.Text = "PlayerManagerDialog";
            this.grpPlayerList.ResumeLayout(false);
            this.grpInfo.ResumeLayout(false);
            this.grpInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPlayerList;
        private Components.ColoredListBox lstPlayers;
        private System.Windows.Forms.GroupBox grpInfo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private Components.HintedTextbox txtUndo;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnBan;
        private System.Windows.Forms.Button btnKick;
        private System.Windows.Forms.Button btnEditRank;
        private System.Windows.Forms.TextBox txtRank;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnEditMap;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMap;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Components.HintedTextbox txtChat;
        private System.Windows.Forms.Label label2;
        private MCForge.Gui.Components.ColorSelectionButton btnColor;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label1;
        private Components.ColorSelectionButton btnTitleColor;

    }
}