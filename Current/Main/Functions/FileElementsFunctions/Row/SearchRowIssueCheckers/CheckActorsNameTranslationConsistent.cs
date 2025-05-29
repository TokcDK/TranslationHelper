using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.SearchIssueCheckers
{
    internal class CheckActorsNameTranslationConsistent : SearchIssueCheckerBase
    {
        DataTable Actors;

        public CheckActorsNameTranslationConsistent()
        {
            if (AppSettings.SearchRowIssueOptionsCheckActors && Actors == null) GetActorsTable();
        }

        public override string Description => "Actor name is different or missing in translation";

        public override bool IsHaveTheIssue(SearchIssueCheckerData data)
        {
            if (!AppSettings.SearchRowIssueOptionsCheckActors) return false;

            //------------------------------
            //Проверка актеров
            if (Actors == null || Actors.Rows.Count == 0) return false;

            var thisOriginal = data.Original;
            var thisTranslation = data.Translation;
            foreach (DataRow ActorsLine in Actors.Rows)
            {
                var original = ActorsLine.Field<string>(Project.OriginalColumnIndex);
                if (original.IsMultiline() || original.Length > 255)//skip multiline and long rows
                {
                    continue;
                }
                var translation = ActorsLine.Field<string>(Project.OriginalColumnIndex);

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
            foreach (DataTable table in Project.FilesContent.Tables)
            {
                if (!table.TableName.StartsWith("Actors")) continue;

                Actors = table;
                break;
            }
        }
    }
}
