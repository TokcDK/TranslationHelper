﻿//using KenshModTIO;
//using TranslationHelper.Data;

//namespace TranslationHelper.Formats.LOFI.Kenshi
//{
//    class MOD : FormatStringBase
//    {
//        protected override bool TryOpen()
//        {
//            AddTables();

//            GetStrings();

//            return CheckTablesContent(FilePath);
//        }

//        void GetStrings()
//        {
//            foreach (var str in KenshModIO.GetStrings(ProjectData.CurrentProject.SelectedGameDir, FilePath))
//            {
//                AddRowData(str, CheckInput: false);
//            }
//        }
//    }
//}
