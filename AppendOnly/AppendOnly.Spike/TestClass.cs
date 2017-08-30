using ProtoBuf;

namespace Renfield.AppendOnly.Spike
{
    [ProtoContract]
    public class TestClass
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Address { get; set; }
    }
}