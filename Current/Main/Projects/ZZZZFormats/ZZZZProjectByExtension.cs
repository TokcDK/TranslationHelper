using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects.ZZZZFormats
{
    class ZZZZProjectByExtension : ProjectBase
    {
        /// <summary>
        /// Project will be set be specified extension of found format
        /// </summary>
        public ZZZZProjectByExtension()
        {
        }

        public override bool IsSaveToSourceFile => true; // we opened standalone file and will write in it

        internal override bool IsValid()
        {
            var fileExt = Path.GetExtension(AppData.SelectedProjectFilePath);
            foreach (var formatType in GetFormatTypes(typeof(FormatBase)))
                if (IsValidFormat(formatType, fileExt)) return true;

            return false;
        }

        private static IEnumerable<Type> GetFormatTypes(Type type)
        {
            foreach (var fType in
                GetListOfSubClasses.Inherited.GetInheritedTypes(type)) yield return fType;
        }

        private static bool IsValidFormat(Type formatType, string fileExt)
        {
            var format = (IFormat)Activator.CreateInstance(formatType);

            return string.Equals(format.Extension, fileExt, StringComparison.InvariantCultureIgnoreCase);
        }

        FormatBase Format;

        public override string Name => Format == null ? T._("Try open file by extension") : string.IsNullOrWhiteSpace(Format.Description) ? Format.Extension : Format.Description;

        public override bool Open()
        {
            var fileExt = Path.GetExtension(AppData.SelectedProjectFilePath);
            var foundTypes = new List<Type>();
            foreach (var formatType in GetFormatTypes(typeof(FormatBase)))
                if (IsValidFormat(formatType, fileExt)) foundTypes.Add(formatType);

            if (foundTypes.Count == 0) return false;

            int selectedIndex = -1;
            var foundForm = new FoundTypesbyExtensionForm();
            foreach (var type in foundTypes) foundForm.listBox1.Items.Add(type.FullName);

            var result = foundForm.ShowDialog();
            if (result == DialogResult.OK) selectedIndex = foundForm.SelectedTypeIndex;

            foundForm.Dispose();

            if (selectedIndex == -1) return false;

            Format = (FormatBase)Activator.CreateInstance(foundTypes[selectedIndex]);

            return OpenSave();
        }

        public override bool Save() => OpenSave();

        bool GetAll = false;
        bool IsRecursive = false;
        bool OpenSave()
        {
            var dir = Path.GetDirectoryName(AppData.SelectedProjectFilePath);
            var ext = Format.Extension;
            int extCnt = 0;
            foreach (var i in Directory.EnumerateFiles(dir, "*" + ext, SearchOption.AllDirectories)) if (++extCnt > 1) break;

            if (!SaveFileMode && extCnt > 1)
            {
                var dialogForm = GetOpenAllDialogForm(out CheckBox isRecursiveCheckbox);
                if (dialogForm.ShowDialog() == DialogResult.OK)
                {
                    GetAll = true;
                    IsRecursive = isRecursiveCheckbox.Checked;
                }
            }

            return ProjectToolsOpenSave.OpenSaveFilesBase(this, new DirectoryInfo(dir), Format.GetType(), GetAll ? "*" + ext : Path.GetFileName(Format.FilePath), false, searchOption: IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        private Form GetOpenAllDialogForm(out CheckBox isRecursiveCheckbox)
        {
            // parent
            var form = new Form
            {
                Height = 200,
                Width = 400
            };
            var sPanel = new TableLayoutPanel
            {
                AutoSize = true,
                RowCount = 3,
                ColumnCount = 1
            };
            var r1 = new RowStyle
            {
                SizeType = SizeType.Percent,
                Height = 70
            };
            var r2 = new RowStyle
            {
                SizeType = SizeType.Percent,
                Height = 30
            };
            var r3 = new RowStyle
            {
                SizeType = SizeType.Percent,
                Height = 30
            };
            sPanel.RowStyles.Add(r1);
            sPanel.RowStyles.Add(r2);
            sPanel.RowStyles.Add(r3);
            sPanel.Dock = DockStyle.Fill;
            form.Controls.Add(sPanel);

            // message
            var messageLabel = new Label
            {
                Text = T._("Found similar files. Open them too?"),
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            sPanel.Controls.Add(messageLabel);
            sPanel.SetColumn(messageLabel, 0);

            // checkbox
            var recursiveCheckbox = new CheckBox
            {
                Checked = true,
                Text = T._("Recursive")
            };
            sPanel.Controls.Add(recursiveCheckbox);
            sPanel.SetColumn(recursiveCheckbox, 1);
            isRecursiveCheckbox = recursiveCheckbox;

            // buttons yes/no
            var okButton = new Button
            {
                Text = "V",
                Dock = DockStyle.Fill
            };
            okButton.Click += (o, ea) =>
            {
                form.DialogResult = DialogResult.OK;
                form.Close();
            };
            var noButton = new Button
            {
                Text = "X",
                Dock = DockStyle.Fill
            };
            noButton.Click += (o, ea) =>
            {
                form.DialogResult = DialogResult.No;
                form.Close();
            };
            var buttonsPanel = new TableLayoutPanel
            {
                AutoSize = true,
                RowCount = 1,
                ColumnCount = 2,
                Dock = DockStyle.Fill
            };
            buttonsPanel.Controls.Add(okButton);
            buttonsPanel.Controls.Add(noButton);
            buttonsPanel.SetColumn(okButton, 0);
            buttonsPanel.SetColumn(noButton, 1);
            var c1 = new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 50
            };
            var c2 = new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 50
            };
            buttonsPanel.ColumnStyles.Add(c1);
            buttonsPanel.ColumnStyles.Add(c2);
            sPanel.Controls.Add(buttonsPanel);
            sPanel.SetColumn(buttonsPanel, 2);

            return form;
        }
    }
}
