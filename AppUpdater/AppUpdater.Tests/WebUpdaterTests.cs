using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.AppUpdater.Helper;

namespace Renfield.AppUpdater.Tests
{
  [TestClass]
  public class WebUpdaterTests
  {
    private Mock<Presenter> presenter;
    private Mock<ManifestReader> reader;
    private WebUpdater sut;
    private Mock<Loader> loader;
    private Mock<Helper.System> launcher;

    private const string URI = "http://indietracking.com/Content/IndieTrackingSetup.exe";
    private const string CURRENT_VERSION = "current_version";
    private const string NEW_VERSION = "new_version";
    private const string DESTINATION = @"c:\temp";
    private const int PERCENTAGE = 50;

    [TestInitialize]
    public void SetUp()
    {
      presenter = new Mock<Presenter>();
      reader = new Mock<ManifestReader>();
      loader = new Mock<Loader>();
      launcher = new Mock<Helper.System>();

      sut = new WebUpdater(presenter.Object, () => loader.Object);
    }

    [TestMethod]
    public void CheckForUpdateSetsStateToChecking()
    {
      presenter.Setup(it => it.SetState(AutoUpdaterState.Checking)).Verifiable();

      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      presenter.Verify();
    }

    [TestMethod]
    public void CheckForUpdateSetsStateToOkIfSameVersion()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      reader.SetupGet(it => it.Version).Returns(CURRENT_VERSION);
      presenter.Setup(it => it.SetState(AutoUpdaterState.Ok)).Verifiable();

      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      presenter.Verify();
    }

    [TestMethod]
    public void CheckForUpdateSetsStateToUpdateExistsIfDifferentVersion()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      reader.SetupGet(it => it.Version).Returns(NEW_VERSION);
      presenter.Setup(it => it.SetState(AutoUpdaterState.UpdateExists)).Verifiable();

      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      presenter.Verify();
    }

    [TestMethod]
    public void CheckForUpdateSetsStateToErrorIfNeeded()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Error += null, (Exception) null);
      presenter.Setup(it => it.SetState(AutoUpdaterState.Error)).Verifiable();

      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      presenter.Verify();
    }

    [TestMethod]
    public void UpdateSetsStateToUpdating()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      presenter.Setup(it => it.SetState(AutoUpdaterState.Updating)).Verifiable();

      sut.Update(DESTINATION, launcher.Object);

      presenter.Verify();
    }

    [TestMethod]
    public void UpdateLaunchesIfSuccessful()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      loader.Setup(it => it.LoadToDisk(URI, It.IsAny<string>())).Raises(it => it.Completed += null, (string) null);
      launcher.Setup(it => it.Launch(It.IsAny<string>())).Verifiable();

      sut.Update(DESTINATION, launcher.Object);

      launcher.Verify();
    }

    [TestMethod]
    public void UpdateTerminatesCurrentApplicationIfSuccessful()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      loader.Setup(it => it.LoadToDisk(URI, It.IsAny<string>())).Raises(it => it.Completed += null, (string) null);
      launcher.Setup(it => it.EndCurrentApplication()).Verifiable();

      sut.Update(DESTINATION, launcher.Object);

      launcher.Verify();
    }

    [TestMethod]
    public void UpdateSetsPercentageWhenRequired()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      loader.Setup(it => it.LoadToDisk(URI, It.IsAny<string>())).Raises(it => it.Progress += null, PERCENTAGE);
      presenter.Setup(it => it.SetProgress(PERCENTAGE));

      sut.Update(DESTINATION, launcher.Object);

      launcher.Verify();
    }

    [TestMethod]
    public void UpdateSetsStateToErrorIfNeeded()
    {
      loader.Setup(it => it.LoadInMemory(URI)).Raises(it => it.Completed += null, (string) null);
      reader.SetupGet(it => it.URL).Returns(URI);
      sut.CheckForUpdate(URI, CURRENT_VERSION, reader.Object);

      loader.Setup(it => it.LoadToDisk(URI, It.IsAny<string>())).Raises(it => it.Error += null, (Exception) null);
      presenter.Setup(it => it.SetState(AutoUpdaterState.Error)).Verifiable();

      sut.Update(DESTINATION, launcher.Object);

      presenter.Verify();
    }
  }
}