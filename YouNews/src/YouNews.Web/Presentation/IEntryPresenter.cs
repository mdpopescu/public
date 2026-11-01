using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

/// <summary>Turns entries into story cards.</summary>
public interface IEntryPresenter
{
    /// <summary>Builds the card of one entry.</summary>
    EntryView Present(Entry entry);
}