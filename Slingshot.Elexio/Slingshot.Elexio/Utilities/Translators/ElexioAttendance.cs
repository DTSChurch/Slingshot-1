using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioAttendance
    {
        public static Attendance Translate( DataRow row )
        {
            var attendance = new Attendance();

            attendance.AttendanceId = row.Field<int>( "AttendanceId" );
            attendance.PersonId = row.Field<int>( "PersonId" );

            var groupId = row.Field<int?>( "GroupId" );
            if ( groupId.HasValue )
            {
                attendance.GroupId = groupId;
            }

            var campusId = row.Field<int?>( "CampusId" );
            if ( campusId.HasValue )
            {
                attendance.CampusId = campusId;
            }

            var locationId = row.Field<int?>( "LocationId" );
            if ( locationId.HasValue )
            {
                attendance.LocationId = locationId;
            }

            attendance.StartDateTime = row.Field<DateTime>( "StartDateTime" );

            var endDateTime = row.Field<DateTime?>( "EndDateTime" );
            if ( endDateTime.HasValue )
            {
                attendance.EndDateTime = endDateTime.Value;
            }

            return attendance;
        }
    }
}
