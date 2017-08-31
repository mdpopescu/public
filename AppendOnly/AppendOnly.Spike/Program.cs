using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Renfield.AppendOnly.Library.Contracts;
using Renfield.AppendOnly.Library.Services;

namespace Renfield.AppendOnly.Spike
{
    internal class Program
    {
        private const int COUNT = 50;

        private static readonly Random RND = new Random();

        private static void Main()
        {
            var serializer = new ProtoBufSerializationEngine();
            // the protobuf serializer might not be thread-safe
            var safeSerializer = new ConcurrentSerializationEngine(serializer);

            var list = new List<TestClass>();
            for (var i = 0; i < COUNT; i++)
            {
                var c = new TestClass { Name = GenerateRandomString(20), Address = GenerateRandomString(40) };
                list.Add(c);
            }

            ShowTimeWithoutOpsPerSec("Overall - 1", () =>
            {
                Run($"Sequentially writing and reading {COUNT} records", () => Generate1(@"d:\temp\1.tmp", safeSerializer, list, SeqForLoop));
                Verify1(@"d:\temp\1.tmp", safeSerializer, list);

                Run($"Multi-threaded writing and reading {COUNT} records", () => Generate1(@"d:\temp\2.tmp", safeSerializer, list, ParForLoop));
                Verify1(@"d:\temp\2.tmp", safeSerializer, list);
            });

            ShowTimeWithoutOpsPerSec("Overall - 2", () =>
            {
                Run($"Sequentially writing and reading {COUNT} records", () => Generate2(@"d:\temp\1.tmp", safeSerializer, list, SeqForLoop));
                Verify2(@"d:\temp\1.tmp", safeSerializer, list);

                Run($"Multi-threaded writing and reading {COUNT} records", () => Generate2(@"d:\temp\2.tmp", safeSerializer, list, ParForLoop));
                Verify2(@"d:\temp\2.tmp", safeSerializer, list);
            });
        }

        private static void Run(string comment, Action generate)
        {
            Console.WriteLine(comment);
            generate();
        }

        private static void Generate1(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list, Action<Action<int>> loop)
        {
            Console.WriteLine($"Using {tempFile}");

            using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var data = new StreamAccessor(stream);
                CreateAndProcessFile(data, serializer, list, loop);
            }

            // close and reopen the file (rebuilds the index)
            ShowTimeIncludingOpsPerSec("rebuild index", () =>
            {
                using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var data = new StreamAccessor(stream);
                    var lFile = new LowLevelAppendOnlyFile(data);
                }
            });
        }

        private static void Generate2(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list, Action<Action<int>> loop)
        {
            Console.WriteLine($"Using {tempFile}");

            using (var data = new FileAccessor(tempFile))
            {
                CreateAndProcessFile(data, serializer, list, loop);
            }

            // close and reopen the file (rebuilds the index)
            ShowTimeIncludingOpsPerSec("rebuild index", () =>
            {
                using (var data = new FileAccessor(tempFile))
                {
                    var lFile = new LowLevelAppendOnlyFile(data);
                }
            });
        }

        private static void CreateAndProcessFile(RandomAccessor data, SerializationEngine serializer, IReadOnlyList<TestClass> list, Action<Action<int>> loop)
        {
            var lFile = new LowLevelAppendOnlyFile(data);
            var file = new GenericAppendOnlyFile<TestClass>(lFile, serializer);

            // append COUNT records
            ShowTimeIncludingOpsPerSec("append", () => loop(i => file.Append(list[i])));

            // read all the records in a single batch
            ShowTimeIncludingOpsPerSec("read all in a single batch", () =>
            {
                var records = file.ReadFrom(0).ToList();
            });

            // read all the records, individually
            ShowTimeIncludingOpsPerSec("read all, individually", () => loop(i => file.Read(i)));
        }

        private static void SeqForLoop(Action<int> action)
        {
            for (var i = 0; i < COUNT; i++)
                action.Invoke(i);
        }

        private static void ParForLoop(Action<int> action)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = 32 };
            Parallel.For(0, COUNT, options, action);
        }

        private static void ShowTimeWithoutOpsPerSec(string message, Action action)
        {
            var msec = ComputeElapsed(action);
            Console.WriteLine("{0} - time elapsed: {1} msec", message, msec);
        }

        private static void ShowTimeIncludingOpsPerSec(string message, Action action)
        {
            var msec = ComputeElapsed(action);
            Console.WriteLine("{0} - time elapsed: {1} msec ({2:0.00} / sec)", message, msec, (double) COUNT / msec * 1000.0);
        }

        private static long ComputeElapsed(Action action)
        {
            var sw = new Stopwatch();

            sw.Start();
            action.Invoke();
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static string GenerateRandomString(int maxLength)
        {
            var length = RND.Next(maxLength / 2, maxLength + 1);

            var result = new char[length];
            for (var i = 0; i < length; i++)
                result[i] = (char) RND.Next('A', 'Z' + 1);

            return new string(result);
        }

        private static void Verify1(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list)
        {
            using (var stream = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var data = new StreamAccessor(stream);
                var lFile = new LowLevelAppendOnlyFile(data);
                var file = new GenericAppendOnlyFile<TestClass>(lFile, serializer);

                var records = file.ReadFrom(0).ToList();

                // verify that the index is built correctly
                for (var i = 0; i < COUNT; i++)
                {
                    var r = records[i];
                    if (r.Name != list[i].Name)
                        throw new Exception($"Verify1: Expected [{list[i].Name}] but was [{r.Name}].");
                    if (r.Address != list[i].Address)
                        throw new Exception($"Verify1: Expected [{list[i].Address}] but was [{r.Address}].");
                }
            }
        }

        private static void Verify2(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list)
        {
            using (var data = new FileAccessor(tempFile))
            {
                var lFile = new LowLevelAppendOnlyFile(data);
                var file = new GenericAppendOnlyFile<TestClass>(lFile, serializer);

                var records = file.ReadFrom(0).ToList();

                // verify that the index is built correctly
                for (var i = 0; i < COUNT; i++)
                {
                    var r = records[i];
                    if (r.Name != list[i].Name)
                        throw new Exception($"Verify2: Expected [{list[i].Name}] but was [{r.Name}].");
                    if (r.Address != list[i].Address)
                        throw new Exception($"Verify2: Expected [{list[i].Address}] but was [{r.Address}].");
                }
            }
        }
    }
}