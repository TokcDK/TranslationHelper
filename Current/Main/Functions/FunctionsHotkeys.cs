using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRegisterHotkey;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Functions
{
    internal class FunctionsHotkeys
    {
        static HotKeyRegister THFileElementsDataGridViewOriginalToTranslationHotkey;
        static Keys registerKey = Keys.None;
        static KeyModifiers registerModifiers = KeyModifiers.None;
        internal static void BindShortCuts()
        {
            //THFileElementsDataGridViewOriginalToTranslationHotkey = new HotKeyRegister(this.Handle, 100, KeyModifiers.None, Keys.F8);
            //THFileElementsDataGridViewOriginalToTranslationHotkey.HotKeyPressed += new EventHandler(SetOriginalValueToTranslationToolStripMenuItem_Click);
        }

        /// <summary>
        /// Handle the KeyDown of tbHotKey. In this event handler, check the pressed keys.
        /// The keys that must be pressed in combination with the key Ctrl, Shift or Alt,
        /// like Ctrl+Alt+T. The method HotKeyRegister.GetModifiers could check whether 
        /// "T" is pressed.
        /// </summary>
        private void TbHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            // The key event should not be sent to the underlying control.
            e.SuppressKeyPress = true;

            // Check whether the modifier keys are pressed.
            if (e.Modifiers != Keys.None)
            {
                KeyModifiers modifiers = HotKeyRegister.GetModifiers(e.KeyData, out Keys key);

                // If the pressed key is valid...
                if (key != Keys.None)
                {
                    registerKey = key;
                    registerModifiers = modifiers;

                    // Display the pressed key in the textbox.
                    //tbHotKey.Text = string.Format("{0}+{1}",
                    //    this.registerModifiers, this.registerKey);

                    // Enable the button.
                    //btnRegister.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Handle the Click event of btnRegister.
        /// </summary>
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Register the hotkey.
                THFileElementsDataGridViewOriginalToTranslationHotkey = new HotKeyRegister(AppData.Main.Handle, 100,
                    registerModifiers, registerKey);

                // Register the HotKeyPressed event.
                THFileElementsDataGridViewOriginalToTranslationHotkey.HotKeyPressed += new EventHandler(HotKeyPressed);

                // Update the UI.
                //btnRegister.Enabled = false;
                //tbHotKey.Enabled = false;
                //btnUnregister.Enabled = true;
            }
            catch (ArgumentException argumentException)
            {
                _ = MessageBox.Show(argumentException.Message);
            }
            catch (ApplicationException applicationException)
            {
                _ = MessageBox.Show(applicationException.Message);
            }
        }

        /// <summary>
        /// Show a message box if the HotKeyPressed event is raised.
        /// </summary>
        static void  HotKeyPressed(object sender, EventArgs e)
        {
            //SetOriginalToTranslation();

            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    this.WindowState = FormWindowState.Normal;
            //}
            //this.Activate();
        }
    }
}
