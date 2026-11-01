using System.Globalization;
using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed class SetupPresenter : ISetupPresenter
{
    public SetupPresenter(TimeProvider time)
    {
        this.time = time;
    }

    public SetupView Present(SetupSnapshot? snapshot)
    {
        if (snapshot is null)
            return new SetupView(false, "", [], "", null);

        var local = TimeZoneInfo.ConvertTime(snapshot.CheckedAt, time.LocalTimeZone);
        return new SetupView(true, snapshot.Hardware, snapshot.Models, local.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), snapshot.Warning);
    }

    //

    private readonly TimeProvider time;
}