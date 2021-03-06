﻿using System.Linq;
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

        public void SaveUser(Credentials credentials)
        {
            var (salt, hash) = crypto.GenerateHash(credentials);
            var user = new User(credentials.Key, salt, hash);
            userRepository.Save(user);
        }

        public User LoadUser(Credentials credentials)
        {
            var user = userRepository.Load(credentials.Key);
            if (user == null)
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            if (!crypto.VerifyHash(credentials, ByteArrayTuple.Create(user.Salt, user.PasswordHash)))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            return user;
        }

        public bool CheckUser(Credentials credentials)
        {
            var user = userRepository.Load(credentials.Key);
            return user != null && crypto.VerifyHash(credentials, ByteArrayTuple.Create(user.Salt, user.PasswordHash));
        }

        public void SaveSecret(Credentials credentials, byte[] secret)
        {
            var (secretKey, verificationHash) = CheckCredentials(credentials);

            var encryptedSecret = crypto.Encrypt(secretKey, secret);
            var secretData = new SecretData(credentials.Key, encryptedSecret, verificationHash);
            secretRepository.Save(secretData);
        }

        public byte[] LoadSecret(Credentials credentials)
        {
            var (secretKey, verificationHash) = CheckCredentials(credentials);

            var secretData = secretRepository.Load(credentials.Key);
            if (!secretData.VerificationHash.SequenceEqual(verificationHash))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            return crypto.Decrypt(secretKey, secretData.EncryptedSecret);
        }

        //

        private readonly ICryptoFacade crypto;
        private readonly IUserRepository userRepository;
        private readonly ISecretRepository secretRepository;

        private ByteArrayTuple CheckCredentials(Credentials credentials)
        {
            if (!CheckUser(credentials))
                throw new SecurityException(Constants.AUTHENTICATION_ERROR);

            return crypto.Transform(credentials);
        }
    }
}