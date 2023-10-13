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

            if (extCnt > 1 && !SaveFileMode)
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
            var form = new Form();
            form.Height = 200;
            form.Width = 400;
            var sPanel = new TableLayoutPanel();
            sPanel.AutoSize = true;
            sPanel.RowCount= 3;
            sPanel.ColumnCount = 1;
            var r1 = new RowStyle();
            r1.SizeType = SizeType.Percent;
            r1.Height = 70;
            var r2 = new RowStyle();
            r2.SizeType = SizeType.Percent;
            r2.Height = 30;
            var r3 = new RowStyle();
            r3.SizeType = SizeType.Percent;
            r3.Height = 30;
            sPanel.RowStyles.Add(r1);
            sPanel.RowStyles.Add(r2);
            sPanel.RowStyles.Add(r3);
            sPanel.Dock = DockStyle.Fill;
            form.Controls.Add(sPanel);

            var messageLabel = new Label();
            messageLabel.Text = T._("Found similar files. Open them too?");
            messageLabel.AutoSize = true;
            messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            messageLabel.Dock = DockStyle.Fill;
            sPanel.Controls.Add(messageLabel);
            sPanel.SetColumn(messageLabel, 0);

            var recursiveCheckbox = new CheckBox();
            recursiveCheckbox.Checked = true;
            recursiveCheckbox.Text = T._("Recursive");
            sPanel.Controls.Add(recursiveCheckbox);
            sPanel.SetColumn(recursiveCheckbox, 1);

            var okButton = new Button();
            okButton.Text = "V";
            okButton.Dock = DockStyle.Fill;
            okButton.Click += (o, ea) =>
            {
                form.DialogResult = DialogResult.OK;
                form.Close();
            };
            var noButton = new Button();
            noButton.Text = "X";
            noButton.Dock = DockStyle.Fill;
            noButton.Click += (o, ea) =>
            {
                form.DialogResult = DialogResult.No;
                form.Close();
            };
            var buttonsPanel = new TableLayoutPanel();
            buttonsPanel.AutoSize = true;
            buttonsPanel.RowCount= 1;
            buttonsPanel.ColumnCount = 2;
            buttonsPanel.Dock = DockStyle.Fill;
            buttonsPanel.Controls.Add(okButton);
            buttonsPanel.Controls.Add(noButton);
            buttonsPanel.SetColumn(okButton, 0);
            buttonsPanel.SetColumn(noButton, 1);
            var c1 = new ColumnStyle();
            c1.SizeType = SizeType.Percent;
            c1.Width= 50;
            var c2 = new ColumnStyle();
            c2.SizeType = SizeType.Percent;
            c2.Width= 50;
            buttonsPanel.ColumnStyles.Add(c1);
            buttonsPanel.ColumnStyles.Add(c2);
            sPanel.Controls.Add(buttonsPanel);
            sPanel.SetColumn(buttonsPanel, 2);

            isRecursiveCheckbox = recursiveCheckbox;

            return form;
        }
    }
}
