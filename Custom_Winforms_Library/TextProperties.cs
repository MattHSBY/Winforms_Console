using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Winforms_Library
{
    public class TextProperties
    {
        public Color? TextColor { get; set; }
        public Font TextFont { get; set; }
        public float TextSize { get; set; }
        public FontStyle fontStyle { get; set; }



        public TextProperties(Color? textColor = null, Font? textFont = null, float textSize = 12f, FontStyle fontStyle = FontStyle.Regular)
        {
            TextFont = textFont ?? SystemFonts.DefaultFont;
            TextSize = textSize;
            this.fontStyle = fontStyle;
            TextFont = new Font(TextFont, fontStyle);
            TextColor = textColor;
        }


    }
}
