namespace TranslationHelper.Extensions
{
    public static partial class ExtensionsElements
    {
        //https://www.codeproject.com/Tips/292107/TextBox-cursor-position
        /// <summary>
        /// Show Coordinates of cursor in textbox
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static System.Drawing.Point CurrentCharacterPosition(this System.Windows.Forms.RichTextBox textBox)
        {
            if (textBox == null)
            {
                return new System.Drawing.Point(0, 0);
            }

            int s = textBox.SelectionStart;
            int y = textBox.GetLineFromCharIndex(s);
            int x = s - textBox.GetFirstCharIndexFromLine(y);

            return new System.Drawing.Point(x, y);
        }

        public static int CurrentSelectedTextLength(this System.Windows.Forms.RichTextBox textBox)
        {
            if (textBox == null)
            {
                return 0;
            }

            if (textBox.SelectionLength > 0)
            {
                return textBox.SelectionLength;
            }
            else
            {
                return textBox.Text.Length;
            }
        }
    }
}
