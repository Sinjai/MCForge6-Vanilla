using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCForge.Gui.Components.Interfaces {
    /// <summary>
    /// Interface that has a form assoiated with it.
    /// </summary>
    public interface IFormSharer {
        /// <summary>
        /// Gets the form to share.
        /// </summary>
       Form FormToShare { get; }
    }
}
