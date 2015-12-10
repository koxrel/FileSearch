using System.Diagnostics;

namespace FileSearch
{
    class NotepadProcess
    {
        private readonly string _fileName;

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
