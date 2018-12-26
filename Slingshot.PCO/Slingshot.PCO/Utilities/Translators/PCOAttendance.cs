using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;
using Slingshot.PCO.Models;

namespace Slingshot.PCO.Utilities.Translators
{
    public static class PCOAttendance
    {
        public static List<Attendance> Translate( List<PCOCheckIn> inputAttendance )
        {
            List<Attendance> attendanceList = new List<Attendance>();

            foreach ( var attendee in inputAttendance )
            {
                if ( attendee.person_id.HasValue && attendee.event_period.starts_at.HasValue )
                {

                    var attendance = new Attendance();
                    attendanceList.Add( attendance );
                    attendance.PersonId = attendee.person_id.Value;
                    attendance.StartDateTime = attendee.event_period.starts_at.Value;
                    attendance.GroupId = attendee.location.id;
                }
            }

            return attendanceList;
        }
    }
}
