using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCForge.Gui.Components {

    /// <summary>
    /// Like a web browser, but limited to only reading the html
    /// </summary>
    public class HtmlReader : WebBrowser {

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader"/> class.
        /// </summary>
        /// <param name="html">The HTML.</param>
        public HtmlReader( string html )
            : this() {
            WriteHtml(html);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReader"/> class.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Windows.Forms.WebBrowser"/> control is hosted inside Internet Explorer.</exception>
        public HtmlReader() {
            this.AllowNavigation = false;
            this.AllowWebBrowserDrop = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScriptErrorsSuppressed = true;
        }

        /// <summary>
        /// Writes the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        public void WriteHtml( string html ) {
            HtmlDocument doc = Document.OpenNew( true );
            doc.Write( html );
        }

        protected override CreateParams CreateParams {
            get {
                var parms = base.CreateParams;
                      parms.Style |= 0x800000;
                return parms;
            }
        }
    }
}
