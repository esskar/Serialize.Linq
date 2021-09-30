namespace Serialize.Linq.Interfaces
{
    public interface IGenericSerializer<TSerialize> : IGenericNodeSerializer<TSerialize>, IGenericExpressionSerializer<TSerialize> { }
}
