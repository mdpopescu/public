using System.Collections.Generic;

namespace Renfield.Anagrams
{
  public class Node
  {
    public Node(char letter = '\0', bool final = false, int depth = 0)
    {
      this.letter = letter;
      this.final = final;
      this.depth = depth;

      children = new Dictionary<char, Node>();
    }

    public void Add(string letters)
    {
      var node = this;
      var index = 0;
      var lastIndex = letters.Length - 1;
      foreach (var l in letters)
      {
        if (!(node.children.ContainsKey(l)))
          node.children[l] = new Node(l, index == lastIndex, index + 1);

        node = node.children[l];
        index++;
      }
    }

    public IEnumerable<string> GetAnagrams(string letters, int minWordLength)
    {
      var counters = new Dictionary<char, int>();
      foreach (var l in letters)
        counters[l] = counters.Get(l) + 1;

      return InternalGetAnagrams(counters, new List<string>(), this, letters.Length, minWordLength);
    }

    //

    private char letter;
    private readonly bool final;
    private readonly int depth;
    private readonly Dictionary<char, Node> children;

    private IEnumerable<string> InternalGetAnagrams(Dictionary<char, int> tiles, List<string> path, Node root,
                                                    int minLength, int minWordLength)
    {
      if (final && depth >= minWordLength)
      {
        var word = string.Join("", path);
        var length = word.Replace(" ", "").Length;
        if (length >= minLength)
          yield return word;

        using (new Guard(() => path.Push(" "), path.Pop))
        {
          foreach (var anagram in root.InternalGetAnagrams(tiles, path, root, minLength, minWordLength))
            yield return anagram;
        }
      }

      foreach (var child in children)
      {
        var l = child.Key;
        var node = child.Value;

        var count = tiles.Get(l);
        if (count == 0)
          continue;

        using (new Guard(() => tiles[l] = count - 1, () => tiles[l] = count))
        using (new Guard(() => path.Push(l.ToString()), path.Pop))
        {
          foreach (var anagram in node.InternalGetAnagrams(tiles, path, root, minLength, minWordLength))
            yield return anagram;
        }
      }
    }
  }
}