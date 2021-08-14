using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class EXE : FormatBase
    {
        public EXE()
        {
        }

        internal override string Ext()
        {
            return ".exe";
        }

        internal override bool Open()
        {
            return OpenSaveEXE();
        }

        private bool OpenSaveEXE()
        {
            if (string.IsNullOrWhiteSpace(ProjectData.FilePath) || !File.Exists(ProjectData.FilePath) || Path.GetExtension(ProjectData.FilePath) != ".exe")
            {
                return false;
            }

            ParseStringFilePreOpen();

            long startpos = 1720080;
            long endpos = 6803572;

            List<byte> NewEXEBytesForWrite = null;//new file content
            var translatedbytesControlPosition = 0;
            if (ProjectData.SaveFileMode)
            {
                NewEXEBytesForWrite = new List<byte>();//init only for translation
            }

            var filetranslated = false;

            using (var fs = new FileStream(ProjectData.SelectedFilePath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs, Encoding.GetEncoding(932)))
            {
                byte currentbyte;
                var zeroffbytesAfter = new List<byte>();
                var ffbytesBefore = new List<byte>();
                var ffbytesAfter = new List<byte>();
                //var ffbytesAfterMode = false; ;
                var candidate = new List<byte>();
                var readstring = true;
                var BaseStreamLength = br.BaseStream.Length;
                var ffbytesBeforeForSave = new List<byte>();
                var strtranslated = false;

                if (ProjectData.OpenFileMode)
                {
                    br.BaseStream.Position = startpos;//go to strings part
                }
                else
                {
                    //add bytes before start position
                    while (br.BaseStream.Position < startpos)
                    {
                        NewEXEBytesForWrite.Add(br.ReadByte());
                        translatedbytesControlPosition++;
                    }
                }

                while (br.BaseStream.Position <= endpos)
                {
                    currentbyte = br.ReadByte();

                    if (readstring)
                    {
                        if (currentbyte == 0 || currentbyte == 0xFF) //string end and need to count zero bytes after this
                        {
                            readstring = false;
                            //ffbytesAfterMode = false;
                            zeroffbytesAfter.Add(currentbyte);
                        }
                        //else if (currentbyte == 0xFF)
                        //{
                        //    readstring = false;
                        //    ffbytesAfterMode = true;
                        //    ffbytesAfter.Add(currentbyte);
                        //}
                        else
                        {
                            candidate.Add(currentbyte);
                        }
                    }
                    else
                    {
                        if (currentbyte == 0 || currentbyte == 0xff) //we count here rest of zero ff bytes
                        {
                            //if (currentbyte == 0 || currentbyte == 0xff)
                            {
                                //ffbytesAfterMode = false;
                                zeroffbytesAfter.Add(currentbyte);
                            }
                            //else if (ffbytesAfterMode && currentbyte == 0xff) // in ffbytesAfterMode add ff bytes after string for write
                            //{
                            //    ffbytesAfter.Add(currentbyte);
                            //}
                            //else if (ProjectData.SaveFileMode && currentbyte == 0xff) // only need for save mode
                            //{
                            //    ffbytesBefore.Add(currentbyte);//save FF bytes for next string to write
                            //}
                        }
                        else//parse string
                        {
                            var str = Encoding.GetEncoding(932).GetString(candidate.ToArray());
                            var oldstr = "";
                            var maxbytes = candidate.Count/* + ffbytesAfter.Count*/ + zeroffbytesAfter.Count;
                            var strhaveotherbytes = false;
                            int otherbytescount = 0;

                            //>>check if string is valid and add row or check for translation if found
                            if (maxbytes > 20)//skip all candidates spam
                            {
                                if (maxbytes > 256 && maxbytes < 300)
                                {
                                    oldstr = str;//remember old string
                                    str = GetCorrectString(str);
                                    strhaveotherbytes = str.Length < oldstr.Length;
                                    if (strhaveotherbytes)//calculate other bytes
                                    {
                                        otherbytescount = candidate.Count - Encoding.GetEncoding(932).GetBytes(str).Length;
                                    }
                                }

                                if (ProjectData.OpenFileMode)
                                {
                                    //recalculate maxbytes
                                    if (strhaveotherbytes)
                                    {
                                        maxbytes -= otherbytescount;//exclude other from string area
                                    }

                                    var info = "Orig bytes length(" + Encoding.GetEncoding(932).GetByteCount(str) + ")" + "\r\n" + "Zero bytes length after" + " (" + (/*ffbytesAfter.Count + */zeroffbytesAfter.Count) + ") " + "\r\n" + "Max bytes length" + " (" + maxbytes + ")";
                                    AddRowData(str, info, true, true);
                                }
                                else//save mode
                                {
                                    if (IsValidString(str) && TablesLinesDict.ContainsKey(str))
                                    {
                                        str = TablesLinesDict[str];
                                        strtranslated = true;
                                        filetranslated = true;
                                    }
                                }
                            }
                            //<<--check string

                            //write all bytes-->>
                            if (ProjectData.SaveFileMode && strtranslated)
                            {
                                var lastwritepos = br.BaseStream.Position - 1; //-1 because 1st byte of next string already read in currentbyte

                                //if (translatedbytesControlPosition < pos - 1)// -1 here for make one zero on the end of block
                                //{
                                //    foreach (var _ in ffbytesBeforeForSave)// add pre FF bytes if exist
                                //    {
                                //        NewEXEBytesForWrite.Add(0xff);
                                //        translatedbytesControlPosition++;
                                //        if (translatedbytesControlPosition == pos - 1)//remove all bytes which over of maxbytes limit
                                //        {
                                //            break;
                                //        }
                                //    }
                                //}

                                //add other skipped bytes if they exist
                                if (strhaveotherbytes)
                                {
                                    foreach (var b in candidate)
                                    {
                                        NewEXEBytesForWrite.Add(b);
                                        translatedbytesControlPosition++;
                                        if (translatedbytesControlPosition == lastwritepos)//remove all bytes which over of maxbytes limit
                                        {
                                            break;
                                        }

                                        otherbytescount--;

                                        if (otherbytescount == 0)
                                            break;
                                    }
                                }

                                //add translation bytes
                                var translatonControlPos = (lastwritepos)/* - ffbytesAfter.Count*/;//recalculate end position for translation depend on ffbytes after count
                                if (translatedbytesControlPosition < translatonControlPos)
                                {
                                    var strBytes = Encoding.GetEncoding(932).GetBytes(str);
                                    foreach (var b in strBytes)//add main string bytes
                                    {
                                        NewEXEBytesForWrite.Add(b);
                                        translatedbytesControlPosition++;
                                        if (translatedbytesControlPosition == translatonControlPos)//remove all bytes which over of maxbytes limit
                                        {
                                            break;
                                        }
                                    }
                                }

                                //FF bytes after translation
                                //if (translatedbytesControlPosition < pos - 1)// -1 here for make one zero on the end of block
                                //{
                                //    foreach (var _ in ffbytesAfter)// add pre FF bytes if exist
                                //    {
                                //        NewEXEBytesForWrite.Add(0xff);
                                //        translatedbytesControlPosition++;
                                //        if (translatedbytesControlPosition == pos - 1)//remove all bytes which over of maxbytes limit
                                //        {
                                //            break;
                                //        }
                                //    }
                                //}

                                //while (translatedbytesControlPosition < pos)//add new zero bytes count after string, add only before stream position
                                //{
                                //    NewEXEBytesForWrite.Add(0);
                                //    translatedbytesControlPosition++;
                                //}

                                //add saved ff and zero bytes
                                if (translatedbytesControlPosition < lastwritepos)
                                {
                                    foreach (var b in zeroffbytesAfter)// add zero and FF bytes after
                                    {
                                        NewEXEBytesForWrite.Add(b);
                                        translatedbytesControlPosition++;
                                        if (translatedbytesControlPosition == lastwritepos)//remove all bytes which over of maxbytes limit
                                        {
                                            break;
                                        }
                                    }
                                }

                                //if lastwritepos still not reached then write last byte of zeroffbytesAfter while lastwritepos will be reached 
                                if (translatedbytesControlPosition < lastwritepos)
                                {
                                    var lastbyte = zeroffbytesAfter[zeroffbytesAfter.Count-1];

                                    while(translatedbytesControlPosition < lastwritepos)
                                    {
                                        NewEXEBytesForWrite.Add(lastbyte);
                                        translatedbytesControlPosition++;
                                    }
                                }
                            }
                            else if (ProjectData.SaveFileMode)
                            {
                                foreach (var b in candidate)//add main string bytes form candidate itself
                                {
                                    NewEXEBytesForWrite.Add(b);
                                    translatedbytesControlPosition++;
                                }
                                foreach (var b in zeroffbytesAfter)// add zero FF bytes
                                {
                                    NewEXEBytesForWrite.Add(b);
                                    translatedbytesControlPosition++;
                                }
                            }
                            //<<--write bytes

                            //reset-->>
                            candidate.Clear();
                            zeroffbytesAfter.Clear();
                            strtranslated = false;
                            //ffbytesAfter.Clear();
                            //ffbytesAfterMode = false;
                            //if (ProjectData.SaveFileMode)
                            //{
                            //    if (ffbytesBefore.Count > 0)
                            //    {
                            //        ffbytesBeforeForSave = ffbytesBefore;//set prenextstring FF for write with next string
                            //    }
                            //    else
                            //    {
                            //        ffbytesBeforeForSave.Clear();
                            //    }
                            //    ffbytesBefore.Clear();
                            //}
                            //<<--reset

                            //check if end position reached
                            if (br.BaseStream.Position >= endpos)
                            {

                                if (ProjectData.SaveFileMode)
                                {
                                    //add currentbyte to translation because it was already read but cyrcle will over
                                    NewEXEBytesForWrite.Add(currentbyte);
                                    translatedbytesControlPosition++;
                                }

                                break;
                            }

                            if (translatedbytesControlPosition != br.BaseStream.Position)
                            {

                            }

                            //start to add new string byte for which was already read and it is not 0x00 and not 0xFF
                            candidate.Add(currentbyte);
                            readstring = true;
                        }
                    }
                }

                if (ProjectData.SaveFileMode)
                {
                    //add rest bytes of the stream
                    while (BaseStreamLength > br.BaseStream.Position)
                    {
                        NewEXEBytesForWrite.Add(br.ReadByte());
                        translatedbytesControlPosition++;
                    }
                }
            }

            if (ProjectData.OpenFileMode)
            {
                return CheckTablesContent(ParseData.tablename);
            }
            else
            {
                if (filetranslated)
                {
                    File.WriteAllBytes(ProjectData.FilePath, NewEXEBytesForWrite.ToArray());
                }

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
