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

            var transactionDate = row.Field<DateTime?>( "TransactionDate" );
            if ( transactionDate.HasValue )
            {
                financialTransaction.TransactionDate = transactionDate;
            }
            else
            {
                financialTransaction.TransactionDate = DateTime.Parse( "01/01/1901" );
            }

            financialTransaction.Summary = row.Field<string>( "Summary" );

            financialTransaction.TransactionType = TransactionType.Contribution;

            var givingMethod = row.Field<string>( "GivingMethod" );
            switch ( givingMethod )
            {
                // built-in Elexio giving methods
                case "<Unknown>":
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;
                case "Online-Credit Card":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    financialTransaction.TransactionSource = TransactionSource.Website;
                    break;
                case "Online-eCheck":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    financialTransaction.TransactionSource = TransactionSource.Website;
                    break;
                case "Online-Reversal":
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;
                case "Fund Adjustment":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    financialTransaction.TransactionSource = TransactionSource.Website;
                    break;
                case "SMS Donation":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    financialTransaction.TransactionSource = TransactionSource.MobileApplication;
                    break;

                // other common methods
                case "Unknown":
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;
                case "Check":
                    financialTransaction.CurrencyType = CurrencyType.Check;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;
                case "Cash":
                    financialTransaction.CurrencyType = CurrencyType.Cash;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;
                case "Credit Card":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    financialTransaction.TransactionSource = TransactionSource.Website;
                    break;
                case "Automatic Deposit":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;

                // grace church methods
                case "Non-cash":
                    financialTransaction.CurrencyType = CurrencyType.NonCash;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;
                case "Old Online Credit Card":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    financialTransaction.TransactionSource = TransactionSource.Website;
                    break;
                case "Old Online eCheck":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    financialTransaction.TransactionSource = TransactionSource.Website;
                    break;
                case "Smart Giving - ACH":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    financialTransaction.TransactionSource = TransactionSource.MobileApplication;
                    break;
                case "Smart Giving - Cards":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    financialTransaction.TransactionSource = TransactionSource.MobileApplication;
                    break;
                case "Other":
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    financialTransaction.TransactionSource = TransactionSource.OnsiteCollection;
                    break;

                default:
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    financialTransaction.Summary += ( " Giving Method: " + givingMethod );
                    break;
            }

            financialTransaction.CreatedDateTime = row.Field<DateTime?>( "CreatedDateTime" );
            financialTransaction.ModifiedDateTime = row.Field<DateTime?>( "ModifiedDateTime" );

            return financialTransaction;
        }
    }
}
