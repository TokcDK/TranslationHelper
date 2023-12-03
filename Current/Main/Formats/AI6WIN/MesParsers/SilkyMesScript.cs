using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TranslationHelper.Formats.Other;

namespace MesScriptDissAss
{
    // C# realisation of https://github.com/TesterTesterov/AI6WINScriptTool

    internal class SilkyMesScript
    {
        protected int Version { get; } = 0;

        internal int DefaultVersion { get; } = 0;

        readonly char[] TechnicalInstances = new char[] { '>', '<' };

        internal virtual Dictionary<int, IVersionData> SupportedVersions { get; } = new Dictionary<int, IVersionData>
        {
            { 0, new SilkyMesScriptDefault() },
        };

        internal virtual List<IArgsParser> ArgsParsers { get; } = new List<IArgsParser>()
        {
            new ArgsB(),
            new ArgsH(),
            new ArgsI(),
            new ArgsS(),
        };

        protected readonly string _mesName;
        protected readonly bool _hackermanMode;
        protected readonly string _encoding;
        protected readonly bool _verbose;
        protected readonly bool _debug;
        protected readonly string _txtName;
        protected int[] _prm;
        protected List<List<int>> _offsets;
        protected List<int> _offsetsD;
        protected List<int> _firstOffsets;
        protected List<int> _secondOffsets;

        protected const string DEFAULT_ENCODING = "shift_jis";

        internal SilkyMesScript(string mesName, string txtName, string encoding = "", int version = 1, bool debug = false, bool verbose = false, bool hackermanMode = false)
        {
            if (!SupportedVersions.ContainsKey(version)) throw new Exception($"Unsupported version {version}!");
            Version = version;
            CommandLibrary = SupportedVersions[version].CommandLibrary;

            _verbose = verbose;
            _encoding = string.IsNullOrEmpty(encoding) ? DEFAULT_ENCODING : encoding;
            _hackermanMode = hackermanMode;
            _debug = debug;
            _mesName = mesName;
            _txtName = txtName;
            _prm = new int[2];
            _offsets = new List<List<int>>();
            _firstOffsets = new List<int>();
            _secondOffsets = new List<int>();
        }

        // [Opcode, struct, name].
        protected (byte Opcode, string Struct, string Name)[] CommandLibrary;
        Dictionary<string, (byte Opcode, string Struct, string Name)> _commandsByName; // assembly only
        Dictionary<byte, (byte Opcode, string Struct, string Name)> _commandsByOpcode; // assembly only

        protected virtual (byte Offset, int I)[] OffsetsLibrary { get; } = new (byte, int)[]
        {
        (0x14, 0),
        (0x15, 0),
        (0x16, 0),
        (0x1b, 0)
        };

        protected byte[] _inputBytes = default;

        internal virtual void Disassemble(byte[] inputBytes = default)
        {
            _inputBytes = inputBytes;

            _offsetsD = new List<int>();
            (_prm, _firstOffsets, _secondOffsets) = DissHeader();

            DissOtherOffsets();

            DisassembleCommands();
        }

        internal virtual void Assemble()
        {
            _commandsByName = CommandLibrary.Where(d => d.Name.Length > 0).ToDictionary(k => k.Name, v => v);
            _commandsByOpcode = CommandLibrary.Where(d => d.Name.Length > 0).ToDictionary(k => k.Opcode, v => v);

            (_prm, _firstOffsets, _secondOffsets, _offsets) = AssembleOffsetsAndParameters();

            AssembleScriptFile();
        }

        // Technical methods for assembling.

        protected virtual void AssembleScriptFile()
        {
            //TemplateFile = File.ReadAllBytes("M:\\DesktopW10\\AI6WINScriptTool-main\\mes\\ending_xx_template.mes");

            //using (StreamReader inFile = new StreamReader(_txtName, Encoding.GetEncoding(_encoding)))
            {
                //try
                //{
                //    var bak = _mesName + ".bak";
                //    if (!File.Exists(bak) && File.Exists(_mesName)) File.Move(_mesName, bak);
                //}
                //catch (IOException)
                //{
                //    // Handle IOException
                //}

                using (BinaryWriter outFile = new BinaryWriter(new FileStream(_mesName, FileMode.Create)))
                {
                    int messageCount = 0;
                    var searchOffset = _offsets.Select(offset => offset[0]).ToList();

                    WriteOffsets(outFile);

                    //string line;
                    //while ((line = inFile.ReadLine()) != null)
                    for (int i = 0; i < DisassembledTxt.Count; i++)
                    {
                        string line = DisassembledTxt[i];

                        if (!IsValid(line)) continue;

                        switch (line[1])
                        {
                            case '0':
                                var b2w = BitConverter.GetBytes(int.Parse(line.Substring(2), System.Globalization.NumberStyles.HexNumber));
                                Write(outFile, b2w);
                                break;
                            case '1':
                                {
                                    string commandString = line.Substring(3);

                                    var (cOpcode, cStruct, _) = GetCommandLibraryData(commandString);

                                    Write(outFile, cOpcode);

                                    //line = inFile.ReadLine();
                                    line = DisassembledTxt[++i];
                                    var argumentList = JsonConvert.DeserializeObject<List<object>>(line);

                                    int thisCommand = cOpcode;
                                    int offsetSet = -1;

                                    if (thisCommand == 0x19)
                                    {
                                        argumentList[0] = messageCount;
                                        messageCount++;
                                    }
                                    else
                                    {
                                        foreach (var (Offset, I) in OffsetsLibrary)
                                        {
                                            if (thisCommand == Offset)
                                            {
                                                offsetSet = I;
                                                break;
                                            }
                                        }
                                    }

                                    if (offsetSet != -1)
                                    {
                                        int indexer = searchOffset.IndexOf(Convert.ToInt32(argumentList[offsetSet]));
                                        argumentList[offsetSet] = _offsets[indexer][1];
                                    }

                                    //if (addCnt > 1640)
                                    //{
                                    //}
                                    byte[] argumentBytes = SetArgs(argumentList, cStruct, _encoding);
                                    Write(outFile, argumentBytes);
                                    break;
                                }
                        }
                    }
                }
            }
        }

        //List<byte> OutFile = new List<byte>();
        //byte[] TemplateFile = null;
        //int addCnt = 0;
        protected void Write(BinaryWriter outFile, byte[] b2w)
        {
            //addCnt++;
            outFile.Write(b2w);
            //OutFile.AddRange(b2w);
            //if (!CompareLastBytesOfByteArrays(OutFile, TemplateFile, b2w.Length))
            //{
            //}
        }
        protected void Write(BinaryWriter outFile, byte b2w)
        {
            //addCnt++;
            outFile.Write(b2w);
            //OutFile.Add(b2w);
            //if (!CompareLastBytesOfByteArrays(OutFile, TemplateFile, 1))
            //{
            //}
        }

        protected bool IsValid(string line)
        {
            return !string.IsNullOrEmpty(line)// EOF.
                && line.Length > 1 // To evade some nasty errors.
                && !(line == "\n" || line[0] == '$'); // Line without text or comment should not be parsed as script.
        }

        protected virtual void WriteOffsets(BinaryWriter outFile)
        {
            foreach (int parameter in _prm)
            {
                Write(outFile, BitConverter.GetBytes(parameter));
            }

            foreach (int firstOffset in _firstOffsets)
            {
                Write(outFile, BitConverter.GetBytes(firstOffset));
            }

            foreach (int secondOffset in _secondOffsets)
            {
                Write(outFile, BitConverter.GetBytes(secondOffset));
            }
        }

        protected (int[] prm, List<int> firstOffsets, List<int> secondOffsets, List<List<int>> offsets) AssembleOffsetsAndParameters()
        {
            //using (StreamReader inFile = new StreamReader(_txtName, Encoding.GetEncoding(_encoding)))
            {
                List<int> firstOffsets = new List<int>();
                List<int> secondOffsets = new List<int>();
                bool isSecondOffsetsUsed = false;
                List<List<int>> offsets = new List<List<int>>();
                int[] prm = new int[2];
                int pointer = 0;
                int messageCount = 0;

                //while ((line = inFile.ReadLine()) != null)
                for (int i = 0; i < DisassembledTxt.Count; i++)
                {
                    string line = DisassembledTxt[i];

                    if (!IsValid(line)) continue;

                    // Actually code strings logic.
                    switch (line[1]) // "Free bytes".
                    {
                        case '0':
                            pointer += line.Substring(2).Split(' ').Length;
                            break;
                        case '1':
                            {
                                string commandString = line.Substring(3);
                                var (cOpcode, cStruct, _) = GetCommandLibraryData(commandString);

                                bool is19Opcode;
                                if (is19Opcode = cOpcode == 0x19) // Since header save offsets to messages.
                                {
                                    messageCount++;
                                    firstOffsets.Add(pointer);
                                }

                                pointer++;

                                // Okay, now is the time for getting arguments length!
                                //string nextLine = inFile.ReadLine();
                                string nextLine = DisassembledTxt[++i];
                                List<object> argumentList = JsonConvert.DeserializeObject<List<object>>(nextLine);

                                if (is19Opcode) // For this to not cause any errors.
                                {
                                    argumentList[0] = 0;
                                }

                                byte[] argumentBytes = SetArgs(argumentList, cStruct, _encoding);
                                pointer += argumentBytes.Length;
                                break;
                            }

                        case '2':
                            {
                                var offsetArray = new List<int>();

                                int offsetNumber = int.Parse(line.Substring(3));
                                offsetArray.Add(offsetNumber);
                                offsetArray.Add(pointer);

                                offsets.Add(offsetArray);
                                break;
                            }

                        case '3':
                            secondOffsets.Add(pointer);
                            isSecondOffsetsUsed = true;
                            break;
                    }
                }

                prm[0] = messageCount;
                if (isSecondOffsetsUsed) prm[1] = secondOffsets.Count;

                return (prm, firstOffsets, secondOffsets, offsets);
            }
        }

        private (byte Opcode, string Struct, string Name) GetCommandLibraryData(string commandString)
        {
            // Check if it is written by name.
            if (!TryGetCommandLibraryData(commandString, out (byte Opcode, string Struct, string Name) commandLibraryData))
            {
                // There is no such command (text). But this should be impossible!
                throw new Exception($"Error! There is no such command.\n{commandString}");
            }
            else return commandLibraryData;
        }

        private bool TryGetCommandLibraryData(string commandString, out (byte Opcode, string Struct, string Name) outCommandLibraryData)
        {
            if (!_commandsByName.TryGetValue(commandString, out (byte Opcode, string Struct, string Name) commandLibraryData)
            && !(int.TryParse(commandString, System.Globalization.NumberStyles.HexNumber, null, out int commandHex) // Check if it is written by hex.
            && _commandsByOpcode.TryGetValue((byte)commandHex, out commandLibraryData))
            )
            {
                outCommandLibraryData = default;
                return false;
            }

            outCommandLibraryData = commandLibraryData;
            return true;
        }

        protected int GetTrueOffset(int rawOffset)
        {
            return rawOffset + _prm.Sum(value => value * 4) + 4;
        }

        protected int SetTrueOffset(int rawOffset)
        {
            return rawOffset - _prm.Sum(value => value * 4) - 4;
        }

        internal List<string> Strings = new List<string>();
        internal List<string> DisassembledTxt = new List<string>();
        protected void DisassembleCommands()
        {
            //List<int> commands = new List<int>();
            List<List<object>> args = new List<List<object>>();
            // [Opcode, struct, name].
            int pointer = GetTrueOffset(0);
            string stringer = "";
            List<int> theseIndices = new List<int>();

            //using (StreamWriter outFile = new StreamWriter(_txtName, false, Encoding.GetEncoding(_encoding)))
            {
                using (BinaryReader inFile = new BinaryReader(GetInputStream()))
                {
                    inFile.BaseStream.Seek(pointer, SeekOrigin.Begin);

                    List<(int, int)> sortedOffset = _offsetsD.Select((x, i) => (i, x)).OrderBy(x => x.Item2).ToList();
                    // Sorted by offsets, but with index saving.
                    List<int> searchOffset = sortedOffset.Select(x => x.Item2).ToList();
                    var initialSortedOffset = new List<(int, int)>(sortedOffset);
                    var initialSearchOffset = new List<int>(searchOffset);
                    // I know, you may say it's pointless, but that's for the sake of optimization.

                    List<int> secondOffsets = _secondOffsets.Select(offset => GetTrueOffset(offset)).ToList();

                    while (true)
                    {
                        pointer = (int)inFile.BaseStream.Position;  // To get current position before the command.

                        // Offsets functionality.
                        // I did try my best to optimize it. It may be looked as bad, but...
                        // I have managed to drastically decrease the number of iterations.
                        // From some hundreds to about 1-2.
                        theseIndices.Clear();
                        int speedyCrutch = -1;
                        foreach (var (pos, offset) in sortedOffset)
                        {
                            speedyCrutch++;
                            if (pointer == offset)
                            {
                                theseIndices.Add(speedyCrutch);
                                var str = $"#2-{pos}" + (_debug ? $" {pointer}" : "");
                                DisassembledTxt.Add(str);
                                //outFile.WriteLine(str);
                                break;
                            }
                            else if (pointer > offset)
                            {
                                break;
                            }
                        }
                        foreach (var used in theseIndices)
                        {
                            sortedOffset.RemoveAt(used);
                            searchOffset.RemoveAt(used);
                        }
                        foreach (var offset in secondOffsets)  // Should be fine since it is rare and not lengthy.
                        {
                            if (pointer == offset)
                            {
                                var str = "#3" + (_debug ? $" {pointer}" : "");
                                DisassembledTxt.Add(str);
                                //outFile.WriteLine(str);
                                break;
                            }
                        }

                        // Commands functionality.

                        int currentByte;
                        if(inFile.BaseStream.Position >= inFile.BaseStream.Length)
                        {
                            break;
                        } 
                        currentByte = inFile.ReadByte();

                        var argsCurrent = new List<object>();
                        args.Add(argsCurrent);
                        //commands.Add(currentByte);
                        string analyzer = currentByte.ToString("X2");

                        int libIndex = -1;
                        for (int i = 0; i < CommandLibrary.Length; i++)
                        {
                            if (currentByte == CommandLibrary[i].Opcode)
                            {
                                libIndex = i;
                                break;
                            }
                        }

                        if (libIndex != -1)
                        {
                            if (!string.IsNullOrEmpty(stringer))
                            {
                                stringer = stringer.TrimStart(' ');
                                stringer = $"#0-{stringer}\n";
                                DisassembledTxt.Add(stringer);
                                //outFile.Write(stringer);
                                stringer = "";
                            }

                            //outFile.Write("#1-");
                            var str = "#1-";
                            var commandLibraryLibIndex = CommandLibrary[libIndex];
                            var commandLibraryLibIndexName = commandLibraryLibIndex.Name;
                            if (string.IsNullOrEmpty(commandLibraryLibIndexName))
                            {
                                str += analyzer;
                                DisassembledTxt.Add(analyzer);
                                //outFile.Write(analyzer);
                            }
                            else
                            {
                                if (commandLibraryLibIndexName == "STR_CRYPT")
                                {
                                    str += "STR_UNCRYPT";
                                    //DisassembledTxt.Add($"{str}STR_UNCRYPT");
                                    //outFile.Write("STR_UNCRYPT");
                                }
                                else
                                {
                                    str += commandLibraryLibIndexName;
                                    //DisassembledTxt.Add(str+commandLibraryLibIndexName);
                                    //outFile.Write(commandLibraryLibIndexName);
                                }
                            }
                            str += _debug ? $" {pointer}" : "";
                            DisassembledTxt.Add(str);
                            //outFile.WriteLine(str);

                            List<object> argumentsList = GetArgs(inFile, commandLibraryLibIndex.Struct, currentByte, _encoding);

                            int whatIndex = -1;
                            for (int entryPos = 0; entryPos < OffsetsLibrary.Length; entryPos++)
                            {
                                if (currentByte == OffsetsLibrary[entryPos].Offset)
                                {
                                    whatIndex = entryPos;
                                    break;
                                }
                            }

                            if (whatIndex != -1)
                            {
                                int firstIndexer = OffsetsLibrary[whatIndex].I;
                                int evilOffset = GetTrueOffset((int)argumentsList[firstIndexer]);
                                int indexer = initialSearchOffset.IndexOf(evilOffset);
                                argumentsList[firstIndexer] = initialSortedOffset[indexer].Item1;
                            }

                            if (commandLibraryLibIndex.Opcode == 0x19)
                            {
                                argumentsList[0] = "*MESSAGE_NUMBER*";
                            }

                            foreach (var arg in argumentsList)
                            {
                                argsCurrent.Add(arg);
                            }

                            str = JsonConvert.SerializeObject(argumentsList);
                            DisassembledTxt.Add(str);
                            //if (commandLibraryLibIndexName == "STR_PRIMARY")
                            //{
                                //Strings.Add(str);
                            //}
                            //outFile.WriteLine(str);
                        }
                        else
                        {
                            stringer += " " + analyzer;
                            pointer += 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(stringer))  // Print remaining free bytes.
                    {
                        stringer = stringer.TrimStart(' ');
                        stringer = $"#0-{stringer}";
                        DisassembledTxt.Add(stringer);
                        //outFile.Write(stringer);
                    }
                }
            }
        }

        internal void DissOtherOffsets()
        {
            int pointer = GetTrueOffset(0);
            using (Stream inFile = GetInputStream())
            {
                inFile.Seek(pointer, SeekOrigin.Begin);

                StreamWriter outFile = null;

                if (_hackermanMode)
                {
                    outFile = new StreamWriter("HACK.txt", false, Encoding.GetEncoding(_encoding));
                }

                while (true)
                {
                    pointer = (int)inFile.Position;
                    int currentByte = inFile.ReadByte();

                    if (currentByte == -1) break;

                    int libIndex = -1;
                    for (int i = 0; i < CommandLibrary.Length; i++)
                    {
                        if (currentByte == CommandLibrary[i].Opcode)
                        {
                            libIndex = i;
                            break;
                        }
                    }

                    if (libIndex != -1)
                    {
                        List<object> argumentsList = GetArgs(new BinaryReader(inFile), CommandLibrary[libIndex].Struct, currentByte, _encoding);

                        if (_hackermanMode)
                        {
                            //outFile.WriteLine($"#1-{BitConverter.ToString(new byte[] { (byte)currentByte })}    {pointer}");
                            //outFile.WriteLine(string.Join(" ", argumentsList));
                            //outFile.WriteLine();
                        }

                        int whatIndex = -1;
                        for (int entryPos = 0; entryPos < OffsetsLibrary.Length; entryPos++)
                        {
                            if (currentByte == OffsetsLibrary[entryPos].Offset)
                            {
                                whatIndex = entryPos;
                                break;
                            }
                        }

                        if (whatIndex != -1)
                        {
                            bool notHere = true;
                            int goodOffset = GetTrueOffset((int)argumentsList[OffsetsLibrary[whatIndex].I]);

                            for (int i = 0; i < _offsetsD.Count; i++)
                            {
                                if (goodOffset == _offsetsD[i])
                                {
                                    notHere = false;
                                    break;
                                }
                            }

                            if (notHere)
                            {
                                //Array.Resize(ref _offsets, _offsets.Count + 1); // list dont need to resize
                                _offsetsD.Add(goodOffset);
                            }
                        }
                    }
                    else
                    {
                        if (_hackermanMode)
                        {
                            //outFile.WriteLine($"#0-{BitConverter.ToString(new byte[] { (byte)currentByte })}    {pointer}");
                        }
                    }
                }

                if (_hackermanMode)
                {
                    outFile.Close();
                }
            }
        }

        internal (int[] prm, List<int> firstOffsets, List<int> secondOffsets) DissHeader()
        {
            var firstOffsets = new List<int>();
            var secondOffsets = new List<int>();

            int[] prm;
            using (Stream mesFile = GetInputStream())
            {
                using (var binaryReader = new BinaryReader(mesFile))
                {
                    prm = ReadOffsets(binaryReader, firstOffsets, secondOffsets);
                }
            }

            return (prm, firstOffsets, secondOffsets);
        }

        private Stream GetInputStream()
        {
            if (_inputBytes != null && _inputBytes.Length > 0)
            {
                return new MemoryStream(_inputBytes);
            }
            else
            {
                return new FileStream(_mesName, FileMode.Open, FileAccess.Read);
            }
        }

        protected virtual int[] ReadOffsets(BinaryReader binaryReader, List<int> firstOffsets, List<int> secondOffsets)
        {
            int[] prm = new int[2];

            prm[0] = binaryReader.ReadInt32();
            prm[1] = binaryReader.ReadInt32();

            for (int i = 0; i < prm[0]; i++)
                firstOffsets.Add(binaryReader.ReadInt32());

            for (int i = 0; i < prm[1]; i++)
                secondOffsets.Add(binaryReader.ReadInt32());

            return prm;
        }

        internal List<object> GetArgs(BinaryReader inFile, string args, int currentByte, string currentEncoding)
        {
            List<object> argumentsList = new List<object>();
            string appendix = "";

            bool checkTechnicalInstances = true;
            foreach (char argument in args)
            {
                if (checkTechnicalInstances && Array.IndexOf(TechnicalInstances, argument) != -1)
                {
                    checkTechnicalInstances = false;
                    appendix = argument.ToString();
                }
                else
                {
                    foreach (var arg in ArgsParsers)
                    {
                        if (arg.Instances.Contains(argument))
                        {
                            var argData = new object[4] { inFile, appendix + argument, currentByte, currentEncoding };
                            argumentsList.Add(arg.Get(argData));

                            break;
                        }
                    }
                }
            }

            return argumentsList;
        }

        internal byte[] SetArgs(List<object> argumentList, string args, string currentEncoding)
        {
            var argsBytes = new List<byte>();
            string appendix = "";
            int currentArgument = 0;

            bool checkTechnicalInstances = true;
            foreach (char argument in args)
            {
                if (checkTechnicalInstances && Array.IndexOf(TechnicalInstances, argument) != -1)
                {
                    checkTechnicalInstances = false;
                    appendix = argument.ToString();
                }
                else
                {

                    // Argument number changing.

                    foreach (var arg in ArgsParsers)
                    {
                        if (arg.Instances.Contains(argument))
                        {
                            var argData = new object[3] { argumentList[currentArgument], appendix + argument, currentEncoding };

                            argsBytes.AddRange(arg.Set(argData));

                            break;
                        }
                    }

                    currentArgument++;
                }
            }

            return argsBytes.ToArray();
        }
    }

}