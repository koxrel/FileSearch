﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearch
{
    class SearchEngine
    {
        string _initialDirectory;
        string _pattern;
        List<string> _searchResults;

        private CancellationToken token;
        private CancellationTokenSource ct;

        public static event Action FoundFile;
        public static event Action EndOfSearch; 

        public SearchEngine(string initialDirectory, string pattern)
        {
            _initialDirectory = initialDirectory;
            _pattern = pattern;
        }

        public void Find(string currentDirectory)
        {
            try
            {
                string[] files = Directory.GetFiles(currentDirectory);
                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
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
                                _searchResults.Add(file);
                                if (FoundFile != null)
                                    FoundFile();
                                Task.Delay(500).Wait();
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
                    Find(directory);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error directory {0}: {1}", currentDirectory, e.Message);
            }
        }

        public List<string> GetFiles()
        {
            _searchResults = new List<string>();
            
            ct = new CancellationTokenSource();
            token = ct.Token;
            Task.Run(() => Find(_initialDirectory), token)
                .ContinueWith(t =>
                {
                    if (EndOfSearch != null)
                        EndOfSearch();
                });
            return _searchResults;
        }

        public void Cancel()
        {
            ct.Cancel();
        }
    }
}
