using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class EXE : FormatBase
    {
        public EXE(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return OpenSaveEXE();
        }

        private bool OpenSaveEXE()
        {
            if (string.IsNullOrWhiteSpace(thDataWork.FilePath) || !File.Exists(thDataWork.FilePath) || Path.GetExtension(thDataWork.FilePath) != ".exe")
            {
                return false;
            }

            ParseStringFilePreOpen();

            var startpos = 1720080;
            var endpos = 2689376;
            var l1 = 256;
            var l2 = 1280;
            using (FileStream fs = new FileStream(thDataWork.SPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs, Encoding.GetEncoding(932)))
            {
                byte currentbyte;
                var zerobytes = new List<byte>();
                var ffbytes = new List<byte>();
                var candidate = new List<byte>();
                var readstring = true;
                var translatedbytes = new List<byte>();

                if (thDataWork.OpenFileMode)
                {
                    br.BaseStream.Position = startpos;//go to strings part
                }
                else
                {
                    var startbytes = br.ReadBytes(startpos);
                    foreach (var b in startbytes)
                    {
                        translatedbytes.Add(b);
                    }
                }

                //var first = br.ReadBytes(Encoding.GetEncoding(932).GetBytes("　　極至王プラムちゃん").Length);
                //var str = Encoding.GetEncoding(932).GetString(first);

                //var s = Encoding.GetEncoding(932).GetBytes("");

                while (br.BaseStream.Position < endpos)
                {
                    currentbyte = br.ReadByte();

                    if (readstring)
                    {
                        if (currentbyte == 0) //string end and need to count zero bytes after this
                        {
                            readstring = false;
                            zerobytes.Add(currentbyte);
                        }
                        else
                        {
                            candidate.Add(currentbyte);
                        }
                    }
                    else
                    {
                        if (currentbyte != 0 && currentbyte != 0xFF)
                        {
                            var str = Encoding.GetEncoding(932).GetString(candidate.ToArray());
                            var maxbytes = candidate.Count + zerobytes.Count;
                            var info = "Orig bytes length("+ Encoding.GetEncoding(932).GetByteCount(str) + ")" + "\r\n" + "Zero bytes length after" + " (" + zerobytes.Count + ") " + "\r\n" + "Max bytes length" + " (" + maxbytes + ")";
                            var IsAdded = false;
                            //addrow here if valid
                            if (maxbytes > 20)//skip all candidates spam
                            {
                                if(thDataWork.OpenFileMode)
                                {
                                    AddRowData(str, info, true, true);
                                }
                                else
                                {
                                    if (IsValidString(str) && TablesLinesDict.ContainsKey(str))
                                    {
                                        str = TablesLinesDict[str];
                                        IsAdded = true;
                                    }
                                }
                            }

                            if (thDataWork.SaveFileMode && IsAdded)
                            {
                                foreach (var b in zerobytes)
                                    translatedbytes.Add(b);

                                foreach (var b in ffbytes)
                                    translatedbytes.Add(b);

                                byte[] bs = Encoding.GetEncoding(932).GetBytes(str);
                                foreach (var b in bs)
                                    translatedbytes.Add(b);
                            }

                            //clear lists
                            candidate.Clear();
                            zerobytes.Clear();

                            //start to add new
                            candidate.Add(currentbyte);
                            readstring = true;
                        }
                        else
                        {
                            if(currentbyte == 0)
                            {
                                zerobytes.Add(currentbyte);
                            }
                            else if (currentbyte == 0xff)
                            {
                                ffbytes.Add(currentbyte);
                            }
                        }
                    }
                }
            }

            return CheckTablesContent(ParseData.tablename);
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
