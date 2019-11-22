namespace TransportTycoon.Library.Models
{
    public class Link
    {
        public Endpoint E1 { get; }
        public Endpoint E2 { get; }

        public Link(Endpoint e1, Endpoint e2)
        {
            E1 = e1;
            E2 = e2;
        }
    }
}