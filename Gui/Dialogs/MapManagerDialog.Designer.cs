namespace MCForge.Gui.Dialogs {
    partial class MapManagerDialog {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lstUnloaded = new System.Windows.Forms.ListBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.grpUnloaded = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtaLoaded = new System.Windows.Forms.DataGridView();
            this.btnCreateLevel = new System.Windows.Forms.Button();
            this.grpUnloaded.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaLoaded)).BeginInit();
            this.SuspendLayout();
            // 
            // lstUnloaded
            // 
            this.lstUnloaded.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstUnloaded.FormattingEnabled = true;
            this.lstUnloaded.ItemHeight = 18;
            this.lstUnloaded.Location = new System.Drawing.Point(6, 19);
            this.lstUnloaded.Name = "lstUnloaded";
            this.lstUnloaded.Size = new System.Drawing.Size(281, 472);
            this.lstUnloaded.TabIndex = 0;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(8, 500);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 47);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(89, 500);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(72, 47);
            this.btnBackup.TabIndex = 2;
            this.btnBackup.Text = "Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(167, 500);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(121, 47);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // grpUnloaded
            // 
            this.grpUnloaded.Controls.Add(this.btnLoad);
            this.grpUnloaded.Controls.Add(this.btnBackup);
            this.grpUnloaded.Controls.Add(this.btnDelete);
            this.grpUnloaded.Controls.Add(this.lstUnloaded);
            this.grpUnloaded.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpUnloaded.Location = new System.Drawing.Point(12, 4);
            this.grpUnloaded.Name = "grpUnloaded";
            this.grpUnloaded.Size = new System.Drawing.Size(293, 553);
            this.grpUnloaded.TabIndex = 1;
            this.grpUnloaded.TabStop = false;
            this.grpUnloaded.Text = "Unloaded";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtaLoaded);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(311, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(610, 610);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Loaded";
            // 
            // dtaLoaded
            // 
            this.dtaLoaded.AllowDrop = true;
            this.dtaLoaded.AllowUserToAddRows = false;
            this.dtaLoaded.AllowUserToDeleteRows = false;
            this.dtaLoaded.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtaLoaded.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dtaLoaded.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtaLoaded.DefaultCellStyle = dataGridViewCellStyle2;
            this.dtaLoaded.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dtaLoaded.Location = new System.Drawing.Point(7, 21);
            this.dtaLoaded.MultiSelect = false;
            this.dtaLoaded.Name = "dtaLoaded";
            this.dtaLoaded.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtaLoaded.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dtaLoaded.ShowEditingIcon = false;
            this.dtaLoaded.Size = new System.Drawing.Size(597, 583);
            this.dtaLoaded.TabIndex = 0;
            this.dtaLoaded.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtaLoaded_CellClick);
            // 
            // btnCreateLevel
            // 
            this.btnCreateLevel.Location = new System.Drawing.Point(18, 567);
            this.btnCreateLevel.Name = "btnCreateLevel";
            this.btnCreateLevel.Size = new System.Drawing.Size(121, 47);
            this.btnCreateLevel.TabIndex = 4;
            this.btnCreateLevel.Text = "Create Level";
            this.btnCreateLevel.UseVisualStyleBackColor = true;
            // 
            // MapManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 626);
            this.Controls.Add(this.btnCreateLevel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpUnloaded);
            this.Name = "MapManagerDialog";
            this.Text = "MapManagerDialog";
            this.Load += new System.EventHandler(this.MapManagerDialog_Load);
            this.grpUnloaded.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtaLoaded)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstUnloaded;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox grpUnloaded;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dtaLoaded;
        private System.Windows.Forms.Button btnCreateLevel;

    }
}