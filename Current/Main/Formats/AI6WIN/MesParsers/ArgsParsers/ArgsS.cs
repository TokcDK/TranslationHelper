using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MesScriptDissAssTest
{
    internal class ArgsS : IArgsParser
    {
        public List<char> Instances => new List<char>() { 'S', 's' };

        public virtual object Get(object[] args)
        {
            BinaryReader inFile = args[0] as BinaryReader;
            int mode = Convert.ToInt32(args[2]);
            string encoding = args[3] as string;

            int length = 0;
            byte[] stringBytes = new byte[0];
            byte currentByte = inFile.ReadByte();

            while (currentByte != 0x00)
            {
                Array.Resize(ref stringBytes, stringBytes.Length + 1);
                stringBytes[length] = currentByte;
                length++;
                currentByte = inFile.ReadByte();
            }

            if (mode == 0x0A)
            {
                string hexString = BitConverter.ToString(stringBytes).Replace("-", "");
                StringBuilder resultString = new StringBuilder();
                int i = 0;

                while (i < hexString.Length)
                {
                    int number = Convert.ToInt32(hexString.Substring(i, 2), 16);

                    if (number < 0x81)
                    {
                        int zlo = number - 0x7D62;
                        int high = (zlo & 0xff00) >> 8;
                        int low = zlo & 0xff;
                        resultString.Append((char)high);
                        resultString.Append((char)low);
                        i += 2;
                    }
                    else
                    {
                        int high = Convert.ToInt32(hexString.Substring(i, 2), 16);
                        resultString.Append((char)high);

                        if ((i + 2) < hexString.Length)
                        {
                            i += 2;
                            int low = Convert.ToInt32(hexString.Substring(i, 2), 16);
                            resultString.Append((char)low);
                        }
                        i += 2;
                    }
                }

                try
                {
                    return resultString.ToString();
                }
                catch (DecoderFallbackException)
                {
                    Console.WriteLine("Decode error: " + BitConverter.ToString(stringBytes));
                    return BitConverter.ToString(stringBytes);
                }
            }
            else if (mode == 0x33 || mode == 0x0B)
            {
                try
                {
                    return Encoding.GetEncoding(encoding).GetString(stringBytes);
                }
                catch (DecoderFallbackException)
                {
                    Console.WriteLine("Decode error: " + BitConverter.ToString(stringBytes));
                    return BitConverter.ToString(stringBytes);
                }
            }
            else
            {
                return Encoding.GetEncoding(encoding).GetString(stringBytes);
            }
        }

        public byte[] Set(object[] args)
        {
            string encoding = args[2] as string;
            string arguments = args[0] as string;

            byte[] argBytes = Encoding.GetEncoding(encoding).GetBytes(arguments + '\x00');
            return argBytes;
        }
    }

}