using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMakerVX.RVData2;
using System.Linq;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class CheckActorsNameTranslationConsistent : ISearchIssueChecker
    {
        DataTable Actors;

        public CheckActorsNameTranslationConsistent()
        {
            if (Actors == null) GetActorsTable();
        }

        public string Description => "Actor name is different or missing in translation";

        public bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            if (!AppSettings.SearchRowIssueOptionsCheckActors) return false;

            //------------------------------
            //Проверка актеров
            if (Actors == null || Actors.Rows.Count == 0) return false;

            var thisOriginal = data.Original;
            var thisTranslation = data.Translation;
            foreach (DataRow ActorsLine in Actors.Rows)
            {
                var original = ActorsLine.Field<string>(AppData.CurrentProject.OriginalColumnIndex);
                if (original.IsMultiline() || original.Length > 255)//skip multiline and long rows
                {
                    continue;
                }
                var translation = ActorsLine.Field<string>(AppData.CurrentProject.OriginalColumnIndex);

                //если оригинал содержит оригинал(Анна) из Actors, а перевод не содержит определение(Anna) из Actors
                if (translation.Length > 0 
                    && original.Length < 80 
                    && thisOriginal.Contains(original) 
                    && !thisTranslation.Contains(translation))
                {
                    return true;
                }
            }

            return false;
        }

        private void GetActorsTable()
        {
            foreach (DataTable table in AppData.CurrentProject.FilesContent.Tables)
            {
                if (!table.TableName.StartsWith("Actors")) continue;

                Actors = table;
                break;
            }
        }
    }
}
