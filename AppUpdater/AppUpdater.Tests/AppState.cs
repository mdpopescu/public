using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Renfield.AppUpdater.Tests
{
  internal class AppState
  {
    private readonly Mock<AutoUpdaterView> view;
    private Image ActualImage { get; set; }

    public Image ExpectedImage { get; set; }
    public string ToolTip { get; set; }
    public bool? CheckVisible { get; set; }
    public bool? UpdateVisible { get; set; }
    public bool? ProgressVisible { get; set; }
    public bool? Enlarge { get; set; }
    public bool? MoveLeft { get; set; }

    public AppState(Mock<AutoUpdaterView> view)
    {
      this.view = view;
    }

    public void SetUp()
    {
      if (ExpectedImage != null)
        view.Setup(it => it.SetImage(It.IsAny<Image>())).Callback<Image>(it => ActualImage = it);

      if (ToolTip != null)
        view.Setup(it => it.SetToolTip(ToolTip));

      if (CheckVisible != null)
        view.Setup(it => it.SetCheckVisible(CheckVisible.Value));

      if (UpdateVisible != null)
        view.Setup(it => it.SetUpdateVisible(UpdateVisible.Value));

      if (ProgressVisible != null)
        view.Setup(it => it.SetProgressVisible(ProgressVisible.Value));

      if (Enlarge != null)
        if (Enlarge.Value)
          view.Setup(it => it.Enlarge());
        else
          view.Setup(it => it.Shrink());

      if (MoveLeft != null)
        if (MoveLeft.Value)
          view.Setup(it => it.MoveLeft());
        else
          view.Setup(it => it.MoveRight());
    }

    public void Check()
    {
      if (ExpectedImage != null)
        Assert.IsTrue(ExpectedImage.IsIdenticalTo(ActualImage));

      view.VerifyAll();
    }
  }
}