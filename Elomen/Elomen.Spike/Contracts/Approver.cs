namespace Elomen.Spike.Contracts
{
    public interface Approver
    {
        string Authorize(string url);
    }
}