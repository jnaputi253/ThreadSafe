using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Net.Sockets;

namespace FileEventListener
{
    class Database
    {
        private SQLiteConnection _connection;
        private SQLiteCommand _command;
        private SQLiteDataReader _reader;

        public Database()
        {
            OpenConnection();
            CreateTable();
        }

        private void OpenConnection()
        {
            try
            {
                _connection = new SQLiteConnection("Data Source=logInfo.db;Version=3;New=True;Compress=True");
                _connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError creating DB connection");
                Console.WriteLine(e.Message);
            }
        }

        private void CreateTable()
        {
            try
            {
                _command = _connection.CreateCommand();
                _command.CommandText = "CREATE TABLE IF NOT EXISTS LogInfo (" +
                                       "File varchar(50), " +
                                       "Path varchar(150), " +
                                       "Event varchar(100), " +
                                       "Time varchar(30))";
                _command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError Creating Table");
                Console.WriteLine(e.Message);
            }
        }

        public void LogChange(FileSystemEventArgs e)
        {
            try
            {
                _command.CommandText = "INSERT INTO LogInfo (File, Path, Event, Time) " +
                                       $"VALUES ('{e.Name}', " +
                                       $"'{e.FullPath}', " +
                                       $"'{e.ChangeType}', " +
                                       $"'{DateTime.Now.ToString("yyyy-M-dd h:mm:ss")}')";

                _command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                Console.WriteLine("Error when logging change\n" + err.Message);
            }
        }

        public void LogRename(RenamedEventArgs e)
        {
            try
            {
                _command.CommandText = "INSERT INTO LogInfo (File, Path, Event, Time) " +
                                   $"VALUES ('{Path.GetFileName(e.FullPath)}', " +
                                   $"'{Path.GetDirectoryName(e.FullPath)}', " +
                                   $"'{Path.GetFileName(e.OldFullPath)} changed to {Path.GetFileName(e.FullPath)}', " +
                                   $"'{DateTime.Now.ToString("yyyy-M-dd h:mm:ss")}')";
                _command.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error when logging rename\n{err.Message}");
            }
        }

        public void DisplayInfo()
        {
            _command.CommandText = "SELECT * FROM LogInfo";
            _reader = _command.ExecuteReader();

            try
            {
                while (_reader.Read())
                {
                    string info = $"File: {_reader["File"]}\n" +
                    $"Path: {_reader["Path"]}\n" +
                    $"Change Type: {_reader["Event"]}\n" +
                    $"Time: {_reader["Time"]}\n";

                    Console.WriteLine(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error when reading:\n{e.Message}");
            }

            _reader.Close();
        }

        public void Cleanup()
        {
            _connection.Close();
        }
    }
}
