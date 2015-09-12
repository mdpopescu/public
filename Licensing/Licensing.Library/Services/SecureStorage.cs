using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class SecureStorage : Storage
  {
    public SecureStorage(StringIO io, Encryptor encryptor, Serializer<LicenseRegistration> serializer)
    {
      this.encryptor = encryptor;
      this.io = io;
      this.serializer = serializer;
    }

    public LicenseRegistration Load()
    {
      var encrypted = io.Read();
      var decrypted = encryptor.Decrypt(encrypted);

      return serializer.Deserialize(decrypted);
    }

    public void Save(LicenseRegistration registration)
    {
      var decrypted = serializer.Serialize(registration);
      var encrypted = encryptor.Encrypt(decrypted);

      io.Write(encrypted);
    }

    //

    private readonly Encryptor encryptor;
    private readonly StringIO io;
    private readonly Serializer<LicenseRegistration> serializer;
  }
}