using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MCForge.Gui.Utils;
using System.Windows.Forms;

namespace MCForge.Gui.Components {
    /// <summary>
    ///  List box that colors the text based on the first char in the item
    /// </summary>
    public class ColoredListBox : ListBox {

        /// <summary>
        /// Initializes a new instance of the <see cref="ColoredListBox"/> class.
        /// </summary>
        public ColoredListBox()
            : base() {
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.DrawItem += new System.Windows.Forms.DrawItemEventHandler(ColoredListBox_DrawItem);
        }

        void ColoredListBox_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {

            if ( e.Index < 0 || e.Index >= Items.Count )
                return;

            var Item = Items[e.Index] as ColoredItem;

            if ( Item == null || String.IsNullOrWhiteSpace(Item.Text) )
                return;

            e.DrawBackground();

            Color? foreColor = Utilities.GetDimColorFromChar(Item.Color);

            if ( e.State == System.Windows.Forms.DrawItemState.Selected || e.State == System.Windows.Forms.DrawItemState.Checked || e.State == System.Windows.Forms.DrawItemState.Focus ) {
                e.Graphics.DrawString(Item.Text, e.Font, Brushes.Black, e.Bounds);
            }
            else {
                using ( Brush brush = new SolidBrush(foreColor ?? e.ForeColor) )
                    e.Graphics.DrawString(Item.Text, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();

        }

        /// <summary>
        /// Gets or sets the currently selected item in the <see cref="T:System.Windows.Forms.ListBox"/>.
        /// </summary>
        /// <returns>An object that represents the current selection in the control.</returns>
        ///   
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        ///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///   </PermissionSet>
        public new string SelectedItem {
            get {
                if ( base.SelectedItem == null )
                    return string.Empty;
                return base.SelectedItem.ToString().Substring(1);
            }
            set {
                base.SelectedItem = value;
            }
        }


        /// <summary>
        /// Adds an item if it is not in the list
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        public void AddIfNotExist(char color, string text) {
            for ( int i = 0; i < Items.Count; i++ ) {
                var item = Items[i] as ColoredItem;

                if ( item == null )
                    continue;

                if ( item.Text == text && item.Color == color )
                    return;

            }

            Items.Add(new ColoredItem(color, text));
        }

        /// <summary>
        /// Removes an item if it is in the list
        /// </summary>
        /// <param name="text">The text.</param>
        public void RemoveIfExists(string text) {
            for ( int i = 0; i < Items.Count; i++ ) {
                var item = Items[i] as ColoredItem;

                if ( item == null )
                    continue;

                if ( item.Text == text )
                    Items.RemoveAt(i);

            }
        }
    }

    /// <summary>
    /// Colored item
    /// </summary>
    public class ColoredItem {

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public char Color { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColoredItem"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="text">The text.</param>
        public ColoredItem(char color, string text) {
            Color = color;
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColoredItem"/> class.
        /// </summary>
        public ColoredItem() {
            Color = 'a';
            Text = string.Empty;
        }

        public override string ToString() {
            return Text;
        }

    }
}
