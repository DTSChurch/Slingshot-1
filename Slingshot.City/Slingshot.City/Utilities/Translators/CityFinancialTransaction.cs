using System;
using System.Data;

using Slingshot.Core.Model;
namespace Slingshot.City.Utilities.Translators
{
    public static class CityFinancialTransaction
    {
        public static FinancialTransaction Translate (DataRow row)
        {
            var financialTransaction = new FinancialTransaction();
            financialTransaction.Id = row.Field<Int32>("id");
            financialTransaction.BatchId = row.Field<Int32>("batch_id");
            financialTransaction.AuthorizedPersonId = row.Field<Int32>("user_id");
            financialTransaction.TransactionDate = row.Field<DateTime>("donation_date");
            switch(row.Field<string>("instrument_type").ToUpper().Trim())
            {
                case "ACH":
                    financialTransaction.CurrencyType = CurrencyType.ACH;
                    break;
                case "CHECK":
                    financialTransaction.CurrencyType = CurrencyType.Check;
                    break;
                case "CASH":
                    financialTransaction.CurrencyType = CurrencyType.Cash;
                    break;
                case "CC":
                    financialTransaction.CurrencyType = CurrencyType.CreditCard;
                    break;
                default:
                    financialTransaction.CurrencyType = CurrencyType.Unknown;
                    break;
            }
            financialTransaction.CreatedDateTime = row.Field<DateTime>("created_at");
            financialTransaction.ModifiedDateTime = row.Field<DateTime>("updated_at");
            return financialTransaction;
            
        }
    }
}
