using IniParser;
using IniParser.Model;
using System.Globalization;
using System.IO;
using System.Text;
using TranslationHelper.ExternalAdditions;

namespace TranslationHelper
{
    public class IniFile
    {
        //Материал: https://habr.com/ru/post/271483/

        readonly string Path; //Имя файла.
        private readonly FileIniDataParser INIParser;
        private readonly IniData INIData;
        bool ActionWasExecuted = false;

        // С помощью конструктора записываем пусть до файла и его имя.
        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString(CultureInfo.InvariantCulture);
            INIParser = new FileIniDataParser();
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, string.Empty);
            }
            INIData = INIParser.ReadFile(Path);

            //Path = new FileInfo(IniPath).FullName.ToString(CultureInfo.InvariantCulture);
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string ReadINI(string Section, string Key)
        {
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

            //var RetVal = new StringBuilder(255);
            //_=NativeMethods.GetPrivateProfileString(Section, Key, "", RetVal, 255, Path);
            //return RetVal.ToString();
        }
        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void WriteINI(string Section, string Key, string Value, bool DoSaveINI = true)
        {
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
                INIParser.WriteFile(Path, INIData);
            }

            //NativeMethods.WritePrivateProfileString(Section, Key, Value, Path);
        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Key, string Section = null, bool DoSaveINI = true)
        {
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
                INIParser.WriteFile(Path, INIData);
            }

            //WriteINI(Section, Key, null);
        }
        //Удаляем выбранную секцию
        public void DeleteSection(string Section = null, bool DoSaveINI = true)
        {
            if (INIData.Sections.ContainsSection(Section))
            {
                INIData.Sections.RemoveSection(Section);
                ActionWasExecuted = true;
            }
            if (DoSaveINI && ActionWasExecuted)
            {
                INIParser.WriteFile(Path, INIData);
            }

            //WriteINI(Section, null, null);
        }
        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Key, string Section = null)
        {
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

            //MessageBox.Show("key length="+ ReadINI(Section, Key).Length);
            //return ReadINI(Section, Key).Length > 0;
        }
    }
}
