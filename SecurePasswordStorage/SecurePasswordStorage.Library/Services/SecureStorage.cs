using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using SecurePasswordStorage.Library.Contracts;
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
            var user = userRepository.Load(credentials.Key);
            var passwordHash = crypto.SecureHash(credentials.Password);
            if (!IsValid(user, passwordHash))
                throw new SecurityException("Invalid user credentials.");

            var (pl, pr) = crypto.LargeHash(credentials.Password);
            var secureKey = crypto.SecureHash(pl);
            var encryptedSecret = crypto.Encrypt(secureKey, secret);
            var verificationHash = crypto.SecureHash(pr);
            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            secretRepository.Save(secretData);
        }

        //

        private readonly ICryptoFacade crypto;
        private readonly IUserRepository userRepository;
        private readonly ISecretRepository secretRepository;

        private static bool IsValid(User user, IEnumerable<byte> passwordHash) =>
            user != null && user.PasswordHash.SequenceEqual(passwordHash);
    }
}