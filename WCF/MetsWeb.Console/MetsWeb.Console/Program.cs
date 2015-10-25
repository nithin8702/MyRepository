using MetsWeb.Console.LoanServiceReference;
using MetsWeb.Console.TestServiceReference;
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
                LoanServiceClient client1 = new LoanServiceClient();
                List<Loan> loans = client1.GetLoans();

                TestServiceClient client2 = new TestServiceClient();
                List<Test> tests = client2.GetTests();


                client1.Close();
                client2.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.GetBaseException());
            }
        }
    }
}
