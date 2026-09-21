using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// The window a themed message box is drawn in.
    /// <para>
    /// It is an ordinary themed window: it derives from <see cref="ThemableForm"/>, so the palette
    /// reaches its text, its buttons and its background the same way it reaches any other window, and
    /// a change of theme while it is open repaints it without this class knowing that themes exist.
    /// Nothing here names a colour.
    /// </para>
    /// <para>
    /// What it does own is the shape of the dialog: how wide the message may be before it wraps, where
    /// the icon sits, and which buttons the requested set turns into. The captions come from Windows
    /// rather than from this file, so they are in the language of the system exactly as the framework's
    /// dialog would show them; see <see cref="Caption"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Marked as code rather than as a form on purpose. It is built entirely in its constructor, which
    /// takes the message to show, so the designer has no way to create an instance of it and would
    /// only report that it cannot.
    /// </remarks>
    [DesignerCategory("Code")]
    internal sealed class ThemedMessageBoxForm : ThemableForm
    {
        /// <summary>
        /// Distance from the edge of the dialog to its contents.
        /// <para>
        /// Named for the dialog rather than simply <c>Margin</c> on purpose: a window already has a
        /// <see cref="Control.Margin"/>, and a constant of that name would hide it.
        /// </para>
        /// </summary>
        private const int DialogMargin = 16;

        /// <summary>Distance between the icon, the message and the row of buttons.</summary>
        private const int Gap = 14;

        private const int IconSize = 32;

        /// <summary>Distance between two buttons.</summary>
        private const int ButtonGap = 8;

        /// <summary>A button is never narrower than this, so that a short caption still looks like one.</summary>
        private const int ButtonMinWidth = 78;

        /// <summary>Room inside a button around its caption.</summary>
        private const int ButtonPadding = 28;

        /// <summary>How wide the message may get before it wraps onto another line.</summary>
        private const int TextMaxWidth = 420;

        private readonly string _message;
        private readonly MessageBoxButtons _buttons;
        private readonly MessageBoxIcon _icon;
        private readonly MessageBoxDefaultButton _defaultButton;

        internal ThemedMessageBoxForm(
            string message,
            string caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            _message = message ?? string.Empty;
            _buttons = buttons;
            _icon = icon;
            _defaultButton = defaultButton;

            Text = CaptionFor(caption);
            Font = SystemFonts.MessageBoxFont;

            // The layout below is measured from this very font, so the framework must not scale it a
            // second time.
            AutoScaleMode = AutoScaleMode.None;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            KeyPreview = true;

            Build();
        }

        /// <summary>
        /// One button: the caption Windows gives it, and the answer pressing it means. The captions are
        /// read once and shared, because they are the same for every dialog the application shows.
        /// </summary>
        private sealed class Choice
        {
            internal uint CaptionId;
            internal string Fallback;
            internal DialogResult Result;
        }

        private static readonly Choice Ok = new Choice { CaptionId = 800, Fallback = "OK", Result = DialogResult.OK };

        private static readonly Choice Cancel = new Choice { CaptionId = 801, Fallback = "Cancel", Result = DialogResult.Cancel };

        private static readonly Choice Abort = new Choice { CaptionId = 802, Fallback = "Abort", Result = DialogResult.Abort };

        private static readonly Choice Retry = new Choice { CaptionId = 803, Fallback = "Retry", Result = DialogResult.Retry };

        private static readonly Choice Ignore = new Choice { CaptionId = 804, Fallback = "Ignore", Result = DialogResult.Ignore };

        private static readonly Choice Yes = new Choice { CaptionId = 805, Fallback = "Yes", Result = DialogResult.Yes };

        private static readonly Choice No = new Choice { CaptionId = 806, Fallback = "No", Result = DialogResult.No };

        private void Build()
        {
            var choices = ChoicesFor(_buttons);
            var icon = IconFor(_icon);
            var iconWidth = icon == null ? 0 : IconSize;

            // The message, wrapped at the width it is allowed to take. Measured rather than guessed,
            // because the dialog is sized around it.
            var message = TextRenderer.MeasureText(
                _message,
                Font,
                new Size(AvailableTextWidth(), int.MaxValue),
                TextFormatFlags.WordBreak);

            var messageWidth = Math.Max(80, message.Width + 2);
            var messageHeight = Math.Max(Font.Height, message.Height);

            var buttons = BuildButtons(choices);
            var buttonRowWidth = RowWidth(buttons);

            var iconAndMessage = iconWidth + (iconWidth > 0 ? Gap : 0) + messageWidth;
            var clientWidth = Math.Max(iconAndMessage, buttonRowWidth) + (DialogMargin * 2);

            var buttonHeight = buttons.Length == 0 ? 0 : buttons[0].Height;
            var clientHeight = DialogMargin + Math.Max(iconWidth, messageHeight) + Gap + buttonHeight + DialogMargin;

            SuspendLayout();
            ClientSize = new Size(clientWidth, clientHeight);

            if (icon != null)
            {
                Controls.Add(new PictureBox
                {
                    Image = icon.ToBitmap(),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(IconSize, IconSize),
                    Location = new Point(DialogMargin, DialogMargin),
                    TabStop = false,
                });
            }

            Controls.Add(new Label
            {
                Text = _message,
                AutoSize = false,
                Font = Font,
                Bounds = new Rectangle(DialogMargin + iconWidth + (iconWidth > 0 ? Gap : 0), DialogMargin, messageWidth, messageHeight),
            });

            var buttonLeft = clientWidth - DialogMargin - buttonRowWidth;
            var buttonTop = clientHeight - DialogMargin - buttonHeight;

            foreach (var button in buttons)
            {
                button.Location = new Point(buttonLeft, buttonTop);
                buttonLeft += button.Width + ButtonGap;
                Controls.Add(button);
            }

            ResumeLayout(false);

            var index = DefaultIndex(choices.Length);
            var chosen = buttons.Length == 0 ? null : buttons[index];

            AcceptButton = chosen;
            ActiveControl = chosen;

            // Escape answers the button that means "no", and does nothing when the requested set has no
            // such button — which is what the framework's dialog does with the same set.
            CancelButton = CancelChoiceFor(choices, buttons);
        }

        /// <summary>
        /// How wide the message may be. A long message on a narrow screen wraps earlier rather than
        /// producing a dialog wider than the desktop.
        /// </summary>
        private static int AvailableTextWidth()
        {
            var screen = Screen.PrimaryScreen;

            if (screen == null)
            {
                return TextMaxWidth;
            }

            return Math.Max(160, Math.Min(TextMaxWidth, screen.WorkingArea.Width - 240));
        }

        private Button[] BuildButtons(Choice[] choices)
        {
            var buttons = new Button[choices.Length];
            var height = Math.Max(26, Font.Height + 12);

            for (var index = 0; index < choices.Length; index++)
            {
                var choice = choices[index];

                var button = new Button
                {
                    Text = Caption(choice),
                    DialogResult = choice.Result,
                    Font = Font,
                    AutoSize = false,
                    Height = height,
                    TabIndex = index,
                };

                button.Width = Math.Max(
                    ButtonMinWidth,
                    TextRenderer.MeasureText(button.Text, Font).Width + ButtonPadding);

                buttons[index] = button;
            }

            return buttons;
        }

        private static int RowWidth(Button[] buttons)
        {
            var width = 0;

            foreach (var button in buttons)
            {
                width += button.Width;
            }

            return buttons.Length > 1
                ? width + (ButtonGap * (buttons.Length - 1))
                : width;
        }

        /// <summary>
        /// The buttons the requested set turns into, in the order the framework's dialog lays them out.
        /// </summary>
        private static Choice[] ChoicesFor(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    return new[] { Ok, Cancel };

                case MessageBoxButtons.YesNo:
                    return new[] { Yes, No };

                case MessageBoxButtons.YesNoCancel:
                    return new[] { Yes, No, Cancel };

                case MessageBoxButtons.RetryCancel:
                    return new[] { Retry, Cancel };

                case MessageBoxButtons.AbortRetryIgnore:
                    return new[] { Abort, Retry, Ignore };

                default:
                    return new[] { Ok };
            }
        }

        /// <summary>Which of the buttons is the default one, clamped to the buttons that exist.</summary>
        private int DefaultIndex(int count)
        {
            var index = (int)_defaultButton;

            if (index < 0 || index >= count)
            {
                return 0;
            }

            return index;
        }

        /// <summary>
        /// The button Escape answers, or null when the set has no button that means "no".
        /// </summary>
        private static Button CancelChoiceFor(Choice[] choices, Button[] buttons)
        {
            for (var index = 0; index < choices.Length; index++)
            {
                if (choices[index] == Cancel || choices[index] == No)
                {
                    return buttons[index];
                }
            }

            // A set without one still has to be closable by Escape, and the first button is the one the
            // framework treats as the way out when there is nothing else.
            return buttons.Length > 0 ? buttons[0] : null;
        }

        /// <summary>
        /// The icon for a requested kind, or null when none was asked for.
        /// </summary>
        private static Icon IconFor(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    return SystemIcons.Error;

                case MessageBoxIcon.Warning:
                    return SystemIcons.Warning;

                case MessageBoxIcon.Information:
                    return SystemIcons.Information;

                case MessageBoxIcon.Question:
                    return SystemIcons.Question;

                default:
                    return null;
            }
        }

        /// <summary>
        /// The caption of the dialog. An empty one means "the name of the application", which is what
        /// the framework's dialog shows when it is given no caption.
        /// </summary>
        private static string CaptionFor(string caption)
        {
            if (!string.IsNullOrEmpty(caption))
            {
                return caption;
            }

            if (!string.IsNullOrEmpty(Application.ProductName))
            {
                return Application.ProductName;
            }

            return System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int LoadString(IntPtr instance, uint id, StringBuilder buffer, int size);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// The module the captions are read from. Windows keeps them among its own string resources,
        /// which is how its dialogs are translated without every application translating them again.
        /// </summary>
        private static readonly IntPtr User32 = GetModuleHandle("user32.dll");

        /// <summary>
        /// What Windows calls a button, in the language of the system. Read from the system rather than
        /// written down here, because a translation table of the application's own would say "Yes" in
        /// English to a user whose dialogs have always said "Да".
        /// </summary>
        private static string Caption(Choice choice)
        {
            if (User32 == IntPtr.Zero)
            {
                return choice.Fallback;
            }

            var buffer = new StringBuilder(64);

            return LoadString(User32, choice.CaptionId, buffer, buffer.Capacity) > 0
                ? buffer.ToString()
                : choice.Fallback;
        }

        /// <summary>
        /// Answers the question with the way out when the dialog is closed by its caption button, so
        /// that a dialog can never be dismissed without an answer. A caller that asked a question and
        /// tested for Yes therefore takes its cautious branch, which is what closing a dialog without
        /// reading it should mean.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && DialogResult == DialogResult.None)
            {
                DialogResult = CancelButton == null ? DialogResult.OK : CancelButton.DialogResult;
            }

            base.OnFormClosing(e);
        }
    }
}
