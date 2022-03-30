using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using OpenConstructionSet.Data;
using OpenConstructionSet.Installations;
using OpenConstructionSet.Mods;
using OpenConstructionSet.Mods.Context;
using System.Linq;
using System;
using System.Collections.Specialized;

namespace TranslationHelper.Formats.LOFI.Kenshi
{
    abstract class MOD : FormatBinaryBase
    {
        public MOD()
        {
        }

        internal override string Ext()
        {
            return ".mod";
        }

        protected override void ParseBytes()
        {
            Parse();

            base.ParseBytes();
        }

        private async void Parse()
        {
            //ModFile referenceMod = new ModFile(ProjectData.SelectedFilePath);

            //ModFileData referenceData;

            //try
            //{
            //    referenceData = await referenceMod.ReadDataAsync();
            //}
            //catch (Exception ex)
            //{
            //    return;
            //    //Error($"Unable to load {ReferenceModName}{Environment.NewLine}Error: {ex}");
            //    //return (0, 0, 0);
            //}

            //var items = new List<string>();
            //foreach (var item in referenceData.Items)
            //{
            //    items.Add(item.Name);

            //    foreach (var n in item.Values)
            //    {
            //        if (!(n.Value is string s)) continue;
            //        if (n.Key != "description" && !n.Key.StartsWith("text")) continue;
            //        if (string.IsNullOrWhiteSpace(s)) continue;

            //        items.Add(s);
            //    }
            //}

            //var iii = items;
        }

        ///// <summary>
        ///// start position in exe from where to start parse.
        ///// Set it to offset of first byte of string.
        ///// </summary>
        //protected abstract long StartPos { get; }
        ///// <summary>
        ///// end position of the exe where to stop parse.
        ///// set it to first not zero and not ff byte offset where scan for strings must stop
        ///// </summary>
        //protected abstract long EndPos { get; }

        //bool readstring;
        //protected override bool FilePreOpenActions()
        //{
        //    var pre = new byte[] { 0x74, 0x65, 0x78, 0x74, 0x30 };
        //    var post = new byte[] { 0x6C, 0x69, 0x6E, 0x65, 0x73 };
        //    TextPrePostBytes.Add();
        //    return base.FilePreOpenActions();
        //}

        //protected override void PreParseBytes()
        //{
        //    readstring = true;
        //}

        //protected override bool ParseBytesReadCondition()
        //{
        //    return ParseData.FStream.Position <= EndPos;
        //}

        //List<byte> searchbytes = new List<byte>();
        //Dictionary<byte[], byte[]> TextPrePostBytes = new Dictionary<byte[], byte[]>();

        //protected override KeywordActionAfter ParseByte()
        //{
        //    if (readstring)
        //    {
        //    }
        //    else
        //    {
        //    }

        //    return KeywordActionAfter.Continue;
        //}
    }
}
