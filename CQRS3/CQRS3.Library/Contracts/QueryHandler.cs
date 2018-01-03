namespace CQRS3.Library.Contracts
{
    public interface QueryHandler<in TQuery, out TResult>
        where TQuery : Query<TResult>
    {
        TResult Handle(TQuery query);
    }
}