using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCForge.Gui.Utils;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MCForge.Gui.Components {
    /// <summary>
    /// A rich text box, that can parse Minecraft/MCForge color codes.
    /// </summary>
    public partial class ColoredTextBox : RichTextBox {

        private bool _nightMode = false;
        private bool _colorize = true;
        private bool _showDateStamp = true;


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ColoredTextBox"/> is colorized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if colorized; otherwise, <c>false</c>.
        /// </value>
        [Browsable( true )]
        [Category( "Apperence" )]
        [DefaultValue( true )]
        public bool Colorize {
            get {
                return _colorize;
            }
            set {
                _colorize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it will include a date stamp in the log
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date stamp]; otherwise, <c>false</c>.
        /// </value>
        [Browsable( true )]
        [Category( "Apperence" )]
        [DefaultValue( true )]
        public bool DateStamp {
            get {
                return _showDateStamp;
            }
            set {
                _showDateStamp = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBox is in nightmode. This will clear the text box when changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [night mode]; otherwise, <c>false</c>.
        /// </value>
        [Browsable( true )]
        [Category( "Apperence" )]
        [DefaultValue( false )]
        public bool NightMode {
            get {
                return _nightMode;
            }
            set {
                _nightMode = value;

                Clear();

                ForeColor = value ? Color.White : Color.Black;
                BackColor = !value ? Color.White : Color.Black;

                Invalidate();
            }
        }


        private string dateStamp {
            get {
                return "[" + DateTime.Now.ToString( "T" ) + "] ";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColoredTextBox"/> class.
        /// </summary>
        public ColoredTextBox() : base() {
            InitializeComponent();
        }

        /// <summary>
        /// Appends the log.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public void AppendLog( string text, Color foreColor ) {
            if ( InvokeRequired ) {
                Invoke( ( MethodInvoker ) delegate { AppendLog( text, foreColor ); } );
                return;
            }

            if ( DateStamp )
                Append( dateStamp, Color.Gray, BackColor );

            if ( !Colorize ) {
                AppendText( text );
                return;
            }
            if ( !text.Contains( '&' ) && !text.Contains( '%' ) ) {
                Append( text, foreColor, BackColor );
                return;
            }

            string[] messagesSplit = text.Split( new[] { '%', '&' }, StringSplitOptions.RemoveEmptyEntries );

            for ( int i = 0; i < messagesSplit.Length; i++ ) {
                string split = messagesSplit[ i ];
                if ( String.IsNullOrWhiteSpace( split ) )
                    continue;
                Color? color = Utilities.GetDimColorFromChar( split[ 0 ] );
                Append( color != null ? split.Substring( 1 ) : split, color ?? foreColor, BackColor );
            }


            Refresh();
        }

        /// <summary>
        /// Appends the log.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public void AppendLog( string text ) {
            AppendLog(text, ForeColor);
        }

        /// <summary>
        /// Appends the log.
        /// </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="foreColor">Color of the foreground.</param>
        /// <param name="bgColor">Color of the background.</param>
        private void Append( string text, Color foreColor, Color bgColor ) {
            if ( InvokeRequired ) {
                Invoke( ( MethodInvoker ) delegate { Append( text, foreColor, bgColor ); } );
                return;
            }

            try
            {
                SelectionStart = TextLength;
                SelectionLength = 0;
                SelectionColor = foreColor;
                SelectionBackColor = bgColor;
                AppendText(text);
                SelectionBackColor = BackColor;
                SelectionColor = ForeColor;
            }
            catch { } //because it can cause a crash on shutdown
        }

        private void ColoredReader_LinkClicked( object sender, System.Windows.Forms.LinkClickedEventArgs e ) {
            if (!e.LinkText.Contains("minecraft.net") && !e.LinkText.Contains("classicube.net"))
            {
                if ( MessageBox.Show( "Never open links from people that you don't trust!", "Warning!!", MessageBoxButtons.OKCancel ) == DialogResult.Cancel )
                    return;
            }

            Process.Start( e.LinkText );
        }

        /// <summary>
        /// Scrolls to the end of the log
        /// </summary>
        public void ScrollToEnd() {
            if ( InvokeRequired ) {
                Invoke( ( MethodInvoker ) ScrollToEnd );
                return;
            }

            Select( Text.Length, 0 );
            ScrollToCaret();
        }

        



        #region Border Style

        private RECT _border;

        protected override void WndProc( ref Message m ) {
            if ( Environment.OSVersion.Platform != PlatformID.Win32NT ) {
                base.WndProc( ref m );
                return;
            }

            switch ( m.Msg ) {
                case Natives.WM_NCPAINT:
                    RenderStyle( ref m );
                    break;
                case Natives.WM_NCCALCSIZE:
                    CalculateSize( ref m );
                    break;
                case Natives.WM_THEMECHANGED:
                    UpdateStyles();
                    break;
                default:
                    base.WndProc( ref m );
                    break;
            }
        }

        private void CalculateSize( ref Message m ) {
            base.WndProc( ref m );

            if ( !Natives.CanRender() )
                return;

            Natives.NCCALCSIZE_PARAMS par = new Natives.NCCALCSIZE_PARAMS();

            RECT windowRect;

            if ( m.WParam == IntPtr.Zero ) {
                windowRect = ( RECT ) Marshal.PtrToStructure( m.LParam, typeof( RECT ) );
            }
            else {
                par = ( Natives.NCCALCSIZE_PARAMS ) Marshal.PtrToStructure( m.LParam, typeof( Natives.NCCALCSIZE_PARAMS ) );
                windowRect = par.rgrc0;
            }

            RECT contentRect;
            IntPtr hDC = Natives.GetWindowDC( this.Handle );
            IntPtr hTheme = Natives.OpenThemeData( this.Handle, "EDIT" );

            if ( Natives.GetThemeBackgroundContentRect( hTheme, hDC, Natives.EP_EDITTEXT, Natives.ETS_NORMAL, ref windowRect, out contentRect ) == Natives.S_OK ) {
                contentRect.Inflate( -1, -1 );
                this._border = new Margins( contentRect.Left - windowRect.Left,
                                                                        contentRect.Top - windowRect.Top,
                                                                         windowRect.Right - contentRect.Right,
                                                                         windowRect.Bottom - contentRect.Bottom );


                if ( m.WParam == IntPtr.Zero ) {
                    Marshal.StructureToPtr( contentRect, m.LParam, false );
                }
                else {
                    par.rgrc0 = contentRect;
                    Marshal.StructureToPtr( par, m.LParam, false );
                }

                m.Result = new IntPtr( Natives.WVR_REDRAW );
            }

            Natives.CloseThemeData( hTheme );
            Natives.ReleaseDC( this.Handle, hDC );

        }

        private void RenderStyle( ref Message m ) {
            base.WndProc( ref m );

            if ( !Natives.CanRender() ) {
                return;
            }

            int partId = Natives.EP_EDITTEXT;

            int stateId;
            if ( this.Enabled )
                if ( this.ReadOnly )
                    stateId = Natives.ETS_READONLY;
                else
                    stateId = Natives.ETS_NORMAL;
            else
                stateId = Natives.ETS_DISABLED;

            RECT windowRect;
            Natives.GetWindowRect( this.Handle, out windowRect );
            windowRect.Right -= windowRect.Left;
            windowRect.Bottom -= windowRect.Top;
            windowRect.Top = 0;
            windowRect.Left = 0;

            IntPtr hDC = Natives.GetWindowDC( this.Handle );

            RECT clientRect = windowRect;
            clientRect.Left += this._border.Left;
            clientRect.Top += this._border.Top;
            clientRect.Right -= this._border.Right;
            clientRect.Bottom -= this._border.Bottom;

            Natives.ExcludeClipRect( hDC, clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom );

            IntPtr hTheme = Natives.OpenThemeData( this.Handle, "EDIT" );

            if ( Natives.IsThemeBackgroundPartiallyTransparent( hTheme, Natives.EP_EDITTEXT, Natives.ETS_NORMAL ) != 0 )
                Natives.DrawThemeParentBackground( this.Handle, hDC, ref windowRect );


            Natives.DrawThemeBackground( hTheme, hDC, partId, stateId, ref windowRect, IntPtr.Zero );
            Natives.CloseThemeData( hTheme );
            Natives.ReleaseDC( this.Handle, hDC );
            m.Result = IntPtr.Zero;
        }

        protected override CreateParams CreateParams {

            get {
                CreateParams p = base.CreateParams;

                if ( Natives.CanRender() && ( p.ExStyle & Natives.WS_EX_CLIENTEDGE ) == Natives.WS_EX_CLIENTEDGE )
                    p.ExStyle ^= Natives.WS_EX_CLIENTEDGE;

                return p;
            }

        }

        #endregion

    }
}
