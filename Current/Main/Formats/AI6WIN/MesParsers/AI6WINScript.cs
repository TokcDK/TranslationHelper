using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MesScriptDissAss
{
    internal class AI6WINScript : SilkyMesScript
    {
        internal AI6WINScript(string mesName, string txtName, string encoding = "", int version = 1, bool debug = false, bool verbose = false, bool hackermanMode = false) : base(mesName, txtName, encoding, version, debug, verbose, hackermanMode)
        {
        }

        internal override Dictionary<int, IVersionData> SupportedVersions { get; } = new Dictionary<int, IVersionData>
        {
            { 0, new AI6WINScriptMostGames() },
            { 1, new AI6WINScriptAishimai4EarliestGames() },
        };

        protected override (byte Offset, int I)[] OffsetsLibrary { get; } =
        {
            (0x14, 0),
            (0x15, 0),
            (0x16, 0),
            (0x1A, 0)
        };

        internal override List<IArgsParser> ArgsParsers { get; } = new List<IArgsParser>()
        {
            new ArgsB(),
            new ArgsH(),
            new ArgsI(),
            new ArgsSAI6(),
        };

        protected override void WriteOffsets(BinaryWriter outFile)
        {
            Write(outFile, BitConverter.GetBytes(_prm[0]));
            foreach (int firstOffset in _firstOffsets)
            {
                Write(outFile, BitConverter.GetBytes(firstOffset));
            }
        }

        protected override int[] ReadOffsets(BinaryReader binaryReader, List<int> firstOffsets, List<int> secondOffsets)
        {
            int[] prm = new int[1];

            prm[0] = binaryReader.ReadInt32();

            for (int i = 0; i < prm[0]; i++)
                firstOffsets.Add(binaryReader.ReadInt32());

            return prm;
        }
    }
}
