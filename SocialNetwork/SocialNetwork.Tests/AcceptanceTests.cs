using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork.Library.Services;

namespace SocialNetwork.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private CommandHandler sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new CommandHandler(new InMemoryRepository(), new TimeFormatter());

      Scenario1_Posting();
    }

    [TestMethod]
    public void Scenario2_Reading()
    {
      Scenario1_Posting();

      var response1 = sut.Read("Alice").ToList();
      Assert.AreEqual(1, response1.Count);
      Assert.AreEqual("I love the weather today (5 minutes ago)", response1[0]);

      var response2 = sut.Read("Bob").ToList();
      Assert.AreEqual(2, response2.Count);
      Assert.AreEqual("Good game though. (1 minute ago)", response2[0]);
      Assert.AreEqual("Damn! We lost! (2 minutes ago)", response2[1]);
    }

    [TestMethod]
    public void Scenario3_Following()
    {
      Scenario1_Posting();

      sut.Post("Charlie", "I'm in New York today! Anyone want to have a coffee?");

      sut.Follow("Charlie", "Alice");
      var response1 = sut.Wall("Charlie").ToList();
      Assert.AreEqual(2, response1.Count);
      Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response1[0]);
      Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response1[1]);

      sut.Follow("Charlie", "Bob");
      var response2 = sut.Wall("Charlie").ToList();
      Assert.AreEqual(4, response2.Count);
      Assert.AreEqual("Charlie - I'm in New York today! Anyone want to have a coffee? (2 seconds ago)", response1[0]);
      Assert.AreEqual("Bob - Good game though. (1 minute ago)", response1[1]);
      Assert.AreEqual("Bob - Damn! We lost! (2 minutes ago)", response1[2]);
      Assert.AreEqual("Alice - I love the weather today (5 minutes ago)", response1[3]);
    }

    //

    private void Scenario1_Posting()
    {
      sut.Post("Alice", "I love the weather today");
      sut.Post("Bob", "Damn! We lost!");
      sut.Post("Bob", "Good game though.");
    }
  }
}