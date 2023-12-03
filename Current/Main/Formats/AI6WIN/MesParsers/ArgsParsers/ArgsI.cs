using System;
using System.Collections.Generic;
using System.IO;

namespace MesScriptDissAss
{
    internal class ArgsI : IArgsParser
    {
        public List<char> Instances => new List<char>() { 'I', 'i' };

        public object Get(object[] args)
        {
            BinaryReader fileIn = args[0] as BinaryReader;
            //string definer = args[0] as string;

            byte[] dummyBytes = fileIn.ReadBytes(4);
            Array.Reverse(dummyBytes); // Reverse the byte order to interpret as big-endian (from little-endian by default)
            var dummy = BitConverter.ToInt32(dummyBytes, 0);

            return dummy;
        }

        public byte[] Set(object[] args)
        {
            int arguments = Convert.ToInt32(args[0]);
            //string command = args[1] as string;

            byte[] bytes = BitConverter.GetBytes(arguments);
            Array.Reverse(bytes);// Reverse the byte order to interpret as little-endian (from big-endian)
            return bytes;
        }
    }

}