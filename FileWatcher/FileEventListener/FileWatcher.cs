using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileEventListener
{
    class FileWatcher
    {
        private readonly FileSystemWatcher mFileSystemWatcher;
        private Database _database;

        public FileWatcher(string path)
        {
            mFileSystemWatcher = new FileSystemWatcher(path);
            OpenDatabase();
            WireupEvents();
        }

        private void OpenDatabase()
        {
            _database = new Database();
        }

        private void WireupEvents()
        {
            mFileSystemWatcher.Changed += OnChanged;
            mFileSystemWatcher.Created += OnChanged;
            mFileSystemWatcher.Deleted += OnChanged;
            mFileSystemWatcher.Renamed += OnRenamed;

            mFileSystemWatcher.EnableRaisingEvents = true;
            mFileSystemWatcher.IncludeSubdirectories = true;
        }

        private void OnChanged(object o, FileSystemEventArgs e)
        {
            string info = $"File: {e.Name} => {e.ChangeType}\n";
            Console.WriteLine(info);

            LogChange(e);
        }

        private void OnRenamed(object o, RenamedEventArgs e)
        {
            Console.WriteLine($"{Path.GetFileName(e.OldFullPath)} renamed to " +
                              $"{Path.GetFileName(e.FullPath)}\n");
            LogRename(e);
        }

        private void LogChange(FileSystemEventArgs e)
        {
            _database.LogChange(e);
        }

        private void LogRename(RenamedEventArgs e)
        {
            _database.LogRename(e);
        }

        public void DisplayInfo()
        {
            _database.DisplayInfo();
        }

        public void Cleanup()
        {
            _database.Cleanup();
        }
    }
}
