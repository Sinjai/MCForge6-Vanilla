using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace MCForge.Gui.Utils {

    public class ColorRelation {

        public string MinecraftColorCode { get; set; }

        public string Text { get; set; }

        public Color BackColor { get; set; }

        public Color TextColor { get; set; }

        public ColorRelation( string minecraftColorCode, string text, Color highlight, Color textColor ) {
            this.BackColor = highlight;
            this.TextColor= textColor;
            this.Text = text;
            this.MinecraftColorCode = minecraftColorCode;
        }


        public static readonly ColorRelation Black  = new ColorRelation( "&0", "Black", Color.Black, Color.White );
        public static readonly ColorRelation Navy   = new ColorRelation( "&1", "Navy", Color.Navy, Color.White );
        public static readonly ColorRelation Green  = new ColorRelation( "&2", "Green", Color.Green, Color.White );
        public static readonly ColorRelation Teal = new ColorRelation("&3", "Teal", Color.Teal, Color.Black);
        public static readonly ColorRelation Maroon = new ColorRelation( "&4", "Maroon", Color.Maroon, Color.White );
        public static readonly ColorRelation Purple = new ColorRelation( "&5", "Purple", Color.Purple, Color.White );
        public static readonly ColorRelation Gold   = new ColorRelation( "&6", "Gold", Color.Gold, Color.Black );
        public static readonly ColorRelation Silver = new ColorRelation( "&7", "Silver", Color.Silver, Color.Black );
        public static readonly ColorRelation Gray   = new ColorRelation( "&8", "Gray", Color.Gray, Color.White );
        public static readonly ColorRelation Blue   = new ColorRelation( "&9", "Blue", Color.Blue, Color.White );
        public static readonly ColorRelation Lime   = new ColorRelation( "&a", "Lime", Color.Lime, Color.Black );
        public static readonly ColorRelation Aqua   = new ColorRelation( "&b", "Aqua", Color.Aqua, Color.Black );
        public static readonly ColorRelation Red    = new ColorRelation( "&c", "Red", Color.Red, Color.White );
        public static readonly ColorRelation Pink   = new ColorRelation( "&d", "Pink", Color.Pink, Color.Black );
        public static readonly ColorRelation Yellow = new ColorRelation( "&e", "Yellow", Color.Yellow, Color.Black );
        public static readonly ColorRelation White  = new ColorRelation( "&f", "White", Color.White, Color.Black);

        public static readonly ColorRelation[] Relations =  {

                                                                         Black,
                                                                         Navy,
                                                                         Green ,
                                                                         Teal,
                                                                         Maroon ,
                                                                         Purple ,
                                                                         Gold,
                                                                         Silver ,
                                                                         Gray  ,
                                                                         Blue,
                                                                         Lime,
                                                                         Aqua,
                                                                         Red  ,
                                                                         Pink  ,
                                                                         Yellow ,
                                                                         White
       };

        public static ColorRelation FindColorRelationByMinecraftCode( string code ) {
            foreach ( var relation in Relations ) {
                if ( String.Compare( relation.MinecraftColorCode, code, true ) == 0 ) {
                    return relation;
                }
            }
            return null;
        }
    }
}
