using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MesScriptDissAssTest
{
    internal class ArgsSAI6 : ArgsS
    {
        public override object Get(object[] args)
        {
            BinaryReader inFile = args[0] as BinaryReader;
            string encoding = args[3] as string;
            //int mode = Convert.ToInt32(args[2]);

            // Read string from file and decode it.
            List<byte> stringBytes = new List<byte>();
            byte byteRead = inFile.ReadByte();
            while (byteRead != 0)
            {
                stringBytes.Add(byteRead);
                byteRead = inFile.ReadByte();
            }

            string resultString = Encoding.GetEncoding(encoding).GetString(stringBytes.ToArray());
            return resultString;
        }
    }

}