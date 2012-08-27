using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MCForge.Gui.Utils;

namespace MCForge.Gui.Components {
    public class ToolTipListBox : ListBox {

        TypeCollection<ListBoxEntry> _collection;
        public new TypeCollection<ListBoxEntry> Items { get { return _collection; } }

        private ToolTip currentToolTip;

        public ToolTipListBox() {
            _collection = new TypeCollection<ListBoxEntry>( this );

            this.MouseHover += new EventHandler( ToolTipListBox_MouseHover );
            this.MouseLeave += new EventHandler( ToolTipListBox_MouseLeave );
        }


        void ToolTipListBox_MouseLeave( object sender, EventArgs e ) {
            if ( currentToolTip != null ) {
                currentToolTip.Hide( this );
                currentToolTip.Dispose();
            }
        }


        void ToolTipListBox_MouseHover( object sender, EventArgs e ) {
            ListBoxEntry entry = DetermineHoveredItem();
            if ( entry == null ) {
                if ( currentToolTip != null ) {
                    currentToolTip.Hide( this );
                    currentToolTip.Dispose();
                }
                return;
            }


            currentToolTip = new ToolTip() {
                ToolTipTitle = entry.Text,
                ToolTipIcon = ToolTipIcon.Info
            };
            Point drawLocation = PointToClient( ListBox.MousePosition );
            drawLocation.Offset(8, 8);
            currentToolTip.Show( entry.HoverText, this, drawLocation);
        }

        private ListBoxEntry DetermineHoveredItem() {
            Point screenPosition = ListBox.MousePosition;
            Point listBoxClientAreaPosition = PointToClient( screenPosition );

            int hoveredIndex = IndexFromPoint( listBoxClientAreaPosition );
            return hoveredIndex != -1 ? Items[ hoveredIndex ] : null;
        }

    }

    /// <summary>
    /// Listbox item
    /// </summary>
    public class ListBoxEntry {

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the hover text.
        /// </summary>
        /// <value>
        /// The hover text.
        /// </value>
        public string HoverText { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxEntry"/> class.
        /// </summary>
        public ListBoxEntry()
            : this( "", "" ) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxEntry"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hoverText">The hover text.</param>
        public ListBoxEntry( string text, string hoverText ) {
            this.Text = text;
            this.HoverText = hoverText;
        }

        public override string ToString() {
            return Text;
        }
    }

}
