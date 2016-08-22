namespace TweetNicer.Library.Interfaces
{
    public interface Encoder<TOriginal, TEncoded>
    {
        TOriginal Encode(TEncoded value);
        TEncoded Decode(TOriginal data);
    }
}