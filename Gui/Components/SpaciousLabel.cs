using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MCForge.Gui.Components {
    public partial class SpaciousLabel : Label {

        private float _spacing = 1.5f;

        [Browsable(true)]
        [DefaultValue(1.5f)]
        [Category("MCForge")]
        public float Spacing {
            get { return _spacing; }
            set { _spacing = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaciousLabel"/> class.
        /// </summary>
        public SpaciousLabel() :
            base() {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e) {
            using ( var brush = new SolidBrush(ForeColor) ) {
                string[] splits = Text.Split(Environment.NewLine.ToArray());
                float currPosY = Padding.Top;

                for ( int i = 0; i < splits.Length; i++ ) {

                    e.Graphics.DrawString(splits[i], Font, brush, Padding.Right, currPosY);
                    currPosY += Spacing + e.Graphics.MeasureString(splits[i], Font).Height;
                }
            }
        }

    }
}
