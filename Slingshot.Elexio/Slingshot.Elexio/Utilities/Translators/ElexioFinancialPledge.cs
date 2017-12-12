using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioFinancialPledge
    {
        public static FinancialPledge Translate( DataRow row )
        {
            var pledge = new FinancialPledge();

            pledge.Id = row.Field<int>( "Id" );
            pledge.PersonId = row.Field<int>( "PersonId" );
            pledge.AccountId = row.Field<int>( "AccountId" );

            var startDate = row.Field<DateTime?>( "StartDate" );
            if ( startDate.HasValue )
            {
                pledge.StartDate = startDate;
            }

            var endDate = row.Field<DateTime?>( "EndDate" );
            if ( endDate.HasValue )
            {
                pledge.EndDate = endDate;
            }

            var interval = row.Field<int>( "PledgeFrequency" );
            switch ( interval )
            {
                case 0:
                    pledge.PledgeFrequency = PledgeFrequency.OneTime;
                    break;
                case 7:
                    pledge.PledgeFrequency = PledgeFrequency.Weekly;
                    break;
                case 30:
                    pledge.PledgeFrequency = PledgeFrequency.Monthly;
                    break;
                case 90:
                    pledge.PledgeFrequency = PledgeFrequency.Quarterly;
                    break;
                case 365:
                    pledge.PledgeFrequency = PledgeFrequency.Yearly;
                    break;
                default:
                    pledge.PledgeFrequency = PledgeFrequency.OneTime;
                    break;
            }

            pledge.TotalAmount = (decimal)row.Field<double>( "TotalAmount" );

            var createdDateTime = row.Field<DateTime?>( "CreatedDateTime" );
            if ( createdDateTime.HasValue )
            {
                pledge.CreatedDateTime = createdDateTime;
            }

            var modifiedDateTime = row.Field<DateTime?>( "ModifiedDateTime" );
            if ( modifiedDateTime.HasValue )
            {
                pledge.ModifiedDateTime = modifiedDateTime;
            }

            return pledge;
        }
    }
}
