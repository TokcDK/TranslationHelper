using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsSave
    {
        internal static async Task PrepareToWrite()
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
                await Task.Run(() => AppData.CurrentProject.Save()).ConfigureAwait(true);
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

            foreach (var codesData in new Dictionary<string, Dictionary<int, int>>()
                    {
                        {"RPGMakerMV Added codes stats", AppData.RpgMVAddedCodesStat },
                        {"RPGMakerMV Skipped codes stats", AppData.RpgMVSkippedCodesStat }
                    }
            )
            {
                if (AppData.Settings.THConfigINI.SectionExistsAndNotEmpty(codesData.Key))
                {
                    foreach (var pair in AppData.Settings.THConfigINI.GetSectionKeyValuePairs(codesData.Key))
                    {
                        var intKey = int.Parse(pair.Key);
                        var intValue = int.Parse(pair.Value);
                        if (codesData.Value.TryGetValue(intKey, out int foundValue))
                        {
                            codesData.Value[intKey] = foundValue + intValue;
                        }
                        else
                        {
                            codesData.Value.Add(intKey, intValue);
                        }
                    }
                }

                foreach (var pair in codesData.Value)
                {
                    AppData.Settings.THConfigINI.SetKey(codesData.Key, pair.Key + "", pair.Value + "");
                }
            }

            AppData.Settings.THConfigINI.WriteFile();
        }
    }
}
