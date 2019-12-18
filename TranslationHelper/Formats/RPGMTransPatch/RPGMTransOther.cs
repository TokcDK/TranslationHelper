﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMTrans
{
    public static class RPGMTransOther
    {
        public static bool CreateRPGMakerTransPatch(string inputdir, string outdir)
        {
            string rpgmakertranscli = Path.Combine(Application.StartupPath, "Res", "rpgmakertrans", "rpgmt.exe");
            bool ret;
            //string projectname = Path.GetFileName(outdir);

            //параметры
            //parser.add_argument("input", help = "Path of input game to patch")
            //parser.add_argument("-p", "--patch", help = "Path of patch (directory or zip)"
            //        "(Defaults to input_directory_patch")
            //parser.add_argument("-o", "--output", help = "Path to output directory "
            //        "(will create) (Defaults to input_directory_translated)")
            //parser.add_argument('-q', '--quiet', help = 'Suppress all output',
            //        action = 'store_true')
            //parser.add_argument('-b', '--use-bom', help = 'Use UTF-8 BOM in Patch'
            //        'files', action = 'store_true')
            //parser.add_argument('-r', '--rebuild', help = "Rebuild patch against game",
            //        action = "store_true")
            //parser.add_argument('-s', '--socket', type = int, default = 27899,
            //        help = 'Socket to use for XP/VX/VX Ace patching'
            //        '(default: 27899)')
            //parser.add_argument('-l', '--dump-labels', action = "store_true",
            //        help = "Dump labels to patch file")
            //parser.add_argument('--dump-scripts', type = str, default = None,
            //        help = "Dump scripts to given directory")
            string rpgmakertranscliargs = "\"" + inputdir + "\" -p \"" + outdir + "_patch\"" + " -o \"" + outdir + "_translated\"";

            ret = FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs);

            if (ret && Directory.Exists(outdir + "_patch\""))
            {
            }
            else
            {
                //чистка папок 
                if (Directory.Exists(outdir + "_patch\""))
                {
                    Directory.Delete(outdir + "_patch", true);
                }
                if (Directory.Exists(outdir + "_translated\""))
                {
                    Directory.Delete(outdir + "_translated", true);
                }

                //попытка с параметром -b - Use UTF-8 BOM in Patch files
                ret = FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs + " -b");
                if (!ret || (ret && !Directory.Exists(outdir + "_patch\"")))
                {
                    string tempDIr = Path.Combine(inputdir, "tempTH");

                    Directory.CreateDirectory(tempDIr);

                    string rgss = RPGMFunctions.GetRPGMakerArc(inputdir);
                    if (rgss.Length == 0)
                    {
                        return false;
                    }

                    rpgmakertranscli = Path.Combine(Application.StartupPath, "Res", "rgssdecryptor", "RgssDecrypter.exe");
                    rpgmakertranscliargs = "\"--output=" + tempDIr + "\" " + rgss;

                    ret = FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs);

                    if (Directory.GetDirectories(tempDIr).Length > 0)
                    {
                        foreach (var dir in Directory.GetDirectories(inputdir))
                        {
                            if (Path.GetFileName(dir) == "tempTH")
                            {
                                continue;
                            }

                            string targetDirPath = dir.Replace(inputdir, tempDIr);
                            if (Directory.Exists(targetDirPath))
                            {
                                foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                                {
                                    string targetFilePath = file.Replace(dir, Path.Combine(tempDIr, Path.GetFileName(file)));
                                    if (File.Exists(targetFilePath))
                                    {
                                        File.Delete(targetFilePath);
                                    }
                                    File.Move(file, targetFilePath);
                                }
                                Directory.Delete(dir, true);
                            }
                            else
                            {
                                Directory.Move(dir, targetDirPath);
                            }
                        }
                        foreach (var dir in Directory.GetDirectories(tempDIr))
                        {
                            Directory.Move(dir, dir.Replace(tempDIr, inputdir));
                        }
                        Directory.Delete(tempDIr, true);

                        if (Directory.GetDirectories(inputdir).Length > 0)
                        {
                            File.Delete(rgss);

                            rpgmakertranscli = Path.Combine(Application.StartupPath, "Res", "rpgmakertrans", "rpgmt.exe");
                            rpgmakertranscliargs = "\"" + inputdir + "\" -p \"" + outdir + "_patch\"" + " -o \"" + outdir + "_translated\"";
                            ret = FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs);
                            if (ret && Directory.Exists(outdir + "_patch\""))
                            {
                            }
                            else
                            {
                                //чистка папок 
                                if (Directory.Exists(outdir + "_patch\""))
                                {
                                    Directory.Delete(outdir + "_patch", true);
                                }
                                if (Directory.Exists(outdir + "_translated\""))
                                {
                                    Directory.Delete(outdir + "_translated", true);
                                }

                                //попытка с параметром -b - Use UTF-8 BOM in Patch files
                                ret = FunctionsProcess.RunProgram(rpgmakertranscli, rpgmakertranscliargs + " -b");
                                if (!ret || (ret && !Directory.Exists(outdir + "_patch\"")))
                                {
                                    //чистка папок 
                                    if (Directory.Exists(outdir + "_patch\""))
                                    {
                                        Directory.Delete(outdir + "_patch", true);
                                    }
                                    if (Directory.Exists(outdir + "_translated\""))
                                    {
                                        Directory.Delete(outdir + "_translated", true);
                                    }
                                    return false;
                                }

                            }
                        }
                    }
                    else
                    {
                        //чистка папок 
                        Directory.Delete(tempDIr, true);
                        if (Directory.Exists(outdir + "_patch\""))
                        {
                            Directory.Delete(outdir + "_patch", true);
                        }
                        if (Directory.Exists(outdir + "_translated\""))
                        {
                            Directory.Delete(outdir + "_translated", true);
                        }
                        return false;
                    }
                }
            }

            return ret;
        }
    }
}