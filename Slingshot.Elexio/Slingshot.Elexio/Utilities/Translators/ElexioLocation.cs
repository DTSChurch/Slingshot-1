using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioLocation
    {
        public static Location Translate( DataRow row )
        {
            var location = new Location();

            location.Id = row.Field<int>( "Id" );
            location.Name = row.Field<string>( "Name" );

            // setting not available - setting all locations to active
            location.IsActive = true;

            return location;
        }
    }
}
