﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearch
{
    internal class SearchEngine
    {
        private readonly string _initialDirectory;
        private readonly string _pattern;
        
        private CancellationTokenSource _ct;
        
        private int _processedFiles;
        private int _numberOfFiles;
        
        public static event Action<string> FoundFile;
        public static event Action<double> ReportProgress;
        
        public SearchEngine(string initialDirectory, string pattern)
        {
            _initialDirectory = initialDirectory;
            _pattern = pattern;
        }
        
        private void Find(string currentDirectory, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    _processedFiles++;
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
                                    FoundFile(file);
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
                    if (ReportProgress != null && _numberOfFiles != 0)
                        ReportProgress((double)_processedFiles / _numberOfFiles * 100);
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
        
        public Task GetFiles()
        {
            _ct = new CancellationTokenSource();
            
            Task.Run(() => GetNumberOfFiles(_initialDirectory, _ct.Token), _ct.Token);

            return Task.Run(() => Find(_initialDirectory, _ct.Token), _ct.Token);
        }
        
        public void Cancel()
        {
            _ct.Cancel();
        }
        
        private void GetNumberOfFiles(string currentDirectory, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Interlocked.Add(ref _numberOfFiles, Directory.GetFiles(currentDirectory).Length);

                Parallel.ForEach(Directory.GetDirectories(currentDirectory), dir => GetNumberOfFiles(dir, token));
            }
            catch (Exception)
            {

            }
        }
    }
}