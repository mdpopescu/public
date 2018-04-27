using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Statix.Library.Models;
using Statix.Library.Services;

namespace Statix.Tests
{
    [TestClass]
    [TestCategory("Slow")]
    public class AcceptanceTests
    {
        [TestMethod]
        public void Scenario1()
        {
            var inputs = new[]
            {
                new MarkdownFile
                {
                    Filename = "123.md",
                    Date = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(3)),
                    Content = "# First article",
                },
                new MarkdownFile
                {
                    Filename = "456.md",
                    Date = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(3)),
                    Content = "Testing *italic* (_italic_), **bold** (__bold__), **bold _italic_** (__bold *italic*__) and finally ~~strikethrough~~.",
                },
            };

            var transformer = new Transformer();
            var outputs = inputs.Select(transformer.Process).ToList();

            Assert.AreEqual(2, outputs.Count);
            Assert.AreEqual("123.html", outputs[0].Filename);
            Assert.AreEqual("<html><body><h1>First article</h1></body></html>", outputs[0].Content);
            Assert.AreEqual("456.html", outputs[1].Filename);
            Assert.AreEqual(
                "<html><body>Testing <i>italic</i> (<i>italic</i>), <b>bold</b> (<b>bold</b>), " +
                "<b>bold <i>italic</i></b> (<b>bold <i>italic</i></b>) and finally <del>strikethrough</del>.</body></html>",
                outputs[1].Content);
        }
    }
}