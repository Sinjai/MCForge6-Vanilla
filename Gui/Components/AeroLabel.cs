using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using System.Drawing;
using MCForge.Gui.Components.Interfaces;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace MCForge.Gui.Components {
    /// <summary>
    /// A label that renders correctly over vista glass. It is just a regular label if cannot render glass.
    /// </summary>
    public class AeroLabel : Label {

        protected override void OnPaint(PaintEventArgs e) {
            if (DesignMode || !Natives.CanUseAero) {
                base.OnPaint(e);
                return;
            }

            using (Bitmap b = new Bitmap(Width / 5, Height / 5))
            using (GraphicsPath path = new GraphicsPath())
            using (Graphics temp = Graphics.FromImage(b))
            using (Pen glow = new Pen(Color.Red, 3))
            using (Brush text = new SolidBrush(Color.Black))
            using (Matrix matrix = new Matrix(1.0f / 5, 0, 0, 1.0f / 5, -(1.0f / 5), -(1.0f / 5))) {

                path.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, Location, StringFormat.GenericTypographic);
                temp.SmoothingMode = SmoothingMode.AntiAlias;
                temp.Transform = matrix;
                temp.DrawPath(glow, path);
                temp.FillPath(Brushes.Red, path);

                e.Graphics.Transform = new Matrix(1, 0, 0, 1, 50, 50);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                e.Graphics.DrawImage(b, ClientRectangle, 0, 0, b.Width, b.Height, GraphicsUnit.Pixel);
                e.Graphics.FillPath(text, path);
            }

        }

        protected override void OnPaintBackground(PaintEventArgs pevent) {
            if (!DesignMode)
                pevent.Graphics.Clear(Color.Transparent);
            else
                base.OnPaintBackground(pevent);
        }

    }
}
