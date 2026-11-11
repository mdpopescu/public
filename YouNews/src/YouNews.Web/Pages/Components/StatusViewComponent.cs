using Microsoft.AspNetCore.Mvc;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Web.Presentation;

namespace YouNews.Web.Pages.Components;

public sealed class StatusViewComponent : ViewComponent
{
    public const string BANNER = "Banner";
    public const string FOOTER = "Footer";

    public StatusViewComponent(IRefreshLog log, IVideoStore videos, IChannelStore channels, IToolProblems toolProblems, IStatusPresenter presenter)
    {
        this.log = log;
        this.videos = videos;
        this.channels = channels;
        this.toolProblems = toolProblems;
        this.presenter = presenter;
    }

    /// <param name="part">Banner for the tool problems at the top, Footer for the last run and the waiting videos.</param>
    public async Task<IViewComponentResult> InvokeAsync(string part)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var problems = await toolProblems.GetAsync(cancellationToken).ConfigureAwait(false);
        if (part == BANNER)
            return View(BANNER, presenter.Present(null, [], problems, PendingQueue.EMPTY));

        var lastRun = await log.GetLatestAsync(cancellationToken).ConfigureAwait(false);
        var pending = await videos.ListByStateAsync(VideoState.Pending, cancellationToken).ConfigureAwait(false);
        var gaveUp = await videos.ListByStateAsync(VideoState.GaveUp, cancellationToken).ConfigureAwait(false);
        var waiting = pending.Where(video => video.Attempts > 0).Concat(gaveUp).ToList();
        var channelNames = (await channels.ListAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(channel => channel.Id, channel => channel.Name);
        return View(FOOTER, presenter.Present(lastRun, waiting, problems, new PendingQueue(pending, channelNames)));
    }

    //

    private readonly IRefreshLog log;
    private readonly IVideoStore videos;
    private readonly IChannelStore channels;
    private readonly IToolProblems toolProblems;
    private readonly IStatusPresenter presenter;
}