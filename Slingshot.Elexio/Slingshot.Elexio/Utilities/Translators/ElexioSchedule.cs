using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioSchedule
    {
        public static Schedule Translate( DataRow row )
        {
            var schedule = new Schedule();

            schedule.Id = row.Field<int>( "Id" );
            schedule.Name = row.Field<string>( "Name" );

            return schedule;
        }
    }
}
