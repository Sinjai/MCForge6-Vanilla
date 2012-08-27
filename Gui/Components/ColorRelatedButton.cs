using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using System.ComponentModel;

namespace MCForge.Gui.Components {
    public class ColorRelatedButton : Button {

        private ColorRelation _relation = ColorRelation.Black;

        [Browsable( true )]
        public ColorRelation Relation {
            get { return _relation; }
            set {
                _relation = value;
                ForeColor = value.TextColor;
                BackColor = value.BackColor;
                Text = value.Text;
            }
        }





    }
}
