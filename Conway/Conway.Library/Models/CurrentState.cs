using System.Collections.Generic;

namespace Conway.Library.Models
{
    public class CurrentState
    {


        //

        private readonly IDictionary<Coordinates, CellState> currentState = new Dictionary<Coordinates, CellState>();
    }
}