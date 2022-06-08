using RPGMVJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonSystem:JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadSystem(path);

            bool isSave = AppData.SaveFileMode;

            int count = data.ArmorTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.ArmorTypes[i];
                if (AddRowData(ref s, isSave ? "" : $"ArmorType {i}") && isSave) data.ArmorTypes[i] = s;
            }

            var u = data.CurrencyUnit;
            if (AddRowData(ref u, isSave ? "" : $"CurrencyUnit") && isSave) data.CurrencyUnit = u;

            count = data.Elements.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Elements[i];
                if (AddRowData(ref s, isSave ? "" : $"Element {i}") && isSave) data.Elements[i] = s;
            }

            count = data.EquipTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.EquipTypes[i];
                if (AddRowData(ref s, isSave ? "" : $"EquipType {i}") && isSave) data.EquipTypes[i] = s;
            }

            count = data.SkillTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.SkillTypes[i];
                if (AddRowData(ref s, isSave ? "" : $"SkillType {i}") && isSave) data.SkillTypes[i] = s;
            }

            count = data.Switches.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Switches[i];
                if (AddRowData(ref s, isSave ? "" : $"Switch {i}") && isSave) data.Switches[i] = s;
            }

            count = data.Terms.Basic.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Terms.Basic[i];
                if (AddRowData(ref s, isSave ? "" : $"Terms Basic {i}") && isSave) data.Terms.Basic[i] = s;
            }

            count = data.Terms.Commands.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Terms.Commands[i];
                if (AddRowData(ref s, isSave ? "" : $"Terms Command {i}") && isSave) data.Terms.Commands[i] = s;
            }

            count = data.Terms.Params.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Terms.Params[i];
                if (AddRowData(ref s, isSave ? "" : $"Terms Param {i}") && isSave) data.Terms.Params[i] = s;
            }

            var keys = new List<string>(data.Terms.Messages.Keys);
            foreach(var key in keys)
            {
                var s = data.Terms.Messages[key];
                if (AddRowData(ref s, isSave ? "" : $"Terms Message\r\nName {key}") && isSave) data.Terms.Messages[key] = s;
            }

            u = data.Title1Name;
            if (AddRowData(ref u, isSave ? "" : $"Title1Name") && isSave) data.Title1Name = u;
            u = data.Title2Name;
            if (AddRowData(ref u, isSave ? "" : $"Title1Name") && isSave) data.Title2Name = u;
            //u = data.TitleBgm.Name; // bgm file name
            //if (AddRowData(ref u, isSave ? "" : $"TitleBgm.Name") && ProjectData.SaveFileMode) data.TitleBgm.Name = u;

            count = data.Variables.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Variables[i];
                if (AddRowData(ref s, isSave ? "" : $"Variable {i}") && isSave) data.Variables[i] = s;
            }

            count = data.WeaponTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.WeaponTypes[i];
                if (AddRowData(ref s, isSave ? "" : $"WeaponType {i}") && isSave) data.WeaponTypes[i] = s;
            }

            return data;
        }
    }
}
