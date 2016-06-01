using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace SqlBenchmark
{
    internal class Program
    {
        private const string CONNECTION_STRING = @"Data Source=.\sqlexpress;Initial Catalog=zzz;Integrated Security=True";
        private const int COUNT = 10 * 1000 * 1000;
        private const int BATCH_SIZE = 1000;

        private static readonly Random RND = new Random();

        private static readonly List<string> GUIDS = new List<string>();

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

            //Console.WriteLine("Individual inserts");
            //WriteResult(table1, Run(COUNT, () => InsertWithoutId(table1)));
            //WriteResult(table2, Run(COUNT, () => InsertWithId(table2)));
            //WriteResult(table3, Run(COUNT, () => InsertWithId(table3)));

            Console.WriteLine("Bulk inserts");
            WriteResult(table1, Run(COUNT / BATCH_SIZE, () => BulkInsertWithoutId(table1)));
            WriteResult(table2, Run(COUNT / BATCH_SIZE, () => BulkInsertWithGuidId(table2)));
            WriteResult(table3, Run(COUNT / BATCH_SIZE, () => BulkInsertWithStringId(table3)));

            Console.WriteLine("Queries");
            WriteResult(table1, Run(1000, () => SelectByInt(table1)));
            WriteResult(table2, Run(1000, () => SelectByGuid(table2)));
            WriteResult(table3, Run(1000, () => SelectByGuid(table3)));
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

        private static void WriteResult(string table, TimeSpan ts)
        {
            Console.WriteLine($"{table} {ts} {COUNT / ts.TotalMilliseconds * 1000} operations per second");
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

        private static string GetUniqueId()
        {
            return Guid.NewGuid().ToString("D").ToUpperInvariant();
        }

        private static void InsertWithoutId(string tableName)
        {
            RunSql($"INSERT INTO {tableName} (Text) VALUES ('zzz')");
        }

        private static void InsertWithId(string tableName)
        {
            var id = GetUniqueId();
            RunSql($"INSERT INTO {tableName} (Id, Text) VALUES ('{id}', 'zzz')");
        }

        private static void BulkInsertWithoutId(string tableName)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add("Text", typeof(string));

                for (var i = 0; i < BATCH_SIZE; i++)
                    data.Rows.Add("zzz");

                BulkInsert(tableName, data);
            }
        }

        private static void BulkInsertWithGuidId(string tableName)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add("Id", typeof(Guid));
                data.Columns.Add("Text", typeof(string));

                // the first GUID gets added to the global list
                var guid = GetUniqueId();
                GUIDS.Add(guid);
                data.Rows.Add(guid, "zzz");

                for (var i = 1; i < BATCH_SIZE; i++)
                    data.Rows.Add(GetUniqueId(), "zzz");

                BulkInsert(tableName, data);
            }
        }

        private static void BulkInsertWithStringId(string tableName)
        {
            using (var data = new DataTable())
            {
                data.Columns.Add("Id", typeof(string));
                data.Columns.Add("Text", typeof(string));

                // the first GUID gets added to the global list
                var guid = GetUniqueId();
                GUIDS.Add(guid);
                data.Rows.Add(guid, "zzz");

                for (var i = 1; i < BATCH_SIZE; i++)
                    data.Rows.Add(GetUniqueId(), "zzz");

                BulkInsert(tableName, data);
            }
        }

        private static void BulkInsert(string tableName, DataTable data)
        {
            using (var cx = new SqlConnection(CONNECTION_STRING))
            {
                cx.Open();

                using (var bulk = new SqlBulkCopy(cx.ConnectionString, SqlBulkCopyOptions.TableLock))
                {
                    bulk.BatchSize = BATCH_SIZE;
                    bulk.DestinationTableName = tableName;

                    bulk.WriteToServer(data);
                }
            }
        }

        private static void SelectByInt(string tableName)
        {
            var id = RND.Next(COUNT);
            RunSql($"SELECT * FROM {tableName} WHERE ID = {id}");
        }

        private static void SelectByGuid(string tableName)
        {
            var id = GUIDS[RND.Next(GUIDS.Count)];
            RunSql($"SELECT * FROM {tableName} WHERE ID = '{id}'");
        }
    }
}