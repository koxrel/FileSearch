using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearch
{
    class NotepadProcess
    {
        private string _fileName;

        public NotepadProcess(string fileName)
        {
            _fileName = fileName;
        }

        public void StartProcess()
        {
            Process p = new Process {StartInfo = {FileName = "notepad.exe"}};

            p.StartInfo.Arguments = _fileName;
            p.Start();
        }
    }
}
