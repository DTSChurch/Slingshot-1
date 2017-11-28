using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioFinancialTransaction
    {
        public static FinancialTransaction Translate( DataRow row )
        {
            var financialTransaction = new FinancialTransaction();

            financialTransaction.Id = row.Field<int>( "Id" );

            // if the financial transaction does not belong to a batch, add it to one.
            var batchId = row.Field<int?>( "BatchID" );
            if ( batchId.HasValue )
            {
                financialTransaction.BatchId = batchId.Value;
            }
            else
            {
                financialTransaction.BatchId = 9999;
            }

            financialTransaction.AuthorizedPersonId = row.Field<int>( "AuthorizedPersonId" );
            financialTransaction.TransactionDate = row.Field<DateTime>( "TransactionDate" );

            financialTransaction.Summary = row.Field<string>( "Summary" );

            financialTransaction.TransactionType = TransactionType.Contribution;

            var currencyType = row.Field<string>( "CurrencyType" );
            switch ( currencyType )
            {
                case "Check":
                    financialTransaction.CurrencyType = CurrencyType.Check;
                    break;
                case "Cash":
                    financialTransaction.CurrencyType = CurrencyType.Cash;
                    break;
                case "Credit Card":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    break;
                case "Automatic Deposit":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    break;
                default:
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    financialTransaction.Summary += ( "Currency Type: " + currencyType );
                    break;
            }

            financialTransaction.CreatedDateTime = row.Field<DateTime?>( "CreatedDateTime" );
            financialTransaction.ModifiedDateTime = row.Field<DateTime?>( "ModifiedDateTime" );

            return financialTransaction;
        }
    }
}
