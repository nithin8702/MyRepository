using System;
using System.Collections.Generic;

namespace MetsWeb.Loan.Repositories
{
    public class LoanService : ILoanService
    {
        public List<Models.Loan> GetLoans()
        {
            List<Models.Loan> loans = null;
            try
            {
                loans = new List<Models.Loan>
                {
                    new Models.Loan { LoanID=1,Comments="Comments 1" },
                    new Models.Loan { LoanID=2,Comments="Comments 2" }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
            return loans;
        }
    }
}
