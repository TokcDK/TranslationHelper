using System;
using System.Collections.Generic;
using System.IO;

namespace MesScriptDissAssTest
{
    internal class ArgsH : IArgsParser
    {
        public List<char> Instances => new List<char>() { 'H', 'h' };

        public object Get(object[] args)
        {
            BinaryReader fileIn = args[0] as BinaryReader;
            //string definer = args[0] as string;

            byte[] dummyBytes = fileIn.ReadBytes(2);
            Array.Reverse(dummyBytes); // Reverse the byte order to interpret as big-endian (from little-endian by default)
            var dummy = BitConverter.ToInt16(dummyBytes, 0);

            return dummy;
        }

        public byte[] Set(object[] args)
        {
            int arguments = Convert.ToInt32(args[0]);
            //string command = args[1] as string;

            var bytes = BitConverter.GetBytes((short)arguments);
            Array.Reverse(bytes); // Reverse the byte order to interpret as little-endian (from big-endian)
            return bytes;
        }
    }

}