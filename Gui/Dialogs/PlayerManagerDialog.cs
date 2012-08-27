using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Entity;
using MCForge.Gui.Utils;

namespace MCForge.Gui.Dialogs {
    public partial class PlayerManagerDialog : Form {

        private Player selectedPlayer;

        public PlayerManagerDialog() {
            InitializeComponent();
        }

        private void lstPlayers_SelectedIndexChanged( object sender, EventArgs e ) {

            selectedPlayer = Player.Find( lstPlayers.SelectedItem.ToString().Substring( 1 ) );

            bool enabled = true;

#if !DEBUG
            if ( selectedPlayer == null ) {
                enabled = false;
            }
#endif


            this.grpInfo.Enabled = enabled;
            this.btnColor.Enabled = enabled;
            this.btnTitleColor.Enabled = enabled;
            this.txtTitle.Enabled = enabled;
            this.btnEditMap.Enabled = enabled;
            this.btnEditRank.Enabled = enabled;
            this.btnBan.Enabled = enabled;
            this.btnKick.Enabled = enabled;
            this.btnUndo.Enabled = enabled;
            this.txtChat.Enabled = enabled;
            this.txtUndo.Enabled = enabled;
            this.txtStatus.Enabled = enabled;

            this.label1.Enabled = enabled;
            this.label2.Enabled = enabled;
            this.label3.Enabled = enabled;
            this.label4.Enabled = enabled;
            this.label5.Enabled = enabled;
            this.label6.Enabled = enabled;
            this.label7.Enabled = enabled;
            this.label8.Enabled = enabled;

            if ( !enabled )
                return;

#if DEBUG
            btnColor.Relation = ColorRelation.Red;
            btnTitleColor.Relation = ColorRelation.Green;
#else
            btnColor.Relation = ColorRelation.FindColorRelationByMinecraftCode( selectedPlayer.Color );
            btnTitleColor.Relation = ColorRelation.FindColorRelationByMinecraftCode( selectedPlayer.ExtraData["TitleColor"].ToString() );
#endif
            

        }

    }
}
