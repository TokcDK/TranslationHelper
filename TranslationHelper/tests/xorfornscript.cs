using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.tests
{
    static class Xorfornscript
    {
        static string dat = Path.Combine("K:\\xgames\\Renalith Saga", "nscript.dat");

        internal static void DecryptXor()
        {
            var nscripttxt = System.Text.Encoding.GetEncoding(932).GetString(File.ReadAllBytes(dat).XorUnxor())/*.Replace("\n", Environment.NewLine)*/;

            File.WriteAllText(dat + ".OpenSaveTest.txt", nscripttxt, System.Text.Encoding.GetEncoding(932));
        }
        internal static void EncryptXor()
        {
            var filecontent = File.ReadAllText(dat + ".OpenSaveTest.txt",Encoding.GetEncoding(932));
            File.Move(dat + ".OpenSaveTest.txt", dat + ".OpenSaveTestOLD.txt");
            //File.WriteAllText(dat + ".OpenSaveTest.txt", filecontent, System.Text.Encoding.GetEncoding(932));
            //var nscriptdat = System.Text.Encoding.GetEncoding(932).GetString(System.Text.Encoding.GetEncoding(932).GetBytes(filecontent/*.Replace(Environment.NewLine, "\n")*/).XorUnxor());
            var nscriptdatbytes = System.Text.Encoding.GetEncoding(932).GetBytes(filecontent.Replace(Environment.NewLine, "\n")).XorUnxor();
            File.WriteAllBytes(dat, nscriptdatbytes);
        }

        //https://stackoverflow.com/questions/22152900/wrong-xor-decryption
        /// <summary>
        /// Added mainly for nscript project
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static byte[] XorUnxor(this byte[] text, byte[] key = null)
        {
            key = key ?? new byte[] { 0x84 };//nscript.dat key

            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte)(text[i] ^ key[i % key.Length]);
            }
            return xor;
        }
    }
}
