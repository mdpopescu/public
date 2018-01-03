using CQRS3.Implementations.Events;
using CQRS3.Implementations.Queries;
using CQRS3.Library.Contracts;

namespace CQRS3.Implementations
{
    public class Counter : StateBase, QueryHandler<GetValueQuery, int>
    {
        public override void Play(EventBase ev)
        {
            switch (ev)
            {
                case Incremented _:
                    value++;
                    break;

                case Decremented _:
                    value--;
                    break;
            }
        }

        public int Handle(GetValueQuery query)
        {
            return value;
        }

        //

        private int value;
    }
}