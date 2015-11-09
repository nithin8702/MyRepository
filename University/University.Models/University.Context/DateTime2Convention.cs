using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace University.Context
{
    public class DateTime2Convention : Convention
    {
        public DateTime2Convention()
        {
            this.Properties<DateTime>()
                .Configure(c => c.HasColumnType("datetime2"));
        }
    }
}
