using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class SecurityLogic : ISecurityLogic
    {
        public SecurityLogic(ICrypto crypto)
        {
            this.crypto = crypto;
        }

        public EncryptedCredentials GetEncryptedCredentials(Credentials loginCredentials, Credentials foreignCredentials)
        {
            var largeHash = crypto.GetLargeHash(loginCredentials);
            return new EncryptedCredentials
            {
                Encrypted = crypto.Encrypt(largeHash.PartOne, foreignCredentials),
                Hashed = crypto.GetSecureHash(largeHash.PartTwo),
            };
        }

        //

        private readonly ICrypto crypto;
    }
}