using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace MCForge.Gui.Components {
    public partial class AeroButton : Button {

        protected bool IsBeingPressed { get; set; }

        protected bool IsFocused { get; set; }

        public AeroButton() {
            InitializeComponent();

        }

        public AeroButton(IContainer container) {
            container.Add(this);
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
           e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
           e.Graphics.CompositingQuality  = CompositingQuality.AssumeLinear;

            e.Graphics.Clear(Color.Transparent);
            VisualStyleElement element = GetState();
           ButtonRenderer.DrawButton(e.Graphics, ClientRectangle, (PushButtonState)element.State);
            using ( Brush brush = new SolidBrush(ForeColor) )
            using ( GraphicsPath path = new GraphicsPath() ) {
                path.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size * 1.3f , new Point(8, 4) , StringFormat.GenericDefault);
                e.Graphics.FillPath(brush, path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            e.Graphics.Clear(Color.Transparent);
        }

        

        VisualStyleElement GetState() {   
            if ( IsBeingPressed )
                return VisualStyleElement.Button.PushButton.Pressed;
            if ( IsFocused )
                return VisualStyleElement.Button.PushButton.Hot;

            return VisualStyleElement.Button.PushButton.Normal;
        }


    }
}
