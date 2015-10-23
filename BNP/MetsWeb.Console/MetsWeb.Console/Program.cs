using MetsWeb.Console.LoanServiceReference;
using System;
using System.Collections.Generic;

namespace MetsWeb.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LoanServiceClient client = new LoanServiceClient();
                List<MetsWeb.Loan.Models.Loan> loans = client.GetLoans();
                client.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.GetBaseException());
            }
        }
    }
}
