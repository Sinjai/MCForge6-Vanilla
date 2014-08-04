using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;

namespace MCForge.Gui.Components {
    internal class GlassToolStripMenuItem : ToolStripMenuItem {

        /* For default value sake */

        private Color _gradTop = Color.FromArgb(50, 194, 224, 255);
        private Color _gradBottom = Color.FromArgb(150, 194, 224, 255);
        private Color _outLine = Color.FromArgb(255, 51, 153, 255);


        /// <summary>
        /// Gets or sets the gradiant top color.
        /// </summary>
        /// <value>
        /// The gradiant color top.
        /// </value>
        [Browsable(true)]
        [DefaultValue(typeof(Color), "0,0,0,0")]
        [Obsolete]
        public Color GradiantColorTop {
            get {
                return _gradTop;
            }
            set {
                _gradTop = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the gradiant bottom color.
        /// </summary>
        /// <value>
        /// The gradiant color bottom.
        /// </value>
        [Browsable(true)]
        [DefaultValue(typeof(Color), "150,194,224,255")]
        public Color GradiantColorBottom {
            get {
                return _gradBottom;
            }
            set {
                _gradBottom = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the outline.
        /// </summary>
        /// <value>
        /// The color of the outline.
        /// </value>
        [Browsable(true)]
        [DefaultValue(typeof(Color), "255,51,153,255")]
        public Color OutlineColor {
            get {
                return _outLine;
            }
            set {
                _outLine = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (IsOnDropDown || !Natives.CanUseAero) {
                base.OnPaint(e);
            }
            else {

                if (Selected && !Pressed) {
                    DrawBackground(e);
                }
                if (Pressed) {
                    DrawBackgroundPressed(e);
                }

                using (SolidBrush brush = new SolidBrush(ForeColor))
                    e.Graphics.DrawString(Text, Font, brush, Padding.Left, Padding.Top);
            }
        }

        private void DrawBackground(PaintEventArgs e) {
            using (var brush = new SolidBrush(GradiantColorBottom)) {
                using (Pen pen = new Pen(OutlineColor)) {

                    Rectangle region = Rectangle.FromLTRB(0, 0, Width - 1, Height - 2);

                    e.Graphics.FillRectangle(brush, region);
                    e.Graphics.DrawRectangle(pen, region);
                }
            }
        }

        private void DrawBackgroundPressed(PaintEventArgs e) {
            if (DropDownItems.Count < 1) {
                return;
            }
            Rectangle region = Rectangle.FromLTRB(0, 0, Width - 1, Height - 2);
            e.Graphics.FillRectangle(Brushes.White, region);

            Point[] points = new Point[4];
            points[0] = new Point(0, region.Height);
            points[1] = new Point();
            points[2] = new Point(region.Width, 0);
            points[3] = new Point(region.Width, region.Height);

            e.Graphics.DrawLines(Pens.Gray, points);

        }
    }
}
