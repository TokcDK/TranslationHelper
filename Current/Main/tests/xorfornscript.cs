using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TranslationHelper.tests
{
    static class Xorfornscript
    {
        internal static void DecryptXor()
        {
            using (var f = new OpenFileDialog())
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                if (string.Equals(Path.GetExtension(f.FileName), ".dat"
                    , StringComparison.InvariantCultureIgnoreCase)) return;

                var nscripttxt = Encoding.GetEncoding(932).GetString(File.ReadAllBytes(f.FileName).XorUnxor())/*.Replace("\n", Environment.NewLine)*/;

                File.WriteAllText(f.FileName + ".OpenSaveTest.txt", nscripttxt, Encoding.GetEncoding(932));
            }
        }
        internal static void EncryptXor()
        {
            using (var f = new OpenFileDialog())
            {
                f.Title = "Select nscript.dat.txt";

                if (f.ShowDialog() != DialogResult.OK) return;
                if (string.Equals(Path.GetExtension(f.FileName), ".txt"
                    , StringComparison.InvariantCultureIgnoreCase)) return;

                var filecontent = File.ReadAllText(f.FileName, Encoding.GetEncoding(932));
                var nscriptdatbytes = Encoding.GetEncoding(932).GetBytes(filecontent.Replace(Environment.NewLine, "\n")).XorUnxor();
                File.WriteAllBytes(f.FileName+".new.dat", nscriptdatbytes);
            }
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
