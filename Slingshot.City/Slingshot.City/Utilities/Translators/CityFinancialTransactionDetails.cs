using System;
using System.Data;

using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityFinancialTransactionDetails
    {
        public static FinancialTransactionDetail Translate(DataRow row)
        {
            var financialTransactionDetail = new FinancialTransactionDetail();
            financialTransactionDetail.Id = row.Field<Int32>("id");
            financialTransactionDetail.TransactionId = row.Field <Int32>("id");
            financialTransactionDetail.AccountId = row.Field<Int32>("fund_id");
            financialTransactionDetail.Amount = Convert.ToDecimal(row.Field<double>("amount"));
            financialTransactionDetail.Summary = row.Field<string>("note");
            financialTransactionDetail.CreatedDateTime = row.Field<DateTime>("created_at");
            financialTransactionDetail.ModifiedDateTime = row.Field<DateTime>("updated_at");
            return financialTransactionDetail;
        }

    }
}
