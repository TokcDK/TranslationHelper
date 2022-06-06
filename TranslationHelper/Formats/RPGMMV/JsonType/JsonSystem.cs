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

            int count = data.ArmorTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.ArmorTypes[i];
                if (AddRowData(ref s, $"ArmorType {i}") && ProjectData.SaveFileMode) data.ArmorTypes[i] = s;
            }

            var u = data.CurrencyUnit;
            if (AddRowData(ref u, $"CurrencyUnit") && ProjectData.SaveFileMode) data.CurrencyUnit = u;

            count = data.Elements.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Elements[i];
                if (AddRowData(ref s, $"Element {i}") && ProjectData.SaveFileMode) data.Elements[i] = s;
            }

            count = data.EquipTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.EquipTypes[i];
                if (AddRowData(ref s, $"EquipType {i}") && ProjectData.SaveFileMode) data.EquipTypes[i] = s;
            }

            count = data.SkillTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.SkillTypes[i];
                if (AddRowData(ref s, $"SkillType {i}") && ProjectData.SaveFileMode) data.SkillTypes[i] = s;
            }

            count = data.Switches.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Switches[i];
                if (AddRowData(ref s, $"Switch {i}") && ProjectData.SaveFileMode) data.Switches[i] = s;
            }

            count = data.Terms.Basic.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Terms.Basic[i];
                if (AddRowData(ref s, $"Terms Basic {i}") && ProjectData.SaveFileMode) data.Terms.Basic[i] = s;
            }

            count = data.Terms.Commands.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Terms.Commands[i];
                if (AddRowData(ref s, $"Terms Command {i}") && ProjectData.SaveFileMode) data.Terms.Commands[i] = s;
            }

            count = data.Terms.Params.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Terms.Params[i];
                if (AddRowData(ref s, $"Terms Param {i}") && ProjectData.SaveFileMode) data.Terms.Params[i] = s;
            }

            var keys = new List<string>(data.Terms.Messages.Keys);
            foreach(var key in keys)
            {
                var s = data.Terms.Messages[key];
                if (AddRowData(ref s, $"Terms Message\r\nName {key}") && ProjectData.SaveFileMode) data.Terms.Messages[key] = s;
            }

            u = data.Title1Name;
            if (AddRowData(ref u, $"Title1Name") && ProjectData.SaveFileMode) data.Title1Name = u;
            u = data.Title2Name;
            if (AddRowData(ref u, $"Title1Name") && ProjectData.SaveFileMode) data.Title2Name = u;
            //u = data.TitleBgm.Name; // bgm file name
            //if (AddRowData(ref u, $"TitleBgm.Name") && ProjectData.SaveFileMode) data.TitleBgm.Name = u;

            count = data.Variables.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.Variables[i];
                if (AddRowData(ref s, $"Variable {i}") && ProjectData.SaveFileMode) data.Variables[i] = s;
            }

            count = data.WeaponTypes.Length;
            for (int i = 0; i < count; i++)
            {
                var s = data.WeaponTypes[i];
                if (AddRowData(ref s, $"WeaponType {i}") && ProjectData.SaveFileMode) data.WeaponTypes[i] = s;
            }

            return data;
        }
    }
}
