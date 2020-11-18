using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer3.Services
{
    public interface IOsDialogService
    {
        string[] ShowFileDialog(string defaultPath);
    }

    public class OsDialogService : IOsDialogService
    {
        public string[] ShowFileDialog(string defaultPath)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var fileDialogResult = openFileDialog.ShowDialog();
            if (fileDialogResult.HasValue && fileDialogResult.Value)
            {
                return openFileDialog.FileNames;
            }
            return new string[0];
        }
    }
}
