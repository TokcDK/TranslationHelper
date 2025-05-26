using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.IO;

namespace TH.WPF.Models
{
    public class MenuCommands
    {
        public static void Open()
        {
            var openDialog = new OpenFileDialog();
            //THFOpen.InitialDirectory = AppData.Settings.THConfigINI.GetKey("Paths", "LastPath");

            //var possibleExtensions = string.Join(";*", GetListOfSubClasses.Inherited.GetListOfinheritedSubClasses<FormatBase>().Where(f => !string.IsNullOrWhiteSpace(f.Ext) && f.Ext[0] == '.').Select(f => f.Ext).Distinct());
            openDialog.Filter = "All|*";

            if (openDialog.ShowDialog() != true || openDialog.FileName == null) return;

            if (string.IsNullOrWhiteSpace(openDialog.FileName) || !File.Exists(openDialog.FileName)) return;

            //bool isProjectFound = false;
            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
            //await Task.Run(() => isProjectFound = GetSourceType(AppData.SelectedFilePath)).ConfigureAwait(true);

        }
    }
}
