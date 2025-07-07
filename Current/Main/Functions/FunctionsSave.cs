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
            if (AppData.RpgMVAddedCodesStat.Count > 0 || AppData.RpgMVSkippedCodesStat.Count > 0)
            {
                AppData.RpgMVAddedCodesStat = AppData.RpgMVAddedCodesStat.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
                AppData.RpgMVSkippedCodesStat = AppData.RpgMVSkippedCodesStat.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

                foreach (var dict in new Dictionary<string, Dictionary<int, int>>()
                    {
                        {"RPGMakerMV Added codes stats", AppData.RpgMVAddedCodesStat },
                        {"RPGMakerMV Skipped codes stats", AppData.RpgMVSkippedCodesStat }
                    }
                )
                {
                    if (AppData.Settings.THConfigINI.SectionExistsAndNotEmpty(dict.Key))
                    {
                        foreach (var pair in AppData.Settings.THConfigINI.GetSectionKeyValuePairs(dict.Key))
                        {
                            var key = int.Parse(pair.Key);
                            var value = int.Parse(pair.Value);
                            if (!dict.Value.ContainsKey(key))
                            {
                                dict.Value.Add(key, value);
                            }
                            else
                            {
                                dict.Value[key] = dict.Value[key] + value;
                            }
                        }
                    }

                    foreach (var pair in dict.Value)
                    {
                        AppData.Settings.THConfigINI.SetKey(dict.Key, pair.Key + "", pair.Value + "");
                    }
                }

                AppData.Settings.THConfigINI.WriteFile();
            }
        }
    }
}
