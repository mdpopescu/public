using System.Collections.Generic;
using System.Linq;
using System.Security;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Helpers;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class SecureStorage
    {
        public SecureStorage(ICryptoFacade crypto, IUserRepository userRepository, ISecretRepository secretRepository)
        {
            this.crypto = crypto;
            this.userRepository = userRepository;
            this.secretRepository = secretRepository;
        }

        public void Save(Credentials credentials, byte[] secret)
        {
            CheckCredentials(credentials);
            crypto.Transform(credentials, out var secretKey, out var verificationHash);

            var encryptedSecret = crypto.Encrypt(secretKey, secret);
            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            secretRepository.Save(secretData);
        }

        public byte[] Load(Credentials credentials)
        {
            CheckCredentials(credentials);
            crypto.Transform(credentials, out var secretKey, out var verificationHash);

            var secretData = secretRepository.Load(credentials.Key);
            if (!secretData.VerificationHash.SequenceEqual(verificationHash))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            return crypto.Decrypt(secretKey, secretData.EncryptedSecret);
        }

        //

        private readonly ICryptoFacade crypto;
        private readonly IUserRepository userRepository;
        private readonly ISecretRepository secretRepository;

        private void CheckCredentials(Credentials credentials)
        {
            var user = userRepository.Load(credentials.Key);
            var passwordHash = crypto.SecureHash(credentials.Password);
            if (!IsValid(user, passwordHash))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);
        }

        private static bool IsValid(User user, IEnumerable<byte> passwordHash) =>
            user != null && user.PasswordHash.SequenceEqual(passwordHash);
    }
}