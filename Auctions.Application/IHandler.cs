namespace Auctions.Application;

public interface IHandler<in T, V> 
    where T : class 
    where V : Result
{
    Task<V> Handle(T command);
}
