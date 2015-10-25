using System.Collections.Generic;
using System.ServiceModel;

namespace MetsWeb.Loan.Repositories
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        List<MetsWeb.Common.Models.Test> GetTests();
    }
}
