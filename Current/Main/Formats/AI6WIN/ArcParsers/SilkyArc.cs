using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MesArcToolCSharp
{
    internal class SilkyArc
    {
        protected const string NameEncoding = "shift_jis";

        protected readonly string _arcName;
        protected readonly string _dirName;
        protected readonly bool _verbose;
        protected readonly bool _integrityCheck;

        protected List<List<object>> names;

        public SilkyArc(string arc, string dir, bool verbose = true, bool integrityCheck = false)
        {
            _arcName = arc;
            _dirName = dir;
            _verbose = verbose;
            _integrityCheck = integrityCheck;
            names = new List<List<object>>();
        }

        public void Unpack()
        {
            if (_verbose)
            {
                Console.WriteLine($"=== === === UNPACKING OF {_arcName} STARTS! === === ===");
            }

            names = UnpackNames();

            if (_verbose)
            {
                Console.WriteLine($"=== Header of {_arcName} unpacked! ===");
            }

            UnpackFiles();

            if (_verbose)
            {
                Console.WriteLine($"=== Files of {_arcName} unpacked! ===");
                Console.WriteLine($"=== === === UNPACKING OF {_arcName} ENDS! === === ===");
            }
        }

        public void Pack()
        {
            if (_verbose)
            {
                Console.WriteLine($"=== === === PACKING OF {_arcName} STARTS! === === ===");
            }

            var (headLen, names, tempFile) = PackNamesAndFiles();

            if (_verbose)
            {
                Console.WriteLine($"=== Data of {_arcName} initialized! ===");
            }

            BakTarget(_arcName);

            PackFiles(headLen, names, tempFile);

            if (_verbose)
            {
                Console.WriteLine($"=== Archive {_arcName} successfully compiled! ===");
                Console.WriteLine($"=== === === PACKING OF {_arcName} ENDS! === === ===");
            }
        }

        private void BakTarget(string arcName)
        {
            try
            {
                if (File.Exists(_arcName))
                {
                    if (!File.Exists($"{_arcName}.bak"))
                    {
                        File.Move(_arcName, $"{_arcName}.bak");
                    }
                    else
                    {
                        int i = 1;
                        while (File.Exists($"{_arcName}.bak{i++}")) { }

                        File.Move(_arcName, $"{_arcName}.bak{i}");
                    }
                }
            }
            catch (IOException)
            {
                // Handle the exception if necessary
            }
        }

        public static byte[] LzssCompress(byte[] data)
        {
            var dec = new SilkyLZSS(data);
            var newBytes = dec.Encode();
            return newBytes;
        }

        public static byte[] LzssDecompress(byte[] data)
        {
            var dec = new SilkyLZSS(data);
            var newBytes = dec.Decode();
            return newBytes;
        }

        protected virtual uint ReadHeader(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        // using in UnpackNames
        protected virtual int GetNameLen(BinaryReader inputFile) => inputFile.ReadByte();
        protected virtual bool CanGetFiles(BinaryReader inputFile, ref uint limit) => inputFile.BaseStream.Position < limit;
        protected virtual List<List<object>> UnpackNames()
        {
            using (var inputFile = new BinaryReader(File.OpenRead(_arcName)))
            {
                var limit = ReadHeader(inputFile);
                var arrayName = new List<List<object>>();

                while (CanGetFiles(inputFile, ref limit))
                {
                    int nameLen = GetNameLen(inputFile);
                    var prms = new List<object>
                    {
                        nameLen,
                        DecryptName(inputFile.ReadBytes(nameLen)),
                        ReadUInt32BigEndian(inputFile),
                        ReadUInt32BigEndian(inputFile),
                        ReadUInt32BigEndian(inputFile)
                    };

                    arrayName.Add(prms);
                }

                return arrayName;
            }
        }
        protected uint ReadUInt32BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            Array.Reverse(bytes); // Reverse the byte order to convert from big-endian to little-endian
            return BitConverter.ToUInt32(bytes, 0);
        }

        private void UnpackFiles()
        {
            Directory.CreateDirectory(_dirName);

            using (var inputFile = new BinaryReader(File.OpenRead(_arcName)))
            {
                foreach (var i in names)
                {
                    var name = (string)i[1];
                    var size = Convert.ToInt32(i[2]);
                    var size2 = Convert.ToInt32(i[3]);
                    var pos = Convert.ToInt32(i[4]);

                    var thisFileName = Path.Combine(_dirName, name);
                    inputFile.BaseStream.Seek(pos, SeekOrigin.Begin);
                    var newFileBytes = inputFile.ReadBytes(size);

                    if (_integrityCheck && newFileBytes.Length != size)
                    {
                        Console.WriteLine($"!!! File {name} compressed size is incorrect!");
                    }

                    if (size != size2)
                    {
                        newFileBytes = LzssDecompress(newFileBytes);

                        if (_integrityCheck && newFileBytes.Length != size2)
                        {
                            Console.WriteLine($"!!! File {name} true size is incorrect!");
                        }
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(thisFileName));

                    File.WriteAllBytes(thisFileName, newFileBytes);

                    if (_verbose)
                    {
                        Console.WriteLine($"> File {name} successfully unpacked!");
                    }
                }
            }
        }

        // using in PackNamesAndFiles 
        protected virtual int InitSum => 0;
        protected virtual object GetEncriptedNameLength(int mod) => mod;
        protected virtual int GetSumMod1(int mod) => mod + 13;
        protected virtual int GetSumMod2(int mod = 0) => 4;
        protected virtual int GetHeadLen(int mod1, int mod2) => mod1;
        protected virtual (int, List<List<object>>, FileStream) PackNamesAndFiles()
        {
            var names = new List<List<object>>();
            var sum = InitSum;

            var tempFile = new FileStream(Path.GetTempFileName(), FileMode.Create);

            foreach (var filePath in Directory.GetFiles(_dirName, "*", SearchOption.AllDirectories))
            {
                var nameArray = new List<object>();

                var realName = Path.GetFileName(filePath);

                var encryptedName = EncryptName(realName);

                var contentBytes = File.ReadAllBytes(filePath);
                var encryptedBytes = LzssCompress(contentBytes);

                //File.WriteAllBytes(filePath + ".tmp", encryptedBytes);
                tempFile.Write(encryptedBytes, 0, encryptedBytes.Length);

                nameArray.Add(GetEncriptedNameLength(encryptedName.Length));
                nameArray.Add(encryptedName);
                nameArray.Add(encryptedBytes.Length);
                nameArray.Add(contentBytes.Length);

                names.Add(nameArray);

                sum += GetSumMod1(encryptedName.Length);

                if (_verbose)
                {
                    Console.WriteLine($"> File {realName} successfully managed!");
                }
            }

            var headLen = GetHeadLen(sum, names.Count);
            sum += GetSumMod2();

            // add offsets of files encrypted content
            foreach (var name in names)
            {
                name.Add(sum); // add offset for encrypted content
                sum += (int)name[2]; // recalc offset by the encrypted file content
            }

            if (_verbose)
            {
                Console.WriteLine(">>> File offsets successfully calculated!");
            }

            return (headLen, names, tempFile);
        }

        //using in PackFiles
        protected virtual void WriteNameLength(BinaryWriter writer, List<object> i)
        {
            writer.Write((byte)i[0]); // name length
        }
        protected virtual void PackFiles(int headLen, List<List<object>> names, FileStream tempFile)
        {
            using (var newArchive = new FileStream(_arcName, FileMode.Create))
            using (var writer = new BinaryWriter(newArchive, Encoding.Default, true))
            {
                writer.Write(headLen);

                foreach (var i in names)
                {
                    WriteNameLength(writer, i);
                    var b = (byte[])i[1];
                    writer.Write(b); // name

                    // size and position
                    for (var j = 2; j < 5; j++)
                    {
                        b = GetBigEndianBytes((int)(i[j]));
                        writer.Write(b);
                    }
                }

                if (_verbose)
                {
                    Console.WriteLine(">>> Archive header successfully created!");
                }

                tempFile.Seek(0, SeekOrigin.Begin);

                foreach (var i in names)
                {
                    var newBytes = new byte[(int)i[2]];
                    tempFile.Read(newBytes, 0, newBytes.Length);

                    if (_integrityCheck && newBytes.Length != (int)i[2])
                    {
                        Console.WriteLine($"!!! File {DecryptName((byte[])i[1])} compressed size is incorrect!");
                    }
                    writer.Write(newBytes, 0, newBytes.Length);
                }

                if (_verbose)
                {
                    Console.WriteLine(">>> Archive files data successfully packed!");
                }
            }
        }

        protected byte[] GetBigEndianBytes(int input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        protected virtual string DecryptName(byte[] fileBytes) 
            => GetRetStr(DecEncBytes(PreProcessBytes(fileBytes)));
        protected virtual byte[] PreProcessBytes(byte[] bytes) => bytes;
        protected virtual int InitK => 0;
        protected virtual int GetK(int k) => -k;
        protected virtual string GetRetStr(byte[] bytes) => Encoding.GetEncoding(NameEncoding).GetString(bytes);
        protected virtual byte[] GetRetBytes(byte[] bytes) => bytes;
        protected virtual byte[] EncryptName(string fileName)
        {
            var fileBytes = Encoding.GetEncoding(NameEncoding).GetBytes(fileName);            
            return GetRetBytes(DecEncBytes(fileBytes, true));
        }
        protected byte[] DecEncBytes(byte[] fileBytes, bool encrypt = false)
        {
            int len = fileBytes.Length;
            var newFileBytes = new byte[len];
            int k = InitK;

            for (var i = len - 1; i >= 0; i--)
            {
                k++;
                newFileBytes[i] = (byte)(fileBytes[i] 
                    + (encrypt ? GetK(k) : -GetK(k)));
            }
            newFileBytes.Reverse();

            return newFileBytes;
        }
    }
}
