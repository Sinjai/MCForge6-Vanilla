using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MCForge.Gui.Properties;
using MCForge.World.Drawing;
using MCForge.Utils;
using MCForge.Gui.Utils;

namespace MCForge.Gui.Components {
    public partial class ChatPreview : Control {

        private bool _drawBackground = true;
        private Image _background;


        private TextSection[] _sections;
        private static readonly ColorWithShadow[] _colorPairs;
        private static readonly Font MinecraftFont;

        static ChatPreview() {
            MinecraftFont = new Font( MCForge.World.Drawing.Fonts.Minecraft, 16 );

            //Colors found at http://wiki.vg/Classic_Protocol#Color_Codes
            //Must be in order from 0-f
            _colorPairs = new[] {
                                new ColorWithShadow(Color.FromArgb(255, 0,0,0 ), Color.FromArgb(255, 0,0,0)), 
                                new ColorWithShadow(Color.FromArgb(255, 0,0,191) ,Color.FromArgb(255,0,0,47)),
                                new ColorWithShadow(Color.FromArgb(255, 0,191, 0 ), Color.FromArgb(255,0,47,0)),
                                new ColorWithShadow(Color.FromArgb(255, 0,191, 191), Color.FromArgb(255,0,47,47)),
                                new ColorWithShadow(Color.FromArgb(255, 191,0, 0), Color.FromArgb(255, 47,0,0)),
                                new ColorWithShadow(Color.FromArgb(255, 191,0, 191), Color.FromArgb(255,47,0,47)),
                                new ColorWithShadow(Color.FromArgb(255, 191,191,0), Color.FromArgb(255,47,47,0)),
                                new ColorWithShadow(Color.FromArgb(255, 191,191,191), Color.FromArgb(255,47,47,47)),

                                new ColorWithShadow(Color.FromArgb(255, 64,64,64 ), Color.FromArgb(255,16,16,16)),
                                new ColorWithShadow(Color.FromArgb(255, 64,64,255 ), Color.FromArgb(255,16,16,63)),
                                new ColorWithShadow(Color.FromArgb(255, 64,255,64), Color.FromArgb(255,16,63,16)),
                                new ColorWithShadow(Color.FromArgb(255, 64,255,255), Color.FromArgb(255,16,63,63)),
                                new ColorWithShadow(Color.FromArgb(255, 255,64,64), Color.FromArgb(255,63,16,16)),
                                new ColorWithShadow(Color.FromArgb(255, 255,64,255), Color.FromArgb(255,63,16,63)),
                                new ColorWithShadow(Color.FromArgb(255, 255,255,64), Color.FromArgb(255,63,63,16)),
                                new ColorWithShadow(Color.FromArgb(255, 255,255,255), Color.FromArgb(255,63,63,63))
            };
        }

        #region Properties

        [Browsable( true )]
        [DefaultValue( true )]
        [Category( "Apperence" )]
        public bool DrawBackground {
            get { return _drawBackground; }
            set { _drawBackground = value; }
        }

        [Browsable( true )]
        [Category( "Apperence" )]
        public Image Background {
            get { return _background; }
            set { _background = value; }
        }

        #endregion



        public ChatPreview() {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public ChatPreview( IContainer container ) {
            container.Add( this );
            InitializeComponent();
            DoubleBuffered = true;
        }

        public void RenderText( params string[] lines ) {
            var sections = new List<TextSection>();

            //Assuming this is the fastest way to create graphics. If not, please let me know.
            using ( var img = new Bitmap( 1, 1 ) )
            using ( var graphics = Graphics.FromImage( img ) ) {
                float currY = 0;

                for ( int i = 0; i < lines.Length; i++ ) {
                    string line = lines[ i ];
                    if ( String.IsNullOrWhiteSpace( line ) )
                        continue;
                    
                    float currX = 0;

                    string[] split = line.Split( new[] { '&', '%' }, StringSplitOptions.RemoveEmptyEntries );

                    for ( int j = 0; j < split.Length; j++ ) {
                        string sec = split[ i ];

                        int colorIndex = Utilities.HexIntFromChar( sec[ 0 ] );

                        if ( colorIndex > 'f' )
                            colorIndex = 'f';

                        
                        sections.Add( new TextSection() {
                            ColorPair = _colorPairs[ colorIndex ],
                            Y = Height / 2 - graphics.MeasureString( sec.Substring( 1 ), MinecraftFont ).Height / 2,
                            X = Width / 2 - graphics.MeasureString( sec.Substring( 1 ), MinecraftFont ).Width / 2,
                            Text = sec.Substring( 1 )
                        } );

                        currX += graphics.MeasureString( sec.Substring( 1 ), MinecraftFont ).Width;
                    }

                    if ( split.Length >= 1 )
                        currY +=  graphics.MeasureString( split[ 0 ], MinecraftFont ).Height  + Constants.PADDING;
                }
            }
            _sections = sections.ToArray();
            Invalidate(); //Redraw control
        }

        protected override void OnPaint( PaintEventArgs e ) {


            e.Graphics.DrawImage( Background, 0, 0, Width, Height ); //Streeeeetch

            if ( _sections != null && _sections.Length >= 1 ) {
                for ( int i = 0; i < _sections.Length; i++ ) {
                    _sections[ i ].Render( e.Graphics );
                }
            }

            base.OnPaint( e );
        }




        #region Inner classes

        private struct ColorWithShadow {

            public readonly Brush Foreground, Background;

            public ColorWithShadow( Color foreGround, Color shadow ) {
                Foreground = new SolidBrush( foreGround );
                Background = new SolidBrush( shadow );
            }

        }

        private class TextSection {
            public string Text { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public ColorWithShadow ColorPair { get; set; }

            public void Render( Graphics graphics ) {
                graphics.DrawString( Text, MinecraftFont, ColorPair.Background, X + 3, Y + 3);
                graphics.DrawString( Text, MinecraftFont, ColorPair.Foreground, X, Y );
            }

        }

        #endregion
    }
}
