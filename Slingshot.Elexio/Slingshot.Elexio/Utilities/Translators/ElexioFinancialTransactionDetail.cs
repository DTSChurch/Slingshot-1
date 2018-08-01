using System;
using System.Data;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioFinancialTransactionDetail
    {
        public static FinancialTransactionDetail Translate( DataRow row )
        {
            var financialTransactionDetail = new FinancialTransactionDetail();

            financialTransactionDetail.TransactionId = row.Field<int>( "Id" );
            financialTransactionDetail.AccountId = row.Field<int>( "AccountId" );
            financialTransactionDetail.Amount = row.Field<decimal>( "Amount" );

            financialTransactionDetail.CreatedDateTime = row.Field<DateTime?>( "CreatedDateTime" );
            financialTransactionDetail.ModifiedDateTime = row.Field<DateTime?>( "ModifiedDateTime" );

            return financialTransactionDetail;
        }
    }
}
