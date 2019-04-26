namespace Shop.Module.MQ.Abstractions.Models
{
    public interface IMQMessage<T>
    {
        string Queue { get; }

        T Value { get; }
    }
}
