using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class SecureStorage : ISecureStorage
    {
        public SecureStorage(ICrypto crypto, IRepository repository)
        {
            this.crypto = crypto;
            this.repository = repository;
        }

        public void Save(Credentials loginCredentials, Credentials foreignCredentials)
        {
            var secureHash = crypto.GetSecureHash(loginCredentials);
            var user = repository.LoadUser(loginCredentials.Username, secureHash);
            if (user == null)
                return;

            var largeHash = crypto.GetLargeHash(loginCredentials);
            var encrypted = crypto.Encrypt(largeHash.PartOne, foreignCredentials);
            var hashed = crypto.GetSecureHash(largeHash.PartTwo);
            var encryptedCredentials = new EncryptedCredentials { Encrypted = encrypted, Hashed = hashed };
            repository.SaveEncryptedCredentials(loginCredentials.Username, encryptedCredentials);
        }

        //

        private readonly ICrypto crypto;
        private readonly IRepository repository;
    }
}