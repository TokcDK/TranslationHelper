using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

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
            long endpos = 6803572;

            List<byte> translatedbytes = null;//new file content
            var translatedbytesControlPosition = 0;
            if (thDataWork.SaveFileMode)
            {
                translatedbytes = new List<byte>();//init only for translation
            }

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
                        translatedbytesControlPosition++;
                    }
                }

                while (br.BaseStream.Position <= endpos)
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
                            var oldstr = "";
                            var maxbytes = candidate.Count + zerobytes.Count;
                            var info = "Orig bytes length(" + Encoding.GetEncoding(932).GetByteCount(str) + ")" + "\r\n" + "Zero bytes length after" + " (" + zerobytes.Count + ") " + "\r\n" + "Max bytes length" + " (" + maxbytes + ")";

                            //addrow here if valid
                            if (maxbytes > 20)//skip all candidates spam
                            {
                                if (maxbytes > 256 && maxbytes < 300)
                                {
                                    oldstr = str;//remember old string
                                    str = GetCorrectString(str);
                                }

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
                                var pos = br.BaseStream.Position - 1; //-1 because 1st byte of next string already read in currentbyte

                                //calculate other bytes if oldstr is longer of str
                                int otherbytescount = 0;
                                if (str.Length < oldstr.Length)
                                {
                                    var strBytes = Encoding.GetEncoding(932).GetBytes(str);
                                    otherbytescount = candidate.Count - strBytes.Length;
                                }

                                if (translatedbytesControlPosition < pos - 1)// -1 here for make one zero on the end of block
                                {
                                    foreach (var b in ffbytesForSave)// add pre FF bytes if exist
                                    {
                                        translatedbytes.Add(b);
                                        translatedbytesControlPosition++;
                                        if (translatedbytesControlPosition == pos - 1)//remove all bytes which over of maxbytes limit
                                        {
                                            break;
                                        }
                                    }
                                }

                                //add other skipped bytes if they exist
                                if (otherbytescount > 0)
                                {
                                    foreach(var b in candidate)
                                    {
                                        translatedbytes.Add(b);
                                        translatedbytesControlPosition++;
                                        if (translatedbytesControlPosition == pos - 1)//remove all bytes which over of maxbytes limit
                                        {
                                            break;
                                        }

                                        otherbytescount--;

                                        if (otherbytescount == 0)
                                            break;
                                    }
                                }

                                if (translatedbytesControlPosition < pos - 1)
                                {
                                    var strBytes = Encoding.GetEncoding(932).GetBytes(str);
                                    foreach (var b in strBytes)//add main string bytes
                                    {
                                        translatedbytes.Add(b);
                                        translatedbytesControlPosition++;
                                        if (translatedbytesControlPosition == pos - 1)//remove all bytes which over of maxbytes limit
                                        {
                                            break;
                                        }
                                    }
                                }

                                while (translatedbytesControlPosition < pos)//add new zero bytes count after string, add only before stream position
                                {
                                    translatedbytes.Add(0);
                                    translatedbytesControlPosition++;
                                }
                            }

                            //clear lists
                            candidate.Clear();
                            zerobytes.Clear();
                            if (thDataWork.SaveFileMode)
                            {
                                if (ffbytes.Count > 0)
                                {
                                    ffbytesForSave = ffbytes;//set prenextstring FF for write with next string
                                }
                                else
                                {
                                    ffbytesForSave.Clear();
                                }
                                ffbytes.Clear();
                            }

                            if (br.BaseStream.Position >= endpos)
                            {

                                if (thDataWork.SaveFileMode)
                                {
                                    //add currentbyte to translation because it was already read but cyrcle will over
                                    translatedbytes.Add(currentbyte);
                                    translatedbytesControlPosition++;
                                }

                                break;
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
                        translatedbytesControlPosition++;
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

        /// <summary>
        /// remove all non kanji/katakana/hiragana symbold and also remove '・' because it can be on start of string but it is not part of string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string GetCorrectString(string str)
        {
            int ind = -1;
            foreach (var c in str)
            {
                ind++;
                if (c != '・' && c.ToString().IsJPChar())
                {
                    return str.Substring(ind);
                }
                else
                {
                    continue;
                }
            }

            return str;
        }

        internal override bool Save()
        {
            return OpenSaveEXE();
        }
    }
}
