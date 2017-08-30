using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Renfield.AppendOnly.Library;
using Renfield.AppendOnly.Library.Contracts;
using Renfield.AppendOnly.Library.Services;

namespace Renfield.AppendOnly.Spike
{
    internal class Program
    {
        private const int COUNT = 25000;

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

            ShowTimeWithoutOpsPerSec("Overall", () =>
            {
                RunSingleThreaded(@"d:\temp\1.tmp", safeSerializer, list);
                RunMultiThreaded(@"d:\temp\2.tmp", safeSerializer, list);
            });
        }

        private static void RunSingleThreaded(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list)
        {
            Console.WriteLine("Using {0} - sequentially writing and reading {1} records", tempFile, COUNT);
            Generate(tempFile, serializer, list, SeqForLoop);

#if (DEBUG) // verify the data
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
                    Debug.Assert(r.Name == list[i].Name);
                    Debug.Assert(r.Address == list[i].Address);
                }
            }
#endif
        }

        private static void RunMultiThreaded(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list)
        {
            Console.WriteLine("Using {0} - multi-threaded writing and reading {1} records", tempFile, COUNT);
            Generate(tempFile, serializer, list, ParForLoop);
        }

        private static void Generate(string tempFile, SerializationEngine serializer, IReadOnlyList<TestClass> list, Action<Action<int>> loop)
        {
            using (var stream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var data = new StreamAccessor(stream);
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
    }
}