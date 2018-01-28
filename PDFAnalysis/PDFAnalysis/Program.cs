using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using MsgReader.Outlook;

namespace PDFAnalysis
{
    internal class Program
    {
        private const string ROOT = @"C:\temp\PDFAnalysis";
        private const string OUTPUT = @"C:\temp\PDFAnalysis\output.csv";

        private static void Main()
        {
            var output = new List<Dictionary<string, string>>();
            foreach (var dir in Directory.GetDirectories(ROOT))
            {
                // phase 1: extract all attachments from .msg files
                foreach (var msg in Directory.GetFiles(dir, "*.msg"))
                    ExtractAttachments(dir, msg);

                // phase 2: process all .pdf files
                output.AddRange(Directory.GetFiles(dir, "*.pdf").Select(ExtractMetadata));
            }

            var allKeys = output
                .SelectMany(dict => dict.Keys)
                .Distinct()
                .ToList();
            using (var sw = new StreamWriter(OUTPUT))
            {
                sw.WriteLine(string.Join(",", allKeys.Quoted()));
                foreach (var dict in output)
                    sw.WriteLine(string.Join(",", dict.Extract(allKeys).Quoted()));
            }
        }

        private static void ExtractAttachments(string path, string filename)
        {
            using (var msg = new Storage.Message(filename))
                foreach (var attachment in msg.Attachments.OfType<Storage.Attachment>())
                    if (attachment.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        var outputFilename = Path.Combine(path, attachment.FileName);
                        var data = attachment.Data;

                        var dups = 0;
                        while (File.Exists(outputFilename))
                        {
                            if (File.ReadAllBytes(outputFilename).GetMD5() == data.GetMD5())
                                break;

                            dups++;
                            outputFilename = Path.Combine(path, Path.GetFileNameWithoutExtension(outputFilename) + $" ({dups}).pdf");
                        }

                        File.WriteAllBytes(outputFilename, data);
                    }
        }

        private static Dictionary<string, string> ExtractMetadata(string pdf)
        {
            var parts = pdf.Split('\\');
            var dict = new Dictionary<string, string>
            {
                ["ZIP Filename"] = parts[parts.Length - 2],
                ["PDF filename"] = parts[parts.Length - 1],
            };

            var fileInfo = new FileInfo(pdf);
            dict["Filesystem Created"] = fileInfo.CreationTime.ToString("u");
            dict["Filesystem Modified"] = fileInfo.LastWriteTime.ToString("u");

            var pdfReader = new PdfReader(pdf);
            var info = pdfReader.Info;
            foreach (var key in info.Keys)
                dict[key] = info[key];

            var content = File.ReadAllText(pdf);
            var count = content.FindAll("%%EOF").Count() - 1;
            dict["Updates"] = count.ToString();

            return dict;
        }
    }
}