using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Turtles.Library.Contracts;
using Turtles.Library.Services;

namespace Turtles.Tests.Services;

[TestClass]
public class FileManagerTests
{
    private const string? ORIGINAL_FILENAME = null;
    private const string ORIGINAL_TEXT = "";
    private const string OLD_FILENAME = "oldfile.txt";
    private const string NEW_FILENAME = "newfile.txt";
    private const string OLD_TEXT = "old text";
    private const string NEW_TEXT = "new text";

    private readonly Mock<IFileUI> ui = new();
    private readonly Mock<IFileSystem> fs = new();

    private readonly FileManager sut;

    public FileManagerTests()
    {
        sut = new FileManager(ui.Object, fs.Object);
    }

    [TestClass]
    public class New : FileManagerTests
    {
        [TestMethod("Unnamed, unmodified")]
        public void Test1()
        {
            var result = sut.New();

            ui.Verify(it => it.GetFilenameToSave(), Times.Never);
            Assert.IsTrue(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified")]
        public void Test2()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);

            var result = sut.New();

            Assert.IsTrue(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - happy path")]
        public void Test3()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSave(NEW_FILENAME);

            var result = sut.New();

            Assert.IsTrue(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - canceled")]
        public void Test4()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToCancel();

            var result = sut.New();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - save failed")]
        public void Test5()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.New();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Named, modified - happy path")]
        public void Test6()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true

            var result = sut.New();

            Assert.IsTrue(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, modified - save failed")]
        public void Test7()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true
            fs.Setup(it => it.Save(OLD_FILENAME, NEW_TEXT)).Throws<Exception>();

            var result = sut.New();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }
    }

    [TestClass]
    public class Open : FileManagerTests
    {
        public Open()
        {
            throw new Exception("There's an error in the File/Open logic (the current changes are saved without asking if they should be)");
        }

        [TestMethod("Unnamed, unmodified - happy path")]
        public void Test1()
        {
            SetupOpen(NEW_FILENAME, NEW_TEXT);

            var result = sut.Open();

            ui.Verify(it => it.GetFilenameToSave(), Times.Never);
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, unmodified - canceled")]
        public void Test2()
        {
            SetupOpenToCancel();

            var result = sut.Open();

            ui.Verify(it => it.GetFilenameToSave(), Times.Never);
            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, unmodified - load failed")]
        public void Test3()
        {
            SetupOpenToFail(NEW_FILENAME);

            var result = sut.Open();

            ui.Verify(it => it.GetFilenameToSave(), Times.Never);
            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - happy path")]
        public void Test4()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            SetupOpen(NEW_FILENAME, NEW_TEXT);

            var result = sut.Open();

            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - canceled")]
        public void Test5()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            SetupOpenToCancel();

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - load failed")]
        public void Test6()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            SetupOpenToFail(NEW_FILENAME);

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - happy path")]
        public void Test7()
        {
            sut.Text = OLD_TEXT; // set the modified flag to true
            SetupSave(OLD_FILENAME);
            SetupOpen(NEW_FILENAME, NEW_TEXT);

            var result = sut.Open();

            fs.Verify(it => it.Save(OLD_FILENAME, OLD_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - save canceled")]
        public void Test8()
        {
            sut.Text = OLD_TEXT; // set the modified flag to true
            SetupSaveToCancel();

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - save failed")]
        public void Test9()
        {
            sut.Text = OLD_TEXT; // set the modified flag to true
            SetupSaveToFail(OLD_FILENAME);

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - load canceled")]
        public void Test10()
        {
            sut.Text = OLD_TEXT; // set the modified flag to true
            SetupSave(OLD_FILENAME);
            SetupOpenToCancel();

            var result = sut.Open();

            fs.Verify(it => it.Save(OLD_FILENAME, OLD_TEXT));
            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - load failed")]
        public void Test11()
        {
            sut.Text = OLD_TEXT; // set the modified flag to true
            SetupSave(OLD_FILENAME);
            SetupOpenToFail(NEW_FILENAME);

            var result = sut.Open();

            fs.Verify(it => it.Save(OLD_FILENAME, OLD_TEXT));
            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, modified - happy path")]
        public void Test12()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = "123"; // set the modified flag to true
            SetupOpen(NEW_FILENAME, NEW_TEXT);

            var result = sut.Open();

            fs.Verify(it => it.Save(OLD_FILENAME, "123"));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, modified - save failed")]
        public void Test13()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = "123"; // set the modified flag to true
            fs.Setup(it => it.Save(OLD_FILENAME, "123")).Throws<Exception>();

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual("123", sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Named, modified - load canceled")]
        public void Test14()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = "123"; // set the modified flag to true
            SetupOpenToCancel();

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual("123", sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, modified - load failed")]
        public void Test15()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = "123"; // set the modified flag to true
            SetupOpenToFail(NEW_FILENAME);

            var result = sut.Open();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual("123", sut.Text);
            Assert.IsFalse(sut.IsModified);
        }
    }

    [TestClass]
    public class Save : FileManagerTests
    {
        [TestMethod("Unnamed, unmodified - happy path")]
        public void Test1()
        {
            SetupSave(NEW_FILENAME);

            var result = sut.Save();

            fs.Verify(it => it.Save(NEW_FILENAME, ORIGINAL_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, unmodified - canceled")]
        public void Test2()
        {
            SetupSaveToCancel();

            var result = sut.Save();

            fs.Verify(it => it.Save(NEW_FILENAME, ORIGINAL_TEXT), Times.Never);
            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, unmodified - save failed")]
        public void Test3()
        {
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.Save();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - happy path")]
        public void Test4()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);

            var result = sut.Save();

            fs.Verify(it => it.Save(OLD_FILENAME, OLD_TEXT), Times.Exactly(2));
            Assert.IsTrue(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - save failed")]
        public void Test5()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            fs.Setup(it => it.Save(OLD_FILENAME, OLD_TEXT)).Throws<Exception>();

            var result = sut.Save();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - happy path")]
        public void Test6()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSave(NEW_FILENAME);

            var result = sut.Save();

            fs.Verify(it => it.Save(NEW_FILENAME, NEW_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - save failed")]
        public void Test7()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.Save();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Named, modified - happy path")]
        public void Test8()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true

            var result = sut.Save();

            fs.Verify(it => it.Save(OLD_FILENAME, NEW_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, modified - save failed")]
        public void Test9()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true
            fs.Setup(it => it.Save(OLD_FILENAME, NEW_TEXT)).Throws<Exception>();

            var result = sut.Save();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }
    }

    [TestClass]
    public class SaveAs : FileManagerTests
    {
        [TestMethod("Unnamed, unmodified - happy path")]
        public void Test1()
        {
            SetupSave(NEW_FILENAME);

            var result = sut.SaveAs();

            fs.Verify(it => it.Save(NEW_FILENAME, ORIGINAL_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, unmodified - canceled")]
        public void Test2()
        {
            SetupSaveToCancel();

            var result = sut.SaveAs();

            fs.Verify(it => it.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, unmodified - save failed")]
        public void Test3()
        {
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.SaveAs();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(ORIGINAL_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - happy path")]
        public void Test4()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            SetupSave(NEW_FILENAME);

            var result = sut.SaveAs();

            fs.Verify(it => it.Save(NEW_FILENAME, OLD_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - canceled")]
        public void Test5()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            SetupSaveToCancel();

            var result = sut.SaveAs();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, unmodified - save failed")]
        public void Test6()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.SaveAs();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(OLD_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - happy path")]
        public void Test7()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSave(NEW_FILENAME);

            var result = sut.SaveAs();

            fs.Verify(it => it.Save(NEW_FILENAME, NEW_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - canceled")]
        public void Test8()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToCancel();

            var result = sut.SaveAs();

            fs.Verify(it => it.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Unnamed, modified - save failed")]
        public void Test9()
        {
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.SaveAs();

            Assert.IsFalse(result);
            Assert.AreEqual(ORIGINAL_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Named, modified - happy path")]
        public void Test10()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSave(NEW_FILENAME);

            var result = sut.SaveAs();

            fs.Verify(it => it.Save(NEW_FILENAME, NEW_TEXT));
            Assert.IsTrue(result);
            Assert.AreEqual(NEW_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsFalse(sut.IsModified);
        }

        [TestMethod("Named, modified - canceled")]
        public void Test11()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToCancel();

            var result = sut.SaveAs();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }

        [TestMethod("Named, modified - save failed")]
        public void Test12()
        {
            SetFilenameAndText(OLD_FILENAME, OLD_TEXT);
            sut.Text = NEW_TEXT; // set the modified flag to true
            SetupSaveToFail(NEW_FILENAME);

            var result = sut.SaveAs();

            Assert.IsFalse(result);
            Assert.AreEqual(OLD_FILENAME, sut.Filename);
            Assert.AreEqual(NEW_TEXT, sut.Text);
            Assert.IsTrue(sut.IsModified);
        }
    }

    //

    private void SetFilenameAndText(string filename, string text)
    {
        sut.Text = text;
        SetFilenameToSave(filename);
        sut.SaveAs(); // to change the filename and set the modified flag to false
    }

    private void SetupOpen(string filename, string text)
    {
        SetFilenameToOpen(filename);
        fs.Setup(it => it.Load(filename)).Returns(text);
    }

    private void SetupOpenToCancel() =>
        SetFilenameToOpen(null);

    private void SetupOpenToFail(string filename)
    {
        SetFilenameToOpen(filename);
        fs.Setup(it => it.Load(filename)).Throws<Exception>();
    }

    private void SetupSave(string filename) =>
        SetFilenameToSave(filename);

    private void SetupSaveToCancel() =>
        SetFilenameToSave(null);

    private void SetupSaveToFail(string filename)
    {
        SetFilenameToSave(filename);
        fs.Setup(it => it.Save(filename, It.IsAny<string>())).Throws<Exception>();
    }

    private void SetFilenameToOpen(string? filename) =>
        ui.Setup(it => it.GetFilenameToOpen()).Returns(filename);

    private void SetFilenameToSave(string? filename) =>
        ui.Setup(it => it.GetFilenameToSave()).Returns(filename);
}