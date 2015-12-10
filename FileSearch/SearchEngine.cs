using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearch
{
    class SearchEngine
    {
        string _initialDirectory;
        string _pattern;
        private int _processedFiles;
        
        private CancellationTokenSource _ct;

        private int _numberOfFiles;

        public static event Action<string, double> FoundFile;
        public static event Action EndOfSearch; 

        public SearchEngine(string initialDirectory, string pattern)
        {
            _initialDirectory = initialDirectory;
            _pattern = pattern;
        }

        private void Find(string currentDirectory, CancellationToken token)
        {
            try
            {
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    _processedFiles += 1;
                    StreamReader sr = null;
                    try
                    {
                        sr = new StreamReader(file);
                        bool found = false;
                        while (!sr.EndOfStream && !found)
                        {
                            string line = sr.ReadLine();
                            if (line.Contains(_pattern))
                            {
                                found = true;
                                if (FoundFile != null)
                                    FoundFile(file, (double)_processedFiles / _numberOfFiles * 100);
                                //Task.Delay(500).Wait();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error processing file {0}: {1}", file, e.Message);
                    }
                    finally
                    {
                        if (sr != null)
                            sr.Close();
                    }
                }
                // Now look through all directories inside the current (recursive call)
                foreach (var directory in Directory.GetDirectories(currentDirectory))
                    Find(directory, token);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error directory {0}: {1}", currentDirectory, e.Message);
            }
        }

        public void GetFiles()
        {
            _ct = new CancellationTokenSource();
            var token = _ct.Token;

            //GetNumberOfFiles(_initialDirectory);
            //Task.Run(() => Find(_initialDirectory, token), token)
            //    .ContinueWith(t =>
            //    {
            //        if (EndOfSearch != null)
            //            EndOfSearch();
            //    });

            Task.Run(() => GetNumberOfFiles(_initialDirectory, token), token) 
                .ContinueWith(t => Find(_initialDirectory, token), token)
                .ContinueWith(t =>
                {
                    if (EndOfSearch != null)
                        EndOfSearch();
                });
        }

        public void Cancel()
        {
            _ct.Cancel();
        }

        private void GetNumberOfFiles(string currentDirectory, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                foreach (var file in Directory.GetFiles(currentDirectory))
                    _numberOfFiles += 1;
                foreach (var directory in Directory.GetDirectories(currentDirectory))
                    GetNumberOfFiles(directory, token);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
