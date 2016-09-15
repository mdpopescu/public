namespace SocialNetwork2.Library.Interfaces
{
    public interface IHandlerFactory
    {
        IHandler GetHandler(string command);
    }
}