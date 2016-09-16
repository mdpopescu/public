namespace Elomen.Storage.Contracts
{
    public interface Encoder<TOriginal, TEncoded>
    {
        TEncoded Encode(TOriginal value);
        TOriginal Decode(TEncoded value);
    }
}