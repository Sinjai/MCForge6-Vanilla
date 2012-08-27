using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace MCForge.Gui.Components {

    public class HintedTextbox : TextBox {

        private bool _inHintState = true;
        private string _hint = "Hint";
        private Color _hintColor = Color.Gray;
        private Color _defaultColor;

        public HintedTextbox() :
            base() {
            _defaultColor = ForeColor;
            this.Enter += new EventHandler( HintedTextbox_Enter );
            this.KeyDown += new KeyEventHandler( HintedTextbox_KeyDown );
            this.Leave += new EventHandler( HintedTextbox_Leave );
        }

        void HintedTextbox_KeyDown( object sender, KeyEventArgs e ) {
            if ( _inHintState )
                InHintState = false;
        }

        void HintedTextbox_Leave( object sender, EventArgs e ) {
            if ( String.IsNullOrEmpty( Text ) )
                InHintState = true;
        }

        void HintedTextbox_Enter( object sender, EventArgs e ) {
            InHintState = false;
        }


        [Browsable( true )]
        [DefaultValue( typeof( Color ), "grey" )]
        [Category( "Apperence" )]
        public Color HintColor {
            get { return _hintColor; }
            set { _hintColor = value; }
        }

        [DefaultValue( true )]
        [Browsable( true )]
        [Category( "Apperence" )]
        public bool InHintState {
            get { return _inHintState; }
            set {
                _inHintState = value;
                if ( value ) {
                    Text = Hint;
                    ForeColor = HintColor;
                }
                else {
                    Text = string.Empty;
                    ForeColor = _defaultColor;
                }
            }
        }

        [Browsable( true )]
        [DefaultValue( "Hint" )]
        [Category( "Apperence" )]
        public string Hint {
            get { return _hint; }
            set { _hint = value; }
        }


    }
}
