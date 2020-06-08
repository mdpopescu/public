namespace Secure.Contracts
{
    public interface ITransformer<in TContext, in TInput, out TOutput>
    {
        TOutput Transform(TContext context, TInput value);
    }
}