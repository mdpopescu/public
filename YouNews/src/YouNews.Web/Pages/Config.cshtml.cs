using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Web.Presentation;

namespace YouNews.Web.Pages;

public sealed class ConfigModel : PageModel
{
    public IReadOnlyList<Channel> Channels { get; private set; } = [];

    public string InterestsText { get; private set; } = "";

    public SetupView Setup { get; private set; } = new(false, "", [], "", null);

    public string Address { get; private set; } = "";

    public string? AddError { get; private set; }

    [TempData]
    public string? Message { get; set; }

    public ConfigModel(IChannelStore channels, IInterestStore interests, IVideoCatalog catalog, ISetupStore setupStore, ISetupPresenter setupPresenter)
    {
        this.channels = channels;
        this.interests = interests;
        this.catalog = catalog;
        this.setupStore = setupStore;
        this.setupPresenter = setupPresenter;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(string? address, CancellationToken cancellationToken)
    {
        Address = address?.Trim() ?? "";
        AddError = await TryAddAsync(Address, cancellationToken);
        if (AddError is not null)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveAsync(long id, CancellationToken cancellationToken)
    {
        await channels.RemoveAsync(id, cancellationToken);
        Message = "Channel removed. Its past entries stay.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostInterestsAsync(string? interestsText, CancellationToken cancellationToken)
    {
        var lines = (interestsText ?? "").Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToList();
        await interests.SaveAsync(lines, cancellationToken);
        Message = "Interests saved.";
        return RedirectToPage();
    }

    //

    private readonly IChannelStore channels;
    private readonly IInterestStore interests;
    private readonly IVideoCatalog catalog;
    private readonly ISetupStore setupStore;
    private readonly ISetupPresenter setupPresenter;

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Channels";
        ViewData["Section"] = "channels";
        Channels = await channels.ListAsync(cancellationToken);
        InterestsText = string.Join("\n", await interests.GetAsync(cancellationToken));
        Setup = setupPresenter.Present(await setupStore.GetLatestAsync(cancellationToken));
    }

    /// <summary>Follows the channel. Returns the message to show when that did not work, or null on success.</summary>
    private async Task<string?> TryAddAsync(string address, CancellationToken cancellationToken)
    {
        if (address.Length == 0)
            return "Enter a channel URL or an @handle.";

        ChannelDetails? details;
        try
        {
            details = await catalog.ResolveChannelAsync(address, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return $"Could not look the channel up, the YouTube tools failed: {exception.Message}";
        }

        if (details is null)
            return $"No YouTube channel was found for \"{address}\". Check the URL or the @handle.";

        if (await channels.AddAsync(details, cancellationToken) is null)
            return $"You already follow {details.Name}.";

        Message = $"Now following {details.Name}.";
        return null;
    }
}