namespace TweetNicer.Library.Interfaces
{
    public interface Serializer<T>
    {
        string Serialize(T value);
        T Deserialize(string data);
    }
}