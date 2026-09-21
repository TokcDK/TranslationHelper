using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemOpenSettings : MainMenuViewSubItemBase
    {
        public override string Text => T._("Open Settings");

        public override string Description => Text;

        /// <summary>
        /// The one settings window of this session. It lives here rather than in <c>AppData</c>
        /// because only this menu opens it, and because the settings themselves are no longer held
        /// by the form: the window is a view over <c>SettingsRegistry</c> and nothing more.
        /// </summary>
        private static THfrmSettings _settingsForm;

        public override void OnClick(object sender, EventArgs e)
        {
            try
            {
                if (_settingsForm == null || _settingsForm.IsDisposed) _settingsForm = new THfrmSettings();

                if (_settingsForm.Visible)
                {
                    _settingsForm.Activate();
                }
                else _settingsForm.Show();
            }
            catch
            {
            }
        }
    }
}
