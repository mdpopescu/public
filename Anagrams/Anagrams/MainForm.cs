using System;
using System.Linq;
using System.Windows.Forms;
using Renfield.Anagrams.Properties;

namespace Renfield.Anagrams
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    //

    private volatile bool cancelRequested;

    private static Node LoadDictionary(string wordlist)
    {
      var result = new Node();

      wordlist
        .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
        .Select(word => word.Trim().ToLowerInvariant())
        .ToList()
        .ForEach(result.Add);

      return result;
    }

    //

    private Node dictionary;

    private void MainForm_Load(object sender, EventArgs e)
    {
      dictionary = LoadDictionary(Resources.WordsList);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Settings.Default.Save();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var aboutForm = new AboutForm())
      {
        aboutForm.ShowDialog();
      }
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
      if (btnGenerate.Text == "Cancel")
      {
        cancelRequested = true;
        return;
      }

      var text = txtText.Text.ToLowerInvariant().Replace(" ", "");

      txtOutput.Clear();
      using (new WaitGuard())
      using (new Guard(() => btnGenerate.Text = "Cancel", () => btnGenerate.Text = "Generate"))
      {
        cancelRequested = false;

        var anagrams = dictionary.GetAnagrams(text, int.Parse(txtMinWordLength.Text));

        var count = 0;
        foreach (var anagram in anagrams.TakeWhile(_ => !cancelRequested))
        {
          txtOutput.AppendText(anagram + Environment.NewLine);
          count++;

          Application.DoEvents();
        }

        txtOutput.AppendText("Total anagrams: " + count);
      }
    }
  }
}