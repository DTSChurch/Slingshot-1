using System;
using System.Data;
using System.Linq;

using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityFinancialBatch
    {
        public static FinancialBatch Translate(DataRow row)
        {
            var financialBatch = new FinancialBatch();
            Int32 BatchId = row.Field<Int32>("id");
            financialBatch.Id = BatchId;
            financialBatch.Name = row.Field<string>("name");
            financialBatch.StartDate = row.Field<DateTime>("batch_date");
            financialBatch.EndDate = row.Field<DateTime>("batch_date");
            string status = row.Field<string>("batch_state");
            switch (status.ToUpper().Trim())
            {
                case "PENDING":
                    financialBatch.Status = BatchStatus.Pending;
                    break;
                case "POSTED":
                    financialBatch.Status = BatchStatus.Closed;
                    break;
                default:
                    financialBatch.Status = BatchStatus.Pending;
                    break;
            }
            financialBatch.CreatedDateTime = row.Field<DateTime>("created_at");
            financialBatch.ModifiedDateTime = row.Field<DateTime>("created_at");
            //var varControlAmt = TheCityExport.BatchTotals.FirstOrDefault(kvp => kvp.Key.Equals(BatchId)).Value;
            //financialBatch.ControlAmount = varControlAmt;
            return financialBatch;
        }
    }
}
