using System.Collections.Generic;
using ExtractLinks.Models;

namespace ExtractLinks.Contracts
{
    public interface MainUI
    {
        IEnumerable<LinkView> AllLinks { get; }
        IEnumerable<LinkView> SelectedLinks { get; }

        void SetLinks(IEnumerable<LinkView> links);
        void SelectLinks(IEnumerable<int> indices);
    }
}