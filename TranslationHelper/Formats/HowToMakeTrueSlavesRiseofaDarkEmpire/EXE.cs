using System;
using System.Collections.Generic;
using System.IO;
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

            long startpos = 1720080;
            long endpos = 2689376;

            List<byte> translatedbytes=null;//new file content
            if (thDataWork.SaveFileMode)
                translatedbytes = new List<byte>();//init only for translation

            using (FileStream fs = new FileStream(thDataWork.SPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs, Encoding.GetEncoding(932)))
            {
                byte currentbyte;
                var zerobytes = new List<byte>();
                var ffbytes = new List<byte>();
                var candidate = new List<byte>();
                var readstring = true;
                var BaseStreamLength = br.BaseStream.Length;
                var ffbytesForSave = new List<byte>();

                if (thDataWork.OpenFileMode)
                {
                    br.BaseStream.Position = startpos;//go to strings part
                }
                else
                {
                    //add bytes before start position
                    while (br.BaseStream.Position < startpos)
                    {
                        translatedbytes.Add(br.ReadByte());
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
                            var info = "Orig bytes length(" + Encoding.GetEncoding(932).GetByteCount(str) + ")" + "\r\n" + "Zero bytes length after" + " (" + zerobytes.Count + ") " + "\r\n" + "Max bytes length" + " (" + maxbytes + ")";

                            //addrow here if valid
                            if (maxbytes > 20)//skip all candidates spam
                            {
                                if (thDataWork.OpenFileMode)
                                {
                                    AddRowData(str, info, true, true);
                                }
                                else
                                {
                                    if (IsValidString(str) && TablesLinesDict.ContainsKey(str))
                                    {
                                        str = TablesLinesDict[str];
                                    }
                                }
                            }

                            if (thDataWork.SaveFileMode)
                            {
                                foreach (var b in ffbytesForSave)// add pre FF bytes if exist
                                    translatedbytes.Add(b);

                                var strBytes = Encoding.GetEncoding(932).GetBytes(str);
                                foreach (var b in strBytes)//add main string bytes
                                    translatedbytes.Add(b);

                                var newmax = maxbytes - strBytes.Length;//calculate new zero count for rest memory bytes size
                                for (int b = 0; b < newmax; b++)//add zero bytes
                                    translatedbytes.Add(0xff);
                            }

                            //clear lists
                            candidate.Clear();
                            zerobytes.Clear();
                            if (thDataWork.SaveFileMode)
                            {
                                if (ffbytes.Count > 0)
                                {
                                    ffbytesForSave = ffbytes;//set presnexttring FF for write with next string
                                    ffbytes.Clear();
                                }
                                else
                                {
                                    ffbytesForSave.Clear();
                                }
                            }

                            //start to add new
                            candidate.Add(currentbyte);
                            readstring = true;
                        }
                        else //we count here rest of zero bytes and pre FF bytes
                        {
                            if (currentbyte == 0)
                            {
                                zerobytes.Add(currentbyte);
                            }
                            else if (thDataWork.SaveFileMode && currentbyte == 0xff) // only need for save mode
                            {
                                ffbytes.Add(currentbyte);//save FF bytes for next string to write
                            }
                        }
                    }
                }

                if (thDataWork.SaveFileMode)
                {
                    //add rest bytes of the stream
                    while (BaseStreamLength > br.BaseStream.Position)
                    {
                        translatedbytes.Add(br.ReadByte());
                    }
                }
            }

            if (thDataWork.OpenFileMode)
            {
                return CheckTablesContent(ParseData.tablename);
            }
            else
            {
                File.WriteAllBytes(thDataWork.FilePath, translatedbytes.ToArray());

                return true;
            }
        }

        internal override bool Save()
        {
            return OpenSaveEXE();
        }
    }
}
