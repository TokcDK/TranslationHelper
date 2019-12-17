using System.IO;
using System.Text;
using TranslationHelper.Main;

namespace TranslationHelper
{
    public class IniFile
    {
        //Материал: https://habr.com/ru/post/271483/

        readonly string Path; //Имя файла.

        // С помощью конструктора записываем пусть до файла и его имя.
        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString();
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string ReadINI(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            NativeMethods.GetPrivateProfileString(Section, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }
        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void WriteINI(string Section, string Key, string Value)
        {
            NativeMethods.WritePrivateProfileString(Section, Key, Value, Path);
        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Key, string Section = null)
        {
            WriteINI(Section, Key, null);
        }
        //Удаляем выбранную секцию
        public void DeleteSection(string Section = null)
        {
            WriteINI(Section, null, null);
        }
        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Key, string Section = null)
        {
            //MessageBox.Show("key length="+ ReadINI(Section, Key).Length);
            return ReadINI(Section, Key).Length > 0;
        }
    }
}
