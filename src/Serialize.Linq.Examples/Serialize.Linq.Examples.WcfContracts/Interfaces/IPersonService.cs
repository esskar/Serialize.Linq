using System.Collections.Generic;
using System.ServiceModel;
using Serialize.Linq.Examples.WcfContracts.Entities;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Examples.WcfContracts.Interfaces
{
    [ServiceContract]
    public interface IPersonService
    {
        [OperationContract]
        IList<Person> GetAllPersons();

        [OperationContract]
        IList<Person> FindPersons(ExpressionNode query);
    }
}
