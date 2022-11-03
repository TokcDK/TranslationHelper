using System.Collections.Generic;
using RPGMVJsonParser;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class ArrayInfo
    {
        internal readonly string[] Array;
        internal readonly string Info;

        public ArrayInfo(string[] array, string info)
        {
            Array = array;
            Info = info;
        }
    }

    internal class JsonSystem : JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadSystem(path);

            bool isSave = SaveFileMode;

            var u = data.GameTitle;
            if (AddRowData(ref u, isSave ? "" : $"GameTitle") && isSave) data.GameTitle = u;
            u = data.CurrencyUnit;
            if (AddRowData(ref u, isSave ? "" : $"CurrencyUnit") && isSave) data.CurrencyUnit = u;

            foreach(var arrayInfo in new ArrayInfo[] 
            {
                new ArrayInfo(data.ArmorTypes, "ArmorType"),
                new ArrayInfo(data.Elements, "Element"),
                new ArrayInfo(data.EquipTypes, "EquipType"),
                new ArrayInfo(data.SkillTypes, "SkillType"),
                new ArrayInfo(data.Switches, "Switch"),
                new ArrayInfo(data.Terms.Basic, "Terms Basic"),
                new ArrayInfo(data.Terms.Commands, "Terms Command"),
                new ArrayInfo(data.Terms.Params, "Terms Param"),
                new ArrayInfo(data.Variables, "Variable"),
                new ArrayInfo(data.WeaponTypes, "WeaponType"),
            })
            {
                CheckArray(arrayInfo.Array, isSave ? "" : arrayInfo.Info, isSave);
            }

            var keys = new List<string>(data.Terms.Messages.Keys);
            foreach (var key in keys)
            {
                var s = data.Terms.Messages[key];
                if (AddRowData(ref s, isSave ? "" : $"Terms Message\r\nName {key}") && isSave) data.Terms.Messages[key] = s;
            }

            u = data.Title1Name;
            if (AddRowData(ref u, isSave ? "" : $"Title1Name") && isSave) data.Title1Name = u;
            u = data.Title2Name;
            if (AddRowData(ref u, isSave ? "" : $"Title1Name") && isSave) data.Title2Name = u;

            return data;
        }

        private void CheckArray(string[] array, string info, bool isSave)
        {
            var count = array.Length;
            for (int i = 0; i < count; i++)
            {
                var s = array[i];
                if (AddRowData(ref s, $"{info} {i}") && isSave) array[i] = s;
            }
        }
    }
}
