﻿namespace MCForge.Gui.Dialogs {
    partial class TextOnlyDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.label1 = new MCForge.Gui.Components.SpaciousLabel();
            this.aeroButton1 = new MCForge.Gui.Components.AeroButton(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(8);
            this.label1.Size = new System.Drawing.Size(338, 233);
            this.label1.TabIndex = 0;
            this.label1.Text = "fasdfjkafasdfasdfasdfasdfasdfasdfkasdfasdfasd\r\nfasd\r\nf\r\nasdf\r\nasdfasdfadsfadslfha" +
    "dskfash dlkfads j f;lkasdfgasdg\r\nadfgadfgadf hsfdh";
            // 
            // aeroButton1
            // 
            this.aeroButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aeroButton1.Location = new System.Drawing.Point(112, 245);
            this.aeroButton1.Name = "aeroButton1";
            this.aeroButton1.Size = new System.Drawing.Size(79, 23);
            this.aeroButton1.TabIndex = 1;
            this.aeroButton1.Text = "Aero Button";
            this.aeroButton1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(31, 245);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Button";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // TextOnlyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 296);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.aeroButton1);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "TextOnlyDialog";
            this.Text = "TextOnlyDialog";
            this.Load += new System.EventHandler(this.TextOnlyDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.SpaciousLabel label1;
        private Components.AeroButton aeroButton1;
        private System.Windows.Forms.Button button1;


    }
}