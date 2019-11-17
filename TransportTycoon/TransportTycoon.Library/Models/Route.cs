namespace TransportTycoon.Library.Models
{
    public class Route
    {
        public Route(string endpoint1, string endpoint2, int cost)
        {
            this.endpoint1 = endpoint1;
            this.endpoint2 = endpoint2;
            this.cost = cost;
        }

        //

        private string endpoint1;
        private string endpoint2;
        private int cost;
    }
}