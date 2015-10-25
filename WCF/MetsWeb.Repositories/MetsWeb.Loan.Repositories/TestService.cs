using System;
using System.Collections.Generic;
using MetsWeb.Common.Models;

namespace MetsWeb.Loan.Repositories
{
    public class TestService : ITestService
    {
        public List<Test> GetTests()
        {
            return new List<Test>
            {
                new Test {TestID=1,TestName="Test 1" },
                new Test {TestID=2,TestName="Test 2" }
            };
        }
    }
}
