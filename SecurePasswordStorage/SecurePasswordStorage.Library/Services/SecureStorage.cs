using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class SecureStorage : ISecureStorage
    {
        public SecureStorage(ICrypto crypto, ISecurityLogic securityLogic, IRepository repository)
        {
            this.crypto = crypto;
            this.securityLogic = securityLogic;
            this.repository = repository;
        }

        public void Save(Credentials loginCredentials, Credentials foreignCredentials)
        {
            // TODO: loginCredentials should be of type VerifiedCredentials
            // TODO: and we should have an Authenticator with a Verify method

            var secureHash = crypto.GetSecureHash(loginCredentials);
            var user = repository.LoadUser(loginCredentials.Username, secureHash);
            if (user == null)
                return;

            var encryptedCredentials = securityLogic.GetEncryptedCredentials(loginCredentials, foreignCredentials);
            repository.SaveEncryptedCredentials(loginCredentials.Username, encryptedCredentials);
        }

        //

        private readonly ICrypto crypto;
        private readonly ISecurityLogic securityLogic;
        private readonly IRepository repository;
    }
}