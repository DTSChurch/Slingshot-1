using System;
using System.Data;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioFinancialAccount
    {
        public static FinancialAccount Translate( DataRow row )
        {
            var financialAccount = new FinancialAccount();

            financialAccount.Id = row.Field<Int16>( "CodeId" );
            financialAccount.Name = row.Field<string>( "Description" );

            return financialAccount;
        }
    }
}
