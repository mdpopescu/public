namespace Elomen.Storage.Contracts
{
    public interface DataEncryptor
    {
        string EncryptForUser(string data);
        string DecryptForUser(string data);

        string EncryptForMachine(string data);
        string DecryptForMachine(string data);
    }
}