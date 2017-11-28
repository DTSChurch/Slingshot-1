using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioFinancialTransactionDetail
    {
        public static FinancialTransactionDetail Translate( DataRow row )
        {
            var financialTransactionDetail = new FinancialTransactionDetail();

            financialTransactionDetail.TransactionId = row.Field<int>( "TransactionId" );
            financialTransactionDetail.AccountId = row.Field<int>( "AccountId" );

            financialTransactionDetail.CreatedDateTime = row.Field<DateTime?>( "CreatedDateTime" );
            financialTransactionDetail.ModifiedDateTime = row.Field<DateTime?>( "ModifiedDateTime" );

            return financialTransactionDetail;
        }
    }
}
