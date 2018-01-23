using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioFinancialBatch
    {
        public static FinancialBatch Translate( DataRow row )
        {
            var financialBatch = new FinancialBatch();

            financialBatch.Id = row.Field<int>( "Id" );
            financialBatch.Name = row.Field<string>( "Name" );

            return financialBatch;
        }
    }
}
