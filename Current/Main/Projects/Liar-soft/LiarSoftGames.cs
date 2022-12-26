using RaiLTools;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Liar_soft;

namespace TranslationHelper.Projects.Liar_soft
{
    class LiarSoftGames : ProjectBase
    {
        public LiarSoftGames()
        {
        }

        internal override bool IsValid()
        {
            return Path.GetExtension(AppData.SelectedFilePath).ToUpperInvariant()==".EXE" && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedFilePath), "scr.xfl"));
        }

        public override string Name => "Liar-Soft";
        internal override string ProjectDBFolderName => "Liar-Soft";

        public override bool Open()
        {
            return OpenSaveXFL();
        }

        public override bool Save()
        {
            return OpenSaveXFL();
        }

        bool OpenSaveXFL()
        {
            //set vars
            var scrxfl = Path.Combine(AppData.CurrentProject.SelectedGameDir, "scr.xfl");
            var archive = XflArchive.FromFile(scrxfl);
            AppData.CurrentProject.ProjectWorkDir = Path.Combine(THSettings.WorkDirPath, ProjectDBFolderName, Path.GetFileName(AppData.CurrentProject.SelectedGameDir));
            var dir = AppData.CurrentProject.ProjectWorkDir;

            archive.ExtractToDirectory(dir);//extract all gsc to work dir

            //save gsc to txt
            foreach (var script in Directory.EnumerateFiles(dir, "*.gsc"))
            {
                var transFile = TransFile.FromGSC(script);
                transFile.Save(script + ".txt");//сохранить в txt
            }

            //open or save txt/gsc
            var ret = OpenSaveFilesBase(dir, typeof(GSCTXT), "*.txt");

            if(SaveFileMode && ret)
            {
                //replace gsc entries with translated
                for (int i=0; i< archive.Entries.Count; i++)
                {
                    var translatedgsc = Path.Combine(dir, archive.Entries[i].Path);
                    if (File.Exists(translatedgsc))
                    {
                        var path = archive.Entries[i].Path;
                        archive.Remove(archive.Entries[i]);//remove non translated
                        archive.CreateEntry(path, File.ReadAllBytes(translatedgsc));//create new translated entrie with same name
                    }
                }

                archive.Save(scrxfl);//save translated scr.xfl
            }

            return ret;
        }

        internal override bool BakCreate()
        {
            return ProjectTools.BackupRestorePaths(new[] { @".\scr.xfl" });
        }

        internal override bool BakRestore()
        {
            return ProjectTools.BackupRestorePaths(new[] { @".\scr.xfl" }, false);
        }
    }
}
