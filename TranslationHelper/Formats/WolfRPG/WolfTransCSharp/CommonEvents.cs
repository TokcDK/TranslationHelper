using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT = WolfTrans.Net;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.CommonEvents;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class CommonEvents:FormatBinaryBase
    {
        protected override void FileOpen()
        {
            var commonEvent = new CommonEventsParser();
            commonEvent.Read(FilePath);

            foreach (var @event in commonEvent.Events)
            {
                var patch_filename = $"dump/common/{@event.ID}_{@event.Name}.txt";

                foreach (var command in @event.Commands)
                {
                    foreach (var @string in Strings_Of_Command(command))
                    {
                        AddRowData(@string);
                    }
                }
            }
        }
        public IEnumerable<string> Strings_Of_Command(Command command)
        {
            if (command is Message m)
            {
                if ( IsTranslatable(m.Text)) yield return m.Text;
            }
            else if (command is Choices || command is StringCondition)
            {
                foreach (var s in command.String_args)
                {
                    if (IsTranslatable(s)) yield return s;
                }
            }
            else if (command is SetString css)
            {
                if (IsTranslatable(css.Text)) yield return css.Text;
            }
            else if (command is Picture pic)
            {
                if (pic.Type() == ":text" && IsTranslatable(pic.Text)) yield return pic.Text;
            }
            else if (command is WT.Parsers.Shared.Database db)
            {
                if (IsTranslatable(db.Text)) yield return db.Text;
            }

            yield break;
        }

        static bool IsTranslatable(string str)
        {
            return !string.IsNullOrEmpty(str) && str != "\u25A0";
        }
    }
}
