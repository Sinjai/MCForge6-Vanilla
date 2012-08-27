using System.Windows.Forms;
namespace MCForge.Gui.Components {
    partial class AeroButton {
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();

            this.MouseHover += new System.EventHandler(AeroButton_MouseHover);
            this.MouseLeave += new System.EventHandler(AeroButton_MouseLeave);
            this.MouseEnter += new System.EventHandler(AeroButton_MouseEnter);

            this.MouseDown += new MouseEventHandler(AeroButton_MouseDown);
            this.MouseUp += new MouseEventHandler(AeroButton_MouseUp);

        }


        void AeroButton_MouseUp(object sender, MouseEventArgs e) {
            this.IsBeingPressed = false;
            Invalidate();
        }

        void AeroButton_MouseDown(object sender, MouseEventArgs e) {
            this.IsBeingPressed = true;
             Invalidate();
        }

        void AeroButton_MouseEnter(object sender, System.EventArgs e) {
            this.IsFocused = true;
             Invalidate();
        }

        void AeroButton_MouseLeave(object sender, System.EventArgs e) {
            this.IsFocused = false;
             Invalidate();
        }

        void AeroButton_MouseHover(object sender, System.EventArgs e) {
            this.IsFocused = true;
             Invalidate();
        }

        #endregion
    }
}
