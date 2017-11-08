using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.F1.Utilities.Translators
{
    public static class F1FinancialTransaction
    {
        public static FinancialTransaction Translate( XElement inputTransaction )
        {
            var transaction = new FinancialTransaction();

            transaction.Id = inputTransaction.Attribute( "id" ).Value.AsInteger();

            if ( inputTransaction.Element( "batch" ).Attribute( "id" )?.Value.AsIntegerOrNull() != null )
            {
                transaction.BatchId = inputTransaction.Element( "batch" ).Attribute( "id" ).Value.AsInteger();
            }

            transaction.Summary = inputTransaction.Element( "memo" )?.Value;
            transaction.TransactionCode = inputTransaction.Element( "accountReference" )?.Value;
            transaction.TransactionDate = inputTransaction.Element( "receivedDate" )?.Value.AsDateTime();
            transaction.AuthorizedPersonId = inputTransaction.Element( "person" ).Attribute( "id" ).Value.AsInteger();

            var currencyType = inputTransaction.Element( "contributionType" ).Element( "name" ).Value;

            switch ( currencyType )
            {
                case "Cash":
                    transaction.CurrencyType = CurrencyType.Cash;
                    break;
                case "Check":
                    transaction.CurrencyType = CurrencyType.Check;
                    break;
                case "Credit Card":
                    transaction.CurrencyType = CurrencyType.CreditCard;
                    break;
                case "ACH":
                    transaction.CurrencyType = CurrencyType.ACH;
                    break;
                case "Non-Cash":
                    transaction.CurrencyType = CurrencyType.NonCash;
                    break;
                default:
                    transaction.CurrencyType = CurrencyType.Unknown;
                    break;
            }

            transaction.CreatedDateTime = inputTransaction.Element( "createdDate" )?.Value.AsDateTime();
            transaction.CreatedByPersonId = inputTransaction.Element( "createdByPerson" ).Attribute( "id" )?.Value.AsIntegerOrNull();

            transaction.ModifiedDateTime = inputTransaction.Element( "lastUpdateDate" )?.Value.AsDateTime();
            transaction.ModifiedByPersonId = inputTransaction.Element( "lastUpdatedByPerson" ).Attribute( "id" )?.Value.AsIntegerOrNull();

            return transaction;
        }
    }
}
