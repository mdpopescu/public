using Microsoft.AspNetCore.Mvc;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;
using YouNews.Web.Presentation;

namespace YouNews.Web.Pages.Components;

public sealed class SidebarViewComponent : ViewComponent
{
    public SidebarViewComponent(IEntryReader reader, IRefreshStatus status, IDayListPresenter dayPresenter, IRefreshPresenter refreshPresenter)
    {
        this.reader = reader;
        this.status = status;
        this.dayPresenter = dayPresenter;
        this.refreshPresenter = refreshPresenter;
    }

    public async Task<IViewComponentResult> InvokeAsync(DateOnly? currentDay, bool channelsCurrent)
    {
        var days = await reader.ListDaysAsync(HttpContext.RequestAborted).ConfigureAwait(false);
        return View(new SidebarView(dayPresenter.Present(days, currentDay), refreshPresenter.Present(status.Current), channelsCurrent));
    }

    //

    private readonly IEntryReader reader;
    private readonly IRefreshStatus status;
    private readonly IDayListPresenter dayPresenter;
    private readonly IRefreshPresenter refreshPresenter;
}