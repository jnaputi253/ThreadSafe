using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileEventListener
{
    class Program
    {
        static void Main()
        {
            string path = GetFilePath();
            FileWatcher watcher = new FileWatcher(path);
            Message(path);

            while (Console.ReadLine() != "q") ;

            Console.WriteLine("Contents of Database");
            watcher.DisplayInfo();
            Console.WriteLine();

            watcher.Cleanup();

            Exit();
        }

        private static string GetFilePath()
        {
            string path = "";

            while (true)
            {
                Console.Write("Enter a file path: ");
                path = Console.ReadLine();

                try
                {
                    if (path != null && !Directory.Exists(path))
                        throw new DirectoryNotFoundException("\nDirectory supplied could not be found");

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Please enter a valid directory\n");
                }
            }

            if (path != null && path[path.Length - 1].ToString().Equals(@"\")) return path;
            var builder = new StringBuilder(path);
            builder.Append(@"\");

            return builder.ToString();
        }

        private static void Message(string path)
        {
            Console.WriteLine($"Directory: {path}, is now being watched");
            Console.WriteLine("Press 'q' to Quit\n");
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
