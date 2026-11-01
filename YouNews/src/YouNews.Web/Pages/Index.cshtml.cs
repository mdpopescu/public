using Microsoft.AspNetCore.Mvc;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;
using YouNews.Web.Presentation;

namespace YouNews.Web.Pages;

public sealed class IndexModel : DayPageModelBase
{
    public string FollowingText => ChannelCount == 1 ? "Following 1 channel." : $"Following {ChannelCount} channels.";

    public bool IsEmpty { get; private set; }

    public int ChannelCount { get; private set; }

    public RefreshView Refresh { get; private set; } = new(false, "", 0, 0, 0);

    public IndexModel(IEntryReader reader, IEntryPresenter presenter, IChannelStore channels, IRefreshStatus status, IRefreshPresenter refreshPresenter)
        : base(reader, presenter)
    {
        this.reader = reader;
        this.channels = channels;
        this.status = status;
        this.refreshPresenter = refreshPresenter;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var days = await reader.ListDaysAsync(cancellationToken);
        if (days.Count > 0)
            return await ShowDayAsync(days[0].Day, cancellationToken);

        IsEmpty = true;
        ChannelCount = (await channels.ListAsync(cancellationToken)).Count;
        Refresh = refreshPresenter.Present(status.Current);
        ViewData["Title"] = "Welcome";
        return Page();
    }

    //

    private readonly IEntryReader reader;
    private readonly IChannelStore channels;
    private readonly IRefreshStatus status;
    private readonly IRefreshPresenter refreshPresenter;
}