using System;
using System.Windows.Forms;
using TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    public class MainMenuFileSubItemCaseMorph : MainMenuEditSubItemBase
    {
        public override string Text => T._("Case Morph");

        public override IMenuItem[] Childs => new IMenuItem[3] 
        { 
            new MenuItemCaseMorphLowerVariated(),
            new MenuItemCaseMorphUpperVariated(),
            new MenuItemCaseMorphUPPERAllVariated(),
        };

        public override void OnClick(object sender, EventArgs e) { }
    }

    internal class MenuItemCaseMorphLowerVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To lower");

        public override string Description => T._("Change case of rows chars to lower");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToLowerCaseAll().AllT(); }

        protected override void OnRows(object sender, EventArgs e) { _ = new ToLowerCaseAll().TableT(); }

        protected override void OnTable(object sender, EventArgs e) { new ToLowerCaseAll().Rows(); }
    }

    internal class MenuItemCaseMorphUpperVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To Upper");

        public override string Description => T._("Change first case of row string to Upper");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToUpperCaseFirst().AllT(); }

        protected override void OnRows(object sender, EventArgs e) { _ = new ToUpperCaseFirst().TableT(); }

        protected override void OnTable(object sender, EventArgs e) { new ToUpperCaseFirst().Rows(); }
    }

    internal class MenuItemCaseMorphUPPERAllVariated : AllTableRowsChildMenuBase
    {
        public override string Text => T._("To Upper");

        public override string Description => T._("Change case of row string chars to UPPER");

        protected override void OnAll(object sender, EventArgs e) { _ = new ToUpperCaseAll().AllT(); }

        protected override void OnRows(object sender, EventArgs e) { _ = new ToUpperCaseAll().TableT(); }

        protected override void OnTable(object sender, EventArgs e) { new ToUpperCaseAll().Rows(); }
    }
}
