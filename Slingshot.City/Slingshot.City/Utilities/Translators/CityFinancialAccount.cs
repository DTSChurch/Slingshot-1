using System;
using System.Data;

using Slingshot.Core.Model;
using System.Threading.Tasks;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityFinancialAccount
    {
        public static FinancialAccount Translate(DataRow  row)
        {
            var financialAccount = new FinancialAccount();
            financialAccount.Id = row.Field<Int32>("id");
            financialAccount.Name = row.Field<string>("name");

            bool isTaxDed = false;
            switch (row.Field<string>("tax_deductible").ToUpper().Trim())
            {
                case "TRUE":
                    isTaxDed = true;
                    break;
                default:
                    isTaxDed = false;
                    break;
            }
            financialAccount.IsTaxDeductible = isTaxDed;
            return financialAccount;
        }
    }
}
