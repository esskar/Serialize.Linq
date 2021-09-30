namespace Serialize.Linq.Interfaces
{
    interface IGenericSerializer<TSerialize> : IGenericNodeSerializer<TSerialize>, IGenericExpressionSerializer<TSerialize> { }
}
