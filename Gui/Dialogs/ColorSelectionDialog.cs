using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using MCForge.Gui.Components;

namespace MCForge.Gui.Dialogs {
    public partial class ColorSelectionDialog : Form {

        [Browsable( false )]
        public ColorRelation Relation { get; set; }

        private ColorRelation preDraw;

        public ColorSelectionDialog(ColorRelation preDraw) {
            if(preDraw == null)
                throw new ArgumentNullException("preDraw", "Cannot be null");

            this.preDraw = preDraw;
            InitializeComponent();

            this.chatPreview1.RenderText(preDraw.MinecraftColorCode + "Color Preview");
        }

        private void btnBlack_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&0Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
        }

        private void btnTeal_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&bColor Preview" );
            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
        }

        private void btnDarkRed_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&4Color Preview" );
            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
        }

        private void btnGold_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&6Color Preview" );
            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
            
        }

        private void btnLightGreen_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&aColor Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnDarkGrey_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&8Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnDarkGreen_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&2Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnPurple_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&5Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnGray_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&7Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnDarkBlue_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&1Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnBlue_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&9Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnDarkTeal_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&3Color Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
        }

        private void btnRed_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&cColor Preview" );


            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
        }

        private void btnPink_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&dColor Preview" );


            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;
        }

        private void btnYellow_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&eColor Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }

        private void btnWhite_Click( object sender, EventArgs e ) {
            this.chatPreview1.RenderText( "&fColor Preview" );

            ColorRelatedButton b = sender as ColorRelatedButton;
            Relation = b.Relation;

        }



        private void btnOk_Click( object sender, EventArgs e ) {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click( object sender, EventArgs e ) {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void ColorSelectionDialog_Load( object sender, EventArgs e ) {

            this.btnBlack.Relation = ColorRelation.Black;
            this.btnBlue.Relation = ColorRelation.Blue;
            this.btnDarkBlue.Relation = ColorRelation.Navy;
            this.btnDarkGreen.Relation = ColorRelation.Green;
            this.btnDarkGrey.Relation = ColorRelation.Gray;
            this.btnDarkRed.Relation = ColorRelation.Maroon;
            this.btnDarkTeal.Relation = ColorRelation.Teal;
            this.btnGold.Relation = ColorRelation.Gold;
            this.btnGray.Relation = ColorRelation.Silver;
            this.btnLightGreen.Relation = ColorRelation.Lime;
            this.btnPink.Relation = ColorRelation.Pink;
            this.btnPurple.Relation = ColorRelation.Purple;
            this.btnRed.Relation = ColorRelation.Red;
            this.btnTeal.Relation = ColorRelation.Aqua;
            this.btnWhite.Relation = ColorRelation.White;
            this.btnYellow.Relation = ColorRelation.Yellow;
        }
    }
}
