using System.Text;

const string DOC_PATH = @"Data\doc.txt";
const string STOP_WORDS_PATH = @"Data\stop_words_english.txt";
const char APOSTROPHE = '\'';

var stopWords = new HashSet<string>();
var stopWord = new StringBuilder();
using (var stream = new FileStream(STOP_WORDS_PATH, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var reader = new StreamReader(stream))
{
    int next;
    while ((next = reader.Read()) != -1)
    {
        var ch = char.ToLowerInvariant((char)next);
        if (char.IsLetter(ch) || ch == APOSTROPHE)
        {
            stopWord.Append(ch);
        }
        else
        {
            var candidate = stopWord.ToString();
            if (candidate != "")
            {
                stopWords.Add(candidate);
            }

            stopWord = new StringBuilder();
        }
    }
}

var allWords = new Dictionary<string, int>();
var word = new StringBuilder();
using (var stream = new FileStream(DOC_PATH, FileMode.Open, FileAccess.Read, FileShare.Read))
using (var reader = new StreamReader(stream))
{
    int next;
    while ((next = reader.Read()) != -1)
    {
        var ch = char.ToLowerInvariant((char)next);
        if (char.IsLetter(ch) || ch == APOSTROPHE)
        {
            word.Append(ch);
        }
        else
        {
            var candidate = word.ToString();
            if (candidate != "")
            {
                if (allWords.ContainsKey(candidate))
                {
                    allWords[candidate]++;
                }
                else if (!stopWords.Contains(candidate))
                {
                    allWords.Add(candidate, 0);
                }
            }

            word = new StringBuilder();
        }
    }
}

var list = allWords.ToList();
list.Sort(new WordComparer());

for (var i = 0; i < list.Count && i < 10; i++)
{
    Console.WriteLine(list[i].Key + ": " + list[i].Value);
}

internal class WordComparer : IComparer<KeyValuePair<string, int>>
{
    // comparing Y to X because we want descending order
    public int Compare(KeyValuePair<string, int> x, KeyValuePair<string, int> y) => y.Value.CompareTo(x.Value);
}