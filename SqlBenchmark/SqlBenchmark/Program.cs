using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace SqlBenchmark
{
    internal class Program
    {
        private const string CONNECTION_STRING = @"Data Source=.\sqlexpress;Initial Catalog=zzz;Integrated Security=True";
        private const int COUNT = 100000;

        private static readonly Random RND = new Random();

        private static void Main()
        {
            Console.WriteLine($"Benchmarking the time it takes to insert {COUNT} records into three tables");
            Console.WriteLine("1. (Id, Text) where Id is an integer identity column");
            Console.WriteLine("2. (Id, Text) where Id is an unique identifier (a GUID)");
            Console.WriteLine("3. (Id, Text) where Id is a varchar(50) (also a GUID)");
            Console.WriteLine("Id is the primary key in all three cases.");

            var table1 = CreateTable("Id int IDENTITY PRIMARY KEY, Text nvarchar(100)");
            var table2 = CreateTable("Id uniqueidentifier PRIMARY KEY, Text nvarchar(100)");
            var table3 = CreateTable("Id varchar(50) PRIMARY KEY, Text nvarchar(100)");

            var ts1 = Run(COUNT, () => InsertWithoutId(table1));
            WriteResult(table1, ts1);

            var ts2 = Run(COUNT, () => InsertWithId(table2));
            WriteResult(table2, ts2);

            var ts3 = Run(COUNT, () => InsertWithId(table3));
            WriteResult(table3, ts3);
        }

        private static string CreateTable(string columns)
        {
            var tableName = GetRandomTableName();

            RunSql($"CREATE TABLE {tableName} ({columns})");

            return tableName;
        }

        private static void RunSql(string command)
        {
            using (var cx = new SqlConnection(CONNECTION_STRING))
            {
                cx.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cx;
                    cmd.CommandText = command;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static string GetRandomTableName()
        {
            var chars = Enumerable
                .Range(1, 6)
                .Select(_ => (char) ('A' + RND.Next(26)))
                .ToArray();
            return new string(chars);
        }

        private static TimeSpan Run(int count, Action action)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < count; i++)
                action();

            sw.Stop();
            return sw.Elapsed;
        }

        private static void InsertWithoutId(string tableName)
        {
            RunSql($"INSERT INTO {tableName} (Text) VALUES ('zzz')");
        }

        private static void InsertWithId(string tableName)
        {
            var id = Guid.NewGuid().ToString("B");
            RunSql($"INSERT INTO {tableName} (Id, Text) VALUES ('{id}', 'zzz')");
        }

        private static void WriteResult(string table, TimeSpan ts)
        {
            Console.WriteLine($"{table} {ts} {COUNT / ts.TotalMilliseconds * 1000} inserts per second");
        }
    }
}