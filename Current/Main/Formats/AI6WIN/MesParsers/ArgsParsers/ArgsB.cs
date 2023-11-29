using System;
using System.Collections.Generic;
using System.IO;

namespace MesScriptDissAssTest
{
    internal class ArgsB : IArgsParser
    {
        public List<char> Instances => new List<char>() { 'B', 'b' };

        public object Get(object[] args)
        {
            BinaryReader fileIn = args[0] as BinaryReader;
            //string definer = args[0] as string;

            //byte[] dummyBytes = fileIn.ReadBytes(1);
            //Array.Reverse(dummyBytes); // Reverse the byte order to interpret as big-endian (from little-endian by default)
            //var dummy = BitConverter.ToInt32(dummyBytes, 0);
            return fileIn.ReadByte();
        }

        public byte[] Set(object[] args)
        {
            int arguments = Convert.ToInt32(args[0]);
            //string command = args[1] as string;

            var bytes = new byte[] { (byte)arguments }; // here is single byte
            return bytes;
        }
    }

}