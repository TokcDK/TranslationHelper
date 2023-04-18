using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats
{
    abstract class ExportFormatsBase : RowBase
    {
        protected ExportFormatsBase()
        {
        }

        protected override void Init()
        {
            if (NeedInit)
            {
                _allRows = new List<string>(SelectedRowsCount);
                base.Init();
            }
        }

        /// <summary>
        /// contains all table rows original translation values
        /// </summary>
        List<string> _allRows;
        /// <summary>
        /// file save dialog filter
        /// determines file's extension
        /// </summary>
        protected abstract string Filter { get; }
        /// <summary>
        /// will be placed in first line of file
        /// </summary>
        protected virtual string MarkerFileStart { get => ""; }
        /// <summary>
        /// will be placed before new record
        /// </summary>
        protected virtual string MarkerStart { get => ""; }
        /// <summary>
        /// will be placed before original
        /// </summary>
        protected abstract string MarkerOiginal { get; }
        /// <summary>
        /// will be placed after original and before translation
        /// </summary>
        protected abstract string MarkerTranslation { get; }
        /// <summary>
        /// will be placed after record
        /// </summary>
        protected virtual string MarkerEnd { get => "\r\n\r\n"; }
        /// <summary>
        /// Encoding with which file will be saved
        /// </summary>
        protected virtual Encoding SaveEncoding { get => Encoding.UTF8; }

        protected override bool Apply()
        {
            Init();

            _allRows.Add(OriginalMod(Original) + MarkerTranslation + TranslationMod(Translation));

            if (IsLastRow)
            {
                using (var save = new SaveFileDialog())
                {
                    save.Filter = Filter;
                    if (save.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(save.FileName))
                    {
                        File.WriteAllText(save.FileName,
                            MarkerFileStart
                            + MarkerStart
                            + MarkerOiginal
                            + string.Join(MarkerEnd + MarkerStart + MarkerOiginal, _allRows)
                            + MarkerEnd
                            , SaveEncoding);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// modification of translation before add
        /// no modification by default
        /// </summary>
        /// <param name=THSettings.TranslationColumnName></param>
        /// <returns></returns>
        protected virtual string TranslationMod(string translation)
        {
            return translation;
        }

        /// <summary>
        /// modification of original before add
        /// no modification by default
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected virtual string OriginalMod(string original)
        {
            return original;
        }

        protected override bool IsValidRow()
        {
            return true;
        }
    }
}
