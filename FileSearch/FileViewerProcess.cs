using System.Diagnostics;

namespace FileSearch
{
    internal class FileViewerProcess
    {
        private readonly string _fileName;
        private const string App = "notepad.exe";

        public FileViewerProcess(string fileName)
        {
            _fileName = fileName;
        }

        public void StartProcess()
        {
            Process p = new Process
            {
                StartInfo =
                {
                    FileName = App,
                    Arguments = _fileName
                }
            };

            p.Start();
        }
    }
}
