using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed class RefreshPresenter : IRefreshPresenter
{
    public RefreshView Present(RefreshProgress? progress)
    {
        if (progress is null)
            return new RefreshView(false, "", 0, 0, 0);

        var percent = progress.VideosTotal > 0 ? Math.Clamp(progress.VideosDone * 100 / progress.VideosTotal, 0, 100) : 0;
        return new RefreshView(true, progress.Activity, progress.VideosDone, progress.VideosTotal, percent);
    }
}