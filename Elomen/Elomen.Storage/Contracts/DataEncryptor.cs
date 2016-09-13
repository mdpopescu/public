namespace Elomen.Storage.Contracts
{
    public interface DataEncryptor
    {
        string Encrypt(Location location, string data, string password);
        string Decrypt(Location location, string data, string password);
    }
}