using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Library.Services
{
  public class SecureStorage : Storage
  {
    public SecureStorage(StringIO io, Serializer<LicenseRegistration> serializer)
    {
      this.io = io;
      this.serializer = serializer;
    }

    public Encryptor Encryptor { get; set; }

    public LicenseRegistration Load()
    {
      var encrypted = io.Read();
      if (string.IsNullOrWhiteSpace(encrypted))
        return new LicenseRegistration();

      var decrypted = Encryptor == null ? encrypted : Encryptor.Decrypt(encrypted);

      return serializer.Deserialize(decrypted);
    }

    public void Save(LicenseRegistration registration)
    {
      var decrypted = serializer.Serialize(registration);
      var encrypted = Encryptor == null ? decrypted : Encryptor.Encrypt(decrypted);

      io.Write(encrypted);
    }

    //

    private readonly StringIO io;
    private readonly Serializer<LicenseRegistration> serializer;
  }
}