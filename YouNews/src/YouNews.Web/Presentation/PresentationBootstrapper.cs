namespace YouNews.Web.Presentation;

public static class PresentationBootstrapper
{
    public static IServiceCollection AddYouNewsPresentation(this IServiceCollection services)
    {
        services.AddSingleton<ParagraphSplitter>();
        services.AddSingleton<IEntryPresenter, EntryPresenter>();
        services.AddSingleton<IDayListPresenter, DayListPresenter>();
        services.AddSingleton<IRefreshPresenter, RefreshPresenter>();
        services.AddSingleton<ISetupPresenter, SetupPresenter>();
        services.AddSingleton<IStatusPresenter, StatusPresenter>();
        services.AddSingleton<IToolProblems, CachedToolProblems>();
        return services;
    }
}