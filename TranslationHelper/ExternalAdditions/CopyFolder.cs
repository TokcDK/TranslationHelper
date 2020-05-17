﻿using System;
using System.IO;

namespace TranslationHelper
{
    //https://code.4noobz.net/c-copy-a-folder-its-content-and-the-subfolders/
    class CopyFolder
    {
        //static void Main(string[] args)
        //{
        //string sourceDirectory = @"C:\temp\source";
        //string targetDirectory = @"C:\temp\destination";

        //Copy(sourceDirectory, targetDirectory);

        //Console.WriteLine("\r\nEnd of program");
        //Console.ReadKey();
        //}

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
