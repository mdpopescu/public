using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

/// <summary>Describes the detected setup of the AI server.</summary>
public interface ISetupPresenter
{
    /// <summary>Builds the view; a null snapshot means the setup was never checked.</summary>
    SetupView Present(SetupSnapshot? snapshot);
}