using System.Linq;
using System.Security;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Helpers;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class SecureStorage : ISecureStorage
    {
        public SecureStorage(ICryptoFacade crypto, IUserRepository userRepository, ISecretRepository secretRepository)
        {
            this.crypto = crypto;
            this.userRepository = userRepository;
            this.secretRepository = secretRepository;
        }

        public void Save(Credentials credentials, byte[] secret)
        {
            CheckCredentials(credentials, out var secretKey, out var verificationHash);

            var encryptedSecret = crypto.Encrypt(secretKey, secret);
            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            secretRepository.Save(secretData);
        }

        public byte[] Load(Credentials credentials)
        {
            CheckCredentials(credentials, out var secretKey, out var verificationHash);

            var secretData = secretRepository.Load(credentials.Key);
            if (!secretData.VerificationHash.SequenceEqual(verificationHash))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            return crypto.Decrypt(secretKey, secretData.EncryptedSecret);
        }

        //

        private readonly ICryptoFacade crypto;
        private readonly IUserRepository userRepository;
        private readonly ISecretRepository secretRepository;

        private void CheckCredentials(Credentials credentials, out byte[] secretKey, out byte[] verificationHash)
        {
            var user = userRepository.Load(credentials.Key);
            if (user == null)
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            var saltedCredentials = credentials.GetSaltedCredentials(user.Salt);
            //var passwordHash = crypto.SecureHash(saltedCredentials);
            // passwordHash == user.Salt + hash(saltedCredentials)
            if (!crypto.VerifyHash(user.PasswordHash, saltedCredentials))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            crypto.Transform(credentials, out secretKey, out verificationHash);
        }
    }
}