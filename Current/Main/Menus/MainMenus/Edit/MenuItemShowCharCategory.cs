using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemShowCharCategory : MainMenuEditSubItemBase, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("ShowCharCategory");

        public override string Description => T._("ShowCharCategory");

        public override void OnClick(object sender, EventArgs e)
        {
            CharFunctionTest();
        }

        private void CharFunctionTest()
        {
            if (AppData.Main.THFileElementsDataGridView.SelectedCells.Count == 1 && (AppData.Main.THFileElementsDataGridView.SelectedCells[0].Value + string.Empty).Length > 0)
            {
                HashSet<char> chars = new HashSet<char>();
                foreach (var c in (AppData.Main.THFileElementsDataGridView.SelectedCells[0].Value + string.Empty))
                {
                    if (chars.Contains(c)) continue;

                    _ = MessageBox.Show("'" + c + "' category is " + Char.GetUnicodeCategory(c));

                    _ = chars.Add(c);
                }
            }


            //foreach (var c in new char[] { '「', '>', '\0', '\n', '\\', '-', '>' })
            //{
            //    var c1 = char.IsSymbol(c);
            //    var c2 = char.IsWhiteSpace(c);
            //    var c3 = char.IsControl(c);
            //    var c4 = char.IsSurrogate(c);
            //    var c5 = char.IsHighSurrogate(c);
            //    var c6 = char.IsLowSurrogate(c);
            //    var c7 = char.IsLetterOrDigit(c);
            //    var c8 = char.IsPunctuation(c);
            //    var c9 = char.IsSeparator(c);
            //}
        }
    }
}
