using System;
using System.Windows.Forms;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public class MainMenuFileSubItemCaseMorph : MainMenuEditSubItemBase, IFileListMenuItem, IFileRowMenuItem, IProjectMenuItem
    {
        public override string Text => T._("Case Morph");

        public override IMenuItem[] Childs => new IMenuItem[5] 
        { 
            new MenuItemCaseMorphLowerVariated(),
            new MenuItemCaseMorphlower1stVariated(),
            new MenuItemCaseMorphUpperVariated(),
            new MenuItemCaseMorphUpperAllLinesVariated(),
            new MenuItemCaseMorphUPPERAllVariated(),
        };

        public override void OnClick(object sender, EventArgs e) { }
    }

    internal class MenuItemCaseMorphLowerVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To lower");

        public override string Description => T._("Change case of rows chars to lower");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToLowerCaseAll().AllT(); }

        protected override void OnTable(object sender, EventArgs e) { _ = new ToLowerCaseAll().TableT(); }

        protected override void OnRows(object sender, EventArgs e) { new ToLowerCaseAll().Rows(); }
    }
    internal class MenuItemCaseMorphlower1stVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To lower 1st");

        public override string Description => T._("Change first char case of row string to lower");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToLowerCaseFirst().AllT(); }

        protected override void OnTable(object sender, EventArgs e) { _ = new ToLowerCaseFirst().TableT(); }

        protected override void OnRows(object sender, EventArgs e) { new ToLowerCaseFirst().Rows(); }
    }

    internal class MenuItemCaseMorphUpperVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To Upper");

        public override string Description => T._("Change first char case of row string to Upper");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToUpperCaseFirst().AllT(); }

        protected override void OnTable(object sender, EventArgs e) { _ = new ToUpperCaseFirst().TableT(); }

        protected override void OnRows(object sender, EventArgs e) { new ToUpperCaseFirst().Rows(); }
    }

    internal class MenuItemCaseMorphUpperAllLinesVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To Upper")+" ("+ T._("All lines") + ")";

        public override string Description => T._("Change first char case of row string to Upper");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToLowerCaseFirstAllLines().AllT(); }

        protected override void OnTable(object sender, EventArgs e) { _ = new ToLowerCaseFirstAllLines().TableT(); }

        protected override void OnRows(object sender, EventArgs e) { new ToLowerCaseFirstAllLines().Rows(); }
    }

    internal class MenuItemCaseMorphUPPERAllVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To UPPER");

        public override string Description => T._("Change case of row string chars to UPPER");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToUpperCaseAll().AllT(); }

        protected override void OnTable(object sender, EventArgs e) { _ = new ToUpperCaseAll().TableT(); }

        protected override void OnRows(object sender, EventArgs e) { new ToUpperCaseAll().Rows(); }
    }
}
