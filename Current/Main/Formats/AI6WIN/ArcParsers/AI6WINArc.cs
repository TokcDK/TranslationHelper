using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MesArcToolCSharp
{
    internal class AI6WINArc : SilkyArc
    {
        private const int BytesForName = 260;

        public AI6WINArc(string arc, string dir, bool verbose = true, bool integrityCheck = false)
            : base(arc, dir, verbose, integrityCheck)
        {
        }

        // Unpacking methods.

        // using in UnpackNames
        protected override int GetNameLen(BinaryReader inputFile) => BytesForName;
        protected override bool CanGetFiles(BinaryReader inputFile, ref uint limit) => 0 < (limit--);

        // Packing methods.

        // using in PackNamesAndFiles 
        protected override int InitSum => 4;
        protected override object GetEncriptedNameLength(int mod) => null;
        protected override int GetSumMod1(int mod) => 272;
        protected override int GetSumMod2(int mod = 0) => 0;
        protected override int GetHeadLen(int mod1, int mod2) => mod2;

        //using in PackFiles
        protected override void WriteNameLength(BinaryWriter writer, List<object> i)
        { /* name length is always 260 here */ }

        // Other technical methods.

        // using by encrypt and decrypt
        protected override byte[] PreProcessBytes(byte[] bytes) => RstripNullBytesFast(bytes);
        protected override int InitK => 1;
        protected override int GetK(int k) => k;
        protected override byte[] GetRetBytes(byte[] bytes) => ResizeArray(bytes, BytesForName);

        private static byte[] RstripNullBytesFast(byte[] bytes)
        {
            int length = Array.FindLastIndex(bytes, b => b != 0) + 1;
            byte[] trimmedBytes = new byte[length];
            Array.Copy(bytes, trimmedBytes, length);
            return trimmedBytes;
        }
        private static byte[] ResizeArray(byte[] fileBytes, int newSize = 260)
        {
            if (fileBytes.Length == newSize) return fileBytes;

            Array.Resize(ref fileBytes, newSize);

            return fileBytes;
        }
    }
}
