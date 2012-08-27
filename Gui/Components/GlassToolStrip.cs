using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using System.Drawing.Drawing2D;
using MCForge.Gui.Components.Interfaces;
using System.Runtime.Serialization;

namespace MCForge.Gui.Components {
    public partial class GlassMenuStrip : MenuStrip {

        /// <summary>
        /// Gets or sets the color top.
        /// </summary>
        /// <value>
        /// The color top.
        /// </value>
        public Color ColorTop { get; set; }
        /// <summary>
        /// Gets or sets the color bottom.
        /// </summary>
        /// <value>
        /// The color bottom.
        /// </value>
        public Color ColorBottom { get; set; }
        /// <summary>
        /// Gets or sets the color outline outer.
        /// </summary>
        /// <value>
        /// The color outline outer.
        /// </value>
        public Color ColorOutlineOuter { get; set; }
        /// <summary>
        /// Gets or sets the color outline inner.
        /// </summary>
        /// <value>
        /// The color outline inner.
        /// </value>
        public Color ColorOutlineInner { get; set; }


        public GlassMenuStrip() {
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (!Natives.CanUseAero) {
                base.OnPaint(e);
                return;
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            int contentLength = 2;

            for (int i = 0; i < Items.Count; i++) {
                var item = Items[i];
                contentLength += item.Width + 2;
            }

            var brush = new LinearGradientBrush(new PointF(), new Point(0, Height), ColorTop, ColorBottom);
            var penInner = new Pen(ColorOutlineInner);
            var penOuter = new Pen(ColorOutlineOuter);


            //Rectangles of all sorts of shapes and sizes
            var rectInner = new RectangleF {
                X = ClientRectangle.X + 5,
                Y = ClientRectangle.Y + 2,
                Width = contentLength,
                Height = ClientRectangle.Height + 1
            };
            var rectOuter = new RectangleF {
                X = ClientRectangle.X + 4.5f,
                Y = ClientRectangle.Y + 1.5f,
                Width = contentLength + 1f,
                Height = ClientRectangle.Height + 2f
            };


            if (DesignMode) {
                e.Graphics.Clear(Color.FromArgb(0xD7, 0xE4, 0xF2));
            }
            else {
                Natives.FillBlackRegion(e.Graphics, ClientRectangle);
            }



            e.Graphics.DrawRoundedRectangle(penOuter, rectOuter, 5f);
            e.Graphics.DrawRoundedRectangle(penInner, rectInner, 5f);
            e.Graphics.FillRoundedRectangle(brush, rectInner, 5f);

            penInner.Dispose();
            penOuter.Dispose();
            brush.Dispose();

            base.OnPaint(e);
        }



        protected override void OnPaintBackground(PaintEventArgs e) {
            if (!Natives.CanUseAero) {
                base.OnPaintBackground(e);
                return;
            }
        }
    }
}
