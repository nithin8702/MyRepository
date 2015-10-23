using System.Collections.Generic;
using System.ServiceModel;

namespace MetsWeb.Loan.Repositories
{
    [ServiceContract]
    public interface ILoanService
    {
        [OperationContract]
        List<Models.Loan> GetLoans();
    }
}
