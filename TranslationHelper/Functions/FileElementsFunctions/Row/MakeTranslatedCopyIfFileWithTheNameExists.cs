using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class MakeTranslatedCopyIfFileWithTheNameExists : RowBase
    {
        public MakeTranslatedCopyIfFileWithTheNameExists() : base()
        {
        }

        bool _needToAddFilePaths = true;
        Dictionary<string, PathsForTheName> _gameFilesList;
        protected override void ActionsPreRowsApply()
        {
            if (_needToAddFilePaths)
            {
                _gameFilesList = new Dictionary<string, PathsForTheName>();
                foreach (var file in Directory.GetFiles(ProjectData.SelectedGameDir, "*", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (!_gameFilesList.ContainsKey(name))
                    {
                        _gameFilesList.Add(name, new PathsForTheName());//add file name with path
                    }
                    _gameFilesList[name].PathsList.Add(file);
                }
                _needToAddFilePaths = false;
            }
        }

        class PathsForTheName
        {
            /// <summary>
            /// list of paths in game's folder for the file's name without extension
            /// </summary>
            internal List<string> PathsList = new List<string>();
        }

        protected override bool Apply()
        {

            string orig = SelectedRow[0] as string;
            if (orig.IsMultiline()) // skip multiline
            {
                return false;
            }

            string trans = SelectedRow[1] + "";
            if (trans.IsMultiline()) // skip multiline
            {
                return false;
            }

            try
            {
                bool hasExtractedFromOrig = false;
                var extractedFromOrig = orig.ExtractMulty();
                string origName = orig;
                if (extractedFromOrig.Length == 1 // only one extracted
                    && !string.IsNullOrWhiteSpace(extractedFromOrig[0]) // not empty extracted value
                    && extractedFromOrig[0] != origName.Trim()) // not just trimmed extracted value
                {
                    origName = extractedFromOrig[0];
                    hasExtractedFromOrig = true;
                }

                if (FunctionsFileFolder.HasInvalidChars(origName))
                {
                    return false;
                }

                origName = Path.GetFileNameWithoutExtension(origName);

                string transName = trans;
                if (hasExtractedFromOrig)
                {
                    var extractedFromTrans = trans.ExtractMulty();

                    if (extractedFromTrans.Length == 1 // only one extracted
                        && !string.IsNullOrWhiteSpace(extractedFromTrans[0]) // not empty extracted value
                        && extractedFromTrans[0] != transName.Trim()) // not just trimmed extracted value
                    {
                        transName = extractedFromTrans[0];
                    }
                }

                if (string.IsNullOrWhiteSpace(transName) || FunctionsFileFolder.HasInvalidChars(transName)) // skip empty and invalid trans
                {
                    return false;
                }

                transName = Path.GetFileNameWithoutExtension(transName);

                if (_gameFilesList == null || !_gameFilesList.ContainsKey(origName)) // skip if not in game folder's files list
                {
                    return false;
                }

                foreach (var path in _gameFilesList[origName].PathsList) // iterate all paths
                {
                    if (string.IsNullOrWhiteSpace(path)) // skip empty?
                    {
                        continue;
                    }

                    var targetPath = Path.Combine(Path.GetDirectoryName(path), transName + Path.GetExtension(path)); // target translated path
                    if (!File.Exists(targetPath))
                    {
                        File.Copy(path, targetPath);

                        //info about translated copy
                        File.WriteAllText(targetPath + ".tr.txt", "original name:" + origName + "\r\noriginal file name is exists in table and in game dir\r\ncreated copy with translated name to prevent possible missing file errors");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                new FunctionsLogs().LogToFile(@"An error occured while file copy\write:\r\n" + ex + "\r\noriginal=" + SelectedRow[0] + "\r\ntranslation=" + SelectedRow[1]);
            }
            return false;
        }
    }
}
