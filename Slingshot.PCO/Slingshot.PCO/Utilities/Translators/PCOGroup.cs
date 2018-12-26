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
    public static class PCOGroup
    {
        public static List<Group> Translate( PCOEvent inputEvent, List<PCOCheckIn>inputCheckin, int parentGroupId )
        {
            List<Group> groups = new List<Group>();

            var group = new Group();

            group.Id = inputEvent.id;
            group.Name = inputEvent.name;

            group.ParentGroupId = parentGroupId;

            group.GroupTypeId = 1; //hard coded PCO Checkin Group Type

            group.IsActive = !inputEvent.archived_at.HasValue;

            groups.Add( group );

            var eventLocations = inputCheckin.Where( x => x.my_event.id == inputEvent.id )
                                            .Select( x => new { eventId = x.my_event.id, locationId = x.location.id , locationName = x.location.name } )
                                            .Distinct()
                                            .ToList();

           

            foreach ( var eventLocation in eventLocations )
            {
                group = new Group();
                group.Id = eventLocation.locationId;
                group.Name = eventLocation.locationName;
                group.ParentGroupId = eventLocation.eventId;

                var eventPeople = inputCheckin.Where( x => x.my_event.id == inputEvent.id && x.location.id == eventLocation.locationId )
                                           .Select( x => new { x.person_id, x.location.id, x.kind } )
                                           .Distinct()
                                           .ToList();

                group.IsActive = eventPeople.Any();
                foreach( var eventPerson in eventPeople )
                {
                    if( eventPerson.person_id.HasValue )
                    {
                        group.GroupMembers.Add( new GroupMember
                        {
                            GroupId = eventPerson.id,
                            PersonId = eventPerson.person_id.Value,
                            Role = eventPerson.kind
                        } );
                    }
                }
                groups.Add( group );
            }


            return groups;
        }
    }
}
