using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Extensions
{
    public static partial class ExtensionsElements
    {
        //https://www.codeproject.com/Tips/292107/TextBox-cursor-position
        /// <summary>
        /// Show Coordinates of cursor in textbox
        /// </summary>
        /// <param name="TextBox"></param>
        /// <returns></returns>
        public static System.Drawing.Point CurrentCharacterPosition(this System.Windows.Forms.RichTextBox TextBox)
        {
            if (TextBox == null)
            {
                return new System.Drawing.Point(0, 0);
            }

            int s = TextBox.SelectionStart;
            int y = TextBox.GetLineFromCharIndex(s);
            int x = s - TextBox.GetFirstCharIndexFromLine(y);

            return new System.Drawing.Point(x, y);
        }

        public static int CurrentSelectedTextLength(this System.Windows.Forms.RichTextBox TextBox)
        {
            if (TextBox == null)
            {
                return 0;
            }

            if (TextBox.SelectionLength > 0)
            {
                return TextBox.SelectionLength;
            }
            else
            {
                return TextBox.Text.Length;
            }
        }
    }
}
