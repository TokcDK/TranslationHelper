using RaiLTools;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Liar_soft;

namespace TranslationHelper.Projects.Liar_soft
{
    class LiarSoftGames : ProjectBase
    {
        public LiarSoftGames() : base()
        {
        }

        internal override bool Check()
        {
            return Path.GetExtension(ProjectData.SelectedFilePath).ToUpperInvariant()==".EXE" && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "scr.xfl"));
        }

        internal override string Name()
        {
            return "Liar-Soft";
        }
        internal override string ProjectFolderName()
        {
            return "Liar-Soft";
        }

        internal override bool Open()
        {
            return OpenSaveXFL();
        }

        internal override bool Save()
        {
            return OpenSaveXFL();
        }

        bool OpenSaveXFL()
        {
            //set vars
            var scrxfl = Path.Combine(ProjectData.SelectedGameDir, "scr.xfl");
            var archive = XflArchive.FromFile(scrxfl);
            ProjectData.ProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), Path.GetFileName(ProjectData.SelectedGameDir));
            var dir = ProjectData.ProjectWorkDir;

            archive.ExtractToDirectory(dir);//extract all gsc to work dir

            //save gsc to txt
            foreach (var script in Directory.EnumerateFiles(dir, "*.gsc"))
            {
                var transFile = TransFile.FromGSC(script);
                transFile.Save(script + ".txt");//сохранить в txt
            }

            //open or save txt/gsc
            var ret = OpenSaveFilesBase(dir, new GSCTXT(), "*.txt");

            if(ProjectData.SaveFileMode && ret)
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
            return BackupRestorePaths(new[] { @".\scr.xfl" });
        }

        internal override bool BakRestore()
        {
            return BackupRestorePaths(new[] { @".\scr.xfl" }, false);
        }
    }
}
