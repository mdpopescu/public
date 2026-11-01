using System.Globalization;
using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed class SetupPresenter : ISetupPresenter
{
    public SetupPresenter(TimeProvider time)
    {
        this.time = time;
    }

    public SetupView Present(SetupSnapshot? snapshot, string currentModel)
    {
        if (snapshot is null)
            return new SetupView(false, "", [], "", null, currentModel, [currentModel]);

        var local = TimeZoneInfo.ConvertTime(snapshot.CheckedAt, time.LocalTimeZone);
        var choices = snapshot.Models.Append(currentModel).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
        return new SetupView(true, snapshot.Hardware, snapshot.Models, local.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), snapshot.Warning, currentModel, choices);
    }

    //

    private readonly TimeProvider time;
}