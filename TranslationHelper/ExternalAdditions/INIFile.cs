using IniParser;
using IniParser.Model;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AIHelper.Manage
{
    public class INIFile
    {
        //Материал: https://habr.com/ru/post/271483/

        private readonly string Path; //Имя файла.
        private readonly FileIniDataParser INIParser;
        private readonly IniData INIData;
        bool ActionWasExecuted = false;

        //[DllImport("kernel32", CharSet = CharSet.Unicode)] // Подключаем kernel32.dll и описываем его функцию WritePrivateProfilesString
        //static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        //[DllImport("kernel32", CharSet = CharSet.Unicode)] // Еще раз подключаем kernel32.dll, а теперь описываем функцию GetPrivateProfileString
        //static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        //public IniData GetINIData()
        //{
        //    return INIData;
        //}

        // С помощью конструктора записываем путь до файла и его имя.
        public INIFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName;
            INIParser = new FileIniDataParser();
            //if (!File.Exists(Path))
            //{
            //    File.WriteAllText(Path, string.Empty);
            //}
            if (File.Exists(Path))
            {
                INIData = INIParser.ReadFile(Path);
            }
            //else
            //{
            //    throw new FileNotFoundException();
            //}
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string ReadINI(string Section, string Key)
        {
            if (INIData == null)
                return string.Empty;

            if (string.IsNullOrEmpty(Section))
            {
                return INIData.Global[Key];
            }
            else
            {
                if (!INIData.Sections.ContainsSection(Section))
                {
                    return string.Empty;
                }
                return INIData[Section][Key];
            }

            //var ini = ExIni.IniFile.FromFile(Path);
            //var section = ini.GetSection(Section);
            //if (section == null)
            //{
            //    return string.Empty;
            //}
            //else
            //{
            //    var key = section.GetKey(Key);

            //    if (key != null)//ExIni не умеет читать ключи с \\ в имени
            //    {
            //        return key.Value;
            //    }
            //    else
            //    {
            //        var RetVal = new StringBuilder(4096);
            //        GetPrivateProfileString(Section, Key, "", RetVal, 4096, Path);//Это почему-то не может прочитать ключи из секций с определенными названиями
            //        return RetVal.ToString();
            //    }
            //}

        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string[] ReadSectionValuesToArray(string Section)
        {
            if (INIData == null)
                return null;

            if (string.IsNullOrEmpty(Section) || !INIData.Sections.ContainsSection(Section))
            {
                return null;
            }
            else
            {

                using (var sectionKeys = INIData[Section].GetEnumerator())
                {
                    Dictionary<int, string> SearchQueries = new Dictionary<int, string>();

                    for (int i = 0; i < INIData[Section].Count; i++)
                    {
                        sectionKeys.MoveNext();

                        if (sectionKeys.Current.Value.Length > 0)
                        {
                            SearchQueries.Add(i, sectionKeys.Current.Value);
                        }
                    }

                    return SearchQueries.Values.ToArray();
                }
            }
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public void WriteArrayToSectionValues(string Section, string[] Values, bool CleanSectionBeforeWrite = true, bool DoSaveINI = true)
        {
            if (INIData == null || string.IsNullOrEmpty(Section) || Values == null || Values.Length == 0)
                return;

            if (CleanSectionBeforeWrite)
            {
                INIData[Section].RemoveAllKeys();
            }

            for (int i = 0; i < Values.Length; i++)
            {
                INIData[Section][i.ToString(CultureInfo.GetCultureInfo("en-US"))] = Values[i];
            }

            if (DoSaveINI)
            {
                INIParser.WriteFile(Path, INIData, Encoding.UTF8);
            }
        }

        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void WriteINI(string Section, string Key, string Value, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (string.IsNullOrEmpty(Section))
            {
                INIData.Global[Key] = Value;
                ActionWasExecuted = true;
            }
            else
            {
                if (!INIData.Sections.ContainsSection(Section))
                {
                    INIData.Sections.AddSection(Section);
                }
                INIData[Section][Key] = Value;
                ActionWasExecuted = true;
            }

            if (DoSaveINI && ActionWasExecuted)
            {
                INIParser.WriteFile(Path, INIData, Encoding.UTF8);
            }
            //if (!ManageStrings.IsStringAContainsStringB(Key, "\\"))
            //{
            //    var ini = ExIni.IniFile.FromFile(Path);
            //    var section = ini.GetSection(Section);
            //    if (section != null)
            //    {
            //        if (section.HasKey(Key))//ExIni не умеет читать ключи с \\ в имени
            //        {
            //            section.GetKey(Key).Value = Value;
            //            ini.Save(Path);
            //            return;
            //        }
            //        else
            //        {
            //            section.CreateKey(Key);
            //            section.GetKey(Key).Value = Value;
            //            ini.Save(Path);
            //            return;
            //        }
            //    }
            //}
            //WritePrivateProfileString(Section, Key, Value, Path);
        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Key, string Section = null, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (string.IsNullOrEmpty(Section))
            {
                INIData.Global.RemoveKey(Key);
                ActionWasExecuted = true;
            }
            else
            {
                if (INIData.Sections.ContainsSection(Section))
                {
                    INIData[Section].RemoveKey(Key);
                    ActionWasExecuted = true;
                }
            }
            if (DoSaveINI && ActionWasExecuted)
            {
                INIParser.WriteFile(Path, INIData, Encoding.UTF8);
            }
            //var ini = ExIni.IniFile.FromFile(Path);
            //var section = ini.GetSection(Section);
            //if (section != null)
            //{
            //    if (section.HasKey(Key) && !ManageStrings.IsStringAContainsStringB(Key,"\\"))//ExIni не умеет читать ключи с \\ в имени
            //    {
            //        section.DeleteKey(Key);
            //        ini.Save(Path);
            //    }
            //    else
            //    {
            //        WriteINI(Section, Key, null);
            //    }
            //}
        }

        //Удаляем выбранную секцию
        public void DeleteSection(string Section/* = null*/, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (INIData.Sections.ContainsSection(Section))
            {
                INIData.Sections.RemoveSection(Section);
                ActionWasExecuted = true;
            }
            if (DoSaveINI && ActionWasExecuted)
            {
                INIParser.WriteFile(Path, INIData, Encoding.UTF8);
            }
            //var ini = ExIni.IniFile.FromFile(Path);
            //if(Section!=null && ini.HasSection(Section))
            //{
            //    ExIni.IniFile.FromFile(Path).DeleteSection(Section);
            //    ini.Save(Path);
            //}
            //WriteINI(Section, null, null);
        }

        //Очистка выбранной секции
        public void ClearSection(string Section/* = null*/, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (INIData.Sections.ContainsSection(Section))
            {
                INIData.Sections[Section].RemoveAllKeys();
                ActionWasExecuted = true;
            }
            if (DoSaveINI && ActionWasExecuted)
            {
                INIParser.WriteFile(Path, INIData, Encoding.UTF8);
            }
            //var ini = ExIni.IniFile.FromFile(Path);
            //if(Section!=null && ini.HasSection(Section))
            //{
            //    ExIni.IniFile.FromFile(Path).DeleteSection(Section);
            //    ini.Save(Path);
            //}
            //WriteINI(Section, null, null);
        }

        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Key, string Section = null)
        {
            if (INIData == null)
                return false;

            if (string.IsNullOrEmpty(Section))
            {
                return INIData.Global.ContainsKey(Key);
            }
            else
            {
                if (!INIData.Sections.ContainsSection(Section))
                {
                    return false;
                }
                return INIData[Section].ContainsKey(Key);
            }
            //var iniSection = ExIni.IniFile.FromFile(Path).GetSection(Section);
            //if (iniSection == null)
            //{
            //    return false;
            //}
            //else
            //{
            //    return iniSection.HasKey(Key);
            //}
            //MessageBox.Show("key length="+ ReadINI(Section, Key).Length);
            //return ReadINI(Section, Key).Length > 0;
        }
    }
}
