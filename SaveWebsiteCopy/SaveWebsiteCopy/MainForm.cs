using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace SaveWebsiteCopy
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    //

    private static string SelectFolder()
    {
      var dlg = new CommonOpenFileDialog
      {
        AllowNonFileSystemItems = true,
        IsFolderPicker = true,
        DefaultFileName = "Select Folder",
        Title = "Select Folder",
      };

      using (dlg)
        return dlg.ShowDialog() == CommonFileDialogResult.Ok ? dlg.FileName : null;
    }

    private static IEnumerable<string> SearchSite(string searchString, string domain)
    {
      using (var web = new WebClient())
      {
        var escapedSearchString = Uri.EscapeDataString(searchString);
        var ddg = $"https://duckduckgo.com/html/?q=\"{escapedSearchString}\"%20site%3A{domain}";
        var html = web.DownloadString(ddg);
        return GetResults(html);
      }
    }

    private static IEnumerable<string> GetResults(string html)
    {
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      if (doc.ParseErrors != null && doc.ParseErrors.Any() || doc.DocumentNode == null)
        return Enumerable.Empty<string>();

      var body = doc.DocumentNode.SelectSingleNode("//body");
      if (body == null)
        return Enumerable.Empty<string>();

      var results = body.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '), ' results ')]");
      if (results == null)
        return Enumerable.Empty<string>();

      var urls = results.SelectNodes("//a[contains(concat(' ', normalize-space(@class), ' '), ' result__snippet ')]");
      return urls
        .Select(it => it.GetAttributeValue("href", ""))
        .Where(it => !string.IsNullOrWhiteSpace(it));
    }

    private void ResetUI(int max)
    {
      txtLog.Clear();

      pbLoading.Minimum = 0;
      pbLoading.Maximum = max;
      pbLoading.Value = 0;
    }

    private void ProcessPage(string page)
    {
      pbLoading.Value++;

      var contents = LoadPage(page);
      if (contents == null)
      {
        txtLog.AppendText($"Could not download {page}\r\n");
        return;
      }

      var filename = PageToFilename(page);
      SavePage(txtSaveDirectory.Text, filename, contents);
    }

    private static string LoadPage(string url)
    {
      using (var web = new WebClient())
      {
        var json = web.DownloadString($"http://archive.org/wayback/available?url={url}");
        dynamic obj = SimpleJson.DeserializeObject(json);

        dynamic snapshots = obj.archived_snapshots;
        if (snapshots == null)
          return null;

        dynamic closest = snapshots.closest;
        if (closest == null)
          return null;

        var actualUrl = closest.url;
        if (actualUrl == null)
          return null;

        return web.DownloadString(actualUrl);
      }
    }

    private static string PageToFilename(string url)
    {
      var result = url
        .Replace("http://", "")
        .Replace("https://", "")
        .Replace("web.archive.org/web/", "");
      result = Regex.Replace(result, "[^a-zA-Z0-9 ]", "_");

      return result + ".html";
    }

    private static void SavePage(string folder, string filename, string contents)
    {
      File.WriteAllText(Path.Combine(folder, filename), contents);
    }

    //

    private void btnSelectDirectory_Click(object sender, EventArgs e)
    {
      var folder = SelectFolder();
      if (folder != null)
        txtSaveDirectory.Text = folder;
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      var pages = SearchSite(txtSearchString.Text, txtURL.Text).ToArray();

      ResetUI(pages.Length);
      foreach (var page in pages)
        ProcessPage(page);
    }

    private void txt_TextChanged(object sender, EventArgs e)
    {
      btnLoad.Enabled = !string.IsNullOrWhiteSpace(txtURL.Text) &&
                        !string.IsNullOrWhiteSpace(txtSearchString.Text) &&
                        !string.IsNullOrWhiteSpace(txtSaveDirectory.Text);
    }
  }
}