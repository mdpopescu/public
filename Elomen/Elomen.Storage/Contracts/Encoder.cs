namespace Elomen.Storage.Contracts
{
    public interface Encoder<TOriginal, TEncoded>
    {
        TOriginal Encode(TEncoded value);
        TEncoded Decode(TOriginal data);
    }
}