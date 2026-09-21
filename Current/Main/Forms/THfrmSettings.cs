using System.Windows.Forms;
using TranslationHelper.Settings;
using TranslationHelper.Theming;

namespace TranslationHelper
{
    /// <summary>
    /// The settings window.
    /// <para>
    /// It owns no setting. It asks <see cref="SettingsRegistry"/> for the settings that exist,
    /// groups them by section into tabs, and gives each one an editor that matches its
    /// <see cref="Setting.ValueKind"/>. Editing an editor writes through
    /// <see cref="Setting.SetValueFromObject"/>, which raises <c>Changed</c>; that is what makes
    /// <see cref="SettingsIniStore"/> write the new value to the INI file straight away. The window
    /// never touches the file itself.
    /// </para>
    /// <para>
    /// Adding a setting therefore adds a row here without this file being edited, and a feature can
    /// change its own settings without the settings window being touched at all.
    /// </para>
    /// </summary>
    public partial class THfrmSettings : ThemableForm
    {
        /// <summary>Width of the column that holds the setting names.</summary>
        private const int LabelColumnWidth = 280;

        /// <summary>Height of one setting's row. Fixed, so rows line up across every tab.</summary>
        private const int RowHeight = 28;

        /// <summary>Width of the editor column for free text, leaving room for the quick links.</summary>
        private const int TextEditorWidth = 360;

        internal THfrmSettings()
        {
            InitializeComponent();

            Text = T._("Settings");

            BuildSettingsPages();
        }

        /// <summary>
        /// Themes the tooltip.
        /// <para>
        /// A tooltip is a component rather than a control, so it is not in the control tree the theme
        /// is applied to and it has to be done here. This is also why the theme can be changed from
        /// inside this very window and take effect at once: the window is one of the windows
        /// <see cref="ThemeManager"/> repaints, so the tooltip is refreshed by the same change that
        /// repainted the rest of the window.
        /// </para>
        /// </summary>
        protected internal override void OnThemeApplied()
        {
            var theme = ThemeManager.Instance.CurrentTheme;

            THSettingsToolTip.BackColor = theme.EditorBack;
            THSettingsToolTip.ForeColor = theme.EditorText;
        }

        /// <summary>
        /// Builds one tab per section, in the order the sections declared for themselves, so the
        /// window follows the same order as the INI file and neither has to be kept in step by hand.
        /// </summary>
        private void BuildSettingsPages()
        {
            foreach (var section in SettingsRegistry.Sections)
            {
                var table = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    ColumnCount = 2,
                    RowCount = 0,
                };
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, LabelColumnWidth));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                foreach (var setting in section.Settings)
                {
                    AddSettingRow(table, setting);
                }

                var page = new TabPage(T._(section.Name))
                {
                    Padding = new Padding(3),
                    UseVisualStyleBackColor = true,
                };
                page.Controls.Add(table);

                THSettingsTabControl.TabPages.Add(page);
            }
        }

        /// <summary>
        /// Adds one row: the setting's name on the left, its editor on the right, and its own
        /// description as the tooltip of both. The description is written by the setting, so the
        /// explanation the user reads here is the same text that is written above the key in the
        /// INI file.
        /// </summary>
        private void AddSettingRow(TableLayoutPanel table, Setting setting)
        {
            var row = table.RowCount;
            table.RowCount = row + 1;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, RowHeight));

            var label = new Label
            {
                Text = T._(setting.Label),
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(3, 8, 3, 0),
            };
            table.Controls.Add(label, 0, row);

            var editor = CreateEditor(setting);
            editor.Anchor = AnchorStyles.Left;
            table.Controls.Add(editor, 1, row);

            var description = T._(setting.Description);
            THSettingsToolTip.SetToolTip(label, description);
            THSettingsToolTip.SetToolTip(editor, description);
        }

        /// <summary>
        /// The one place that maps a kind of value to a kind of editor. A new
        /// <see cref="SettingValueKind"/> needs a new case here and nothing else in the application.
        /// </summary>
        private Control CreateEditor(Setting setting)
        {
            switch (setting.ValueKind)
            {
                case SettingValueKind.Bool:
                    return CreateBoolEditor(setting);
                case SettingValueKind.Int:
                    return CreateIntEditor(setting);
                case SettingValueKind.Choice:
                    return CreateChoiceEditor(setting);
                default:
                    return CreateTextEditor(setting);
            }
        }

        private static Control CreateBoolEditor(Setting setting)
        {
            var checkBox = new CheckBox
            {
                Checked = (bool)setting.ValueAsObject,
                AutoSize = true,
            };

            checkBox.CheckedChanged += (sender, e) => setting.SetValueFromObject(checkBox.Checked);

            return checkBox;
        }

        /// <summary>
        /// A number box that applies its value when it loses focus rather than on every keystroke.
        /// Typing "300" would otherwise store 3, then 30, then 300, briefly leaving an
        /// out-of-range value in the INI file. The box is then refilled from the setting, so what
        /// it shows is always the value that was really accepted.
        /// </summary>
        private static Control CreateIntEditor(Setting setting)
        {
            var textBox = new TextBox
            {
                Text = setting.ValueAsString,
                TextAlign = HorizontalAlignment.Center,
                Width = 64,
            };

            textBox.Validated += (sender, e) =>
            {
                setting.SetValueFromString(textBox.Text);
                textBox.Text = setting.ValueAsString;
            };

            return textBox;
        }

        /// <summary>
        /// A drop-down limited to the values the setting accepts, so the INI file cannot be given a
        /// value the application does not implement.
        /// </summary>
        private static Control CreateChoiceEditor(Setting setting)
        {
            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 220,
            };

            foreach (var choice in ((IChoiceSetting)setting).Choices)
            {
                comboBox.Items.Add(choice);
            }

            var current = setting.ValueAsString;
            if (comboBox.Items.Contains(current))
            {
                comboBox.SelectedItem = current;
            }

            comboBox.SelectionChangeCommitted += (sender, e) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    setting.SetValueFromObject(comboBox.SelectedItem);
                }
            };

            return comboBox;
        }

        /// <summary>
        /// A text box, plus one quick link per known-good value when the setting offers any. A link
        /// fills the box in and applies it, so a service can be picked with one click instead of
        /// typing its URL. Like the number box, the text is applied when the box loses focus.
        /// </summary>
        private Control CreateTextEditor(Setting setting)
        {
            var textBox = new TextBox
            {
                Text = setting.ValueAsString,
                Width = TextEditorWidth,
            };

            textBox.Validated += (sender, e) =>
            {
                setting.SetValueFromString(textBox.Text);
                textBox.Text = setting.ValueAsString;
            };

            var row = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0),
            };
            row.Controls.Add(textBox);

            if (setting is ISuggestingSetting suggesting)
            {
                foreach (var suggestion in suggesting.Suggestions)
                {
                    row.Controls.Add(CreateQuickLink(suggestion, textBox, setting));
                }
            }

            return row;
        }

        private LinkLabel CreateQuickLink(string value, TextBox textBox, Setting setting)
        {
            var link = new LinkLabel
            {
                Text = ShortName(value),
                AutoSize = true,
                Margin = new Padding(8, 5, 0, 0),
            };

            link.LinkClicked += (sender, e) =>
            {
                textBox.Text = value;
                setting.SetValueFromString(value);
            };

            THSettingsToolTip.SetToolTip(link, value);

            return link;
        }

        /// <summary>
        /// A one-letter name for a service URL, so the quick links stay narrow and fit beside the
        /// text box. The scheme and the usual "www." / "translate." prefixes carry no information
        /// when the list is already known to be translation services.
        /// </summary>
        private static string ShortName(string url)
        {
            var name = url;

            if (name.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(8);
            }
            else if (name.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(7);
            }

            if (name.StartsWith("www.", System.StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(4);
            }

            if (name.StartsWith("translate.", System.StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(10);
            }

            return name.Length == 0
                ? url
                : char.ToUpperInvariant(name[0]).ToString();
        }
    }
}
