using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Components;
using MCForge.Gui.Utils;

namespace MCForge.Gui.Dialogs {
    public partial class TextOnlyDialog : AeroForm {

        [Browsable(true)]
        [Category("MCForge")]
        [DefaultValue("MCForge TextOnlyDialog")]
        public string ContentText { get; set; }

        public TextOnlyDialog(string Text) {
            InitializeComponent();
            this.ContentText = Text;
        }

        public TextOnlyDialog() :
            this(string.Empty) { }

        private void TextOnlyDialog_Load(object sender, EventArgs e) {

            if ( Natives.CanUseAero ) {

                this.GlassArea = new Rectangle {
                    Location = label1.Location,
                    Width = label1.Location.X, 
                    Height = label1.Location.Y + 30
                };

                Invalidate();
            }
            else {
                label1.BackColor = BackColor;
                label1.Padding = new Padding();
            }

            label1.Text = ContentText;
        }

        protected override void OnPaint(PaintEventArgs e) {
            if ( !Natives.CanUseAero ) {
                base.OnPaint(e);
                return;
            }

            e.Graphics.Clear(Color.Black);
        }

    }
}
