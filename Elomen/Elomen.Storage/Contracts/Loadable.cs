namespace Elomen.Storage.Contracts
{
    public interface Loadable<out T>
    {
        T Load();
    }
}