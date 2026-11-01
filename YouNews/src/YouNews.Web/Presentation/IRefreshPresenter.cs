using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

/// <summary>Describes the refresh control: a button, or progress.</summary>
public interface IRefreshPresenter
{
    /// <summary>Builds the control for the given progress, null meaning no run is going.</summary>
    RefreshView Present(RefreshProgress? progress);
}