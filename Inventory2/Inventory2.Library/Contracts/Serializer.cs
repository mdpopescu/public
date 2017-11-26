namespace Inventory2.Library.Contracts
{
    public interface Serializer<TRepresentation, TValue>
    {
        TRepresentation Serialize(TValue value);
        TValue Deserialize(TRepresentation serialized);
    }
}