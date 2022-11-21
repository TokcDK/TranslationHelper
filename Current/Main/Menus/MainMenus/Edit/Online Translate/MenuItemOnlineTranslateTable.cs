using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus.FileRowMenus;

namespace TranslationHelper.Menus.MainMenus.Edit.OnlineTranslate
{
    internal class MenuItemOnlineTranslateTable : MainMenuFileSubItemOnlineTranslateBase, IFileRowMenuItem
    {
        public override string Text => T._("Table");

        public override string Description => T._("Translate table rows");

        public override void OnClick(object sender, EventArgs e)
        {
            var trans = new Thread((obj) => new OnlineTranslateNew().Table());
            //
            //..и фикс ошибки:
            //System.TypeInitializationException: Инициализатор типа "TranslationHelper.GoogleAPI" выдал исключение. ---> System.Threading.ThreadStateException: Создание экземпляра элемента управления ActiveX '8856f961-340a-11d0-a96b-00c04fd705a2' невозможно: текущий поток не находится в однопоточном контейнере
            //https://ru.stackoverflow.com/questions/412073/c-webbrowser-threadstateexception-%D0%9E%D0%B4%D0%BD%D0%BE%D0%BF%D0%BE%D1%82%D0%BE%D1%87%D0%BD%D1%8B%D0%B9-%D0%BA%D0%BE%D0%BD%D1%82%D0%B5%D0%B9%D0%BD%D0%B5%D1%80
            trans.SetApartmentState(ApartmentState.STA);
            //но при выборе только одной строчки почему-то кидает исключение
            trans.Start();
        }
    }
}
