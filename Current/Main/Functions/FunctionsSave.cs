using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsSave
    {
        internal static async Task PrepareToWrite(HashSet<int> fileIndexesToWrite = null)
        {
            if (FunctionsUI.SaveInAction)
            {
                return;
            }
            FunctionsUI.SaveInAction = true;
            FunctionsUI.FileDataWasChanged = false;
            AppData.CurrentProject.SaveFileMode = true;

            if (AppData.CurrentProject != null)
            {
                AppData.CurrentProject.BakCreate();
                await Task.Run(() => AppData.CurrentProject.Save(fileIndexesToWrite)).ConfigureAwait(true);
            }

            FunctionsUI.SaveInAction = false;
            FunctionsSounds.PlayAsterisk();
        }

        /// <summary>
        /// Write RPGMakerMV event codes stats
        /// </summary>
        public static void WriteRPGMakerMVStats()
        {
            if (AppData.RpgMVAddedCodesStat.Count == 0 && AppData.RpgMVSkippedCodesStat.Count == 0)
            {
                return;
            }

            AppData.RpgMVAddedCodesStat = AppData.RpgMVAddedCodesStat.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
            AppData.RpgMVSkippedCodesStat = AppData.RpgMVSkippedCodesStat.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

            // Avoid creating dictionary on every call - use array of tuples instead
            var codesDataArray = new[]
            {
                ("RPGMakerMV Added codes stats", AppData.RpgMVAddedCodesStat),
                ("RPGMakerMV Skipped codes stats", AppData.RpgMVSkippedCodesStat)
            };

            foreach (var (key, value) in codesDataArray)
            {
                if (AppData.Settings.THConfigINI.SectionExistsAndNotEmpty(key))
                {
                    foreach (var pair in AppData.Settings.THConfigINI.GetSectionKeyValuePairs(key))
                    {
                        var intKey = int.Parse(pair.Key);
                        var intValue = int.Parse(pair.Value);
                        if (value.TryGetValue(intKey, out int foundValue))
                        {
                            value[intKey] = foundValue + intValue;
                        }
                        else
                        {
                            value.Add(intKey, intValue);
                        }
                    }
                }

                foreach (var pair in value)
                {
                    AppData.Settings.THConfigINI.SetKey(key, pair.Key + "", pair.Value + "");
                }
            }

            AppData.Settings.THConfigINI.WriteFile();
        }
    }
}
