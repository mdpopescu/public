using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.AppUpdater.Helper;
using Renfield.AppUpdater.Properties;

namespace Renfield.AppUpdater.Tests
{
  [TestClass]
  public class AutoUpdaterPresenterTests
  {
    private Mock<AutoUpdaterView> view;
    private AutoUpdaterPresenter sut;

    [TestInitialize]
    public void SetUp()
    {
      view = new Mock<AutoUpdaterView>();
      sut = new AutoUpdaterPresenter(view.Object);
    }

    [TestMethod]
    public void TestInitialize()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.ok,
        ToolTip = Resources.AppStatusOk,
        CheckVisible = true,
        UpdateVisible = false,
        ProgressVisible = false,
      };
      state.SetUp();

      sut.Initialize();

      state.Check();
    }

    [TestMethod]
    public void TestSetStateToOk()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.ok,
        ToolTip = Resources.AppStatusOk,
        CheckVisible = true,
        UpdateVisible = false,
      };
      state.SetUp();

      sut.SetState(AutoUpdaterState.Ok);

      state.Check();
    }

    [TestMethod]
    public void TestSetStateToError()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.exclamation,
        ToolTip = Resources.AppStatusError,
        CheckVisible = true,
        UpdateVisible = false,
      };
      state.SetUp();

      sut.SetState(AutoUpdaterState.Error);

      state.Check();
    }

    [TestMethod]
    public void TestSetStateToUpdateExists()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.information,
        ToolTip = Resources.AppStatusUpdateFound,
        CheckVisible = false,
        UpdateVisible = true,
      };
      state.SetUp();

      sut.SetState(AutoUpdaterState.UpdateExists);

      state.Check();
    }

    [TestMethod]
    public void TestSetStateToChecking()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.clock_network,
        ToolTip = Resources.AppStatusChecking,
        CheckVisible = false,
        UpdateVisible = false,
      };
      state.SetUp();

      sut.SetState(AutoUpdaterState.Checking);

      state.Check();
    }

    [TestMethod]
    public void TestSetStateToUpdating()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.clock_network,
        ToolTip = Resources.AppStatusDownloading,
        CheckVisible = false,
        UpdateVisible = false,
        ProgressVisible = true,
        Enlarge = true,
      };
      state.SetUp();

      view.SetupGet(it => it.IsRightAnchored).Returns(false);

      sut.SetState(AutoUpdaterState.Updating);

      state.Check();
    }

    [TestMethod]
    public void ChangingStateToUpdatingMovesToLeftWhenAnchoredRight()
    {
      var state = new AppState(view)
      {
        ExpectedImage = Resources.clock_network,
        ToolTip = Resources.AppStatusDownloading,
        CheckVisible = false,
        UpdateVisible = false,
        ProgressVisible = true,
        Enlarge = true,
        MoveLeft = true,
      };
      state.SetUp();

      view.SetupGet(it => it.IsRightAnchored).Returns(true);

      sut.SetState(AutoUpdaterState.Updating);

      state.Check();
    }

    [TestMethod]
    public void ChangingStateFromUpdatingMovesToRightWhenAnchoredRight()
    {
      sut.SetState(AutoUpdaterState.Updating);

      var state = new AppState(view)
      {
        ProgressVisible = false,
        Enlarge = false,
        MoveLeft = false,
      };
      state.SetUp();

      view.SetupGet(it => it.IsRightAnchored).Returns(true);

      sut.SetState(AutoUpdaterState.Ok);

      state.Check();
    }
  }
}