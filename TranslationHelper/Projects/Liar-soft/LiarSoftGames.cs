using RaiLTools;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Liar_soft;

namespace TranslationHelper.Projects.Liar_soft
{
    class LiarSoftGames : ProjectBase
    {
        public LiarSoftGames(ProjectData projectData) : base(projectData)
        {
        }

        internal override bool Check()
        {
            return Path.GetExtension(projectData.SPath).ToUpperInvariant()==".EXE" && File.Exists(Path.Combine(Path.GetDirectoryName(projectData.SPath), "scr.xfl"));
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
            var scrxfl = Path.Combine(Properties.Settings.Default.THSelectedGameDir, "scr.xfl");
            var archive = XflArchive.FromFile(scrxfl);
            Properties.Settings.Default.THProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjectFolderName(), Path.GetFileName(Properties.Settings.Default.THSelectedGameDir));
            var dir = Properties.Settings.Default.THProjectWorkDir;

            archive.ExtractToDirectory(dir);//extract all gsc to work dir

            //save gsc to txt
            foreach (var script in Directory.EnumerateFiles(dir, "*.gsc"))
            {
                var transFile = TransFile.FromGSC(script);
                transFile.Save(script + ".txt");//сохранить в txt
            }

            //open or save txt/gsc
            var ret = OpenSaveFilesBase(dir, new GSCTXT(projectData), "*.txt");

            if(projectData.SaveFileMode && ret)
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
