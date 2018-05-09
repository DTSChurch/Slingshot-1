using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.TheCity.Utilities.Translators
{
    public static class TheCityPerson
    {
        public static Person Translate( XElement inputPerson, XElement inputFamily )
        {
            var person = new Person();
            var notes = new List<string>();

            person.Id = inputPerson.Element( "id" ).Value.AsInteger();

            // names
            person.FirstName = inputPerson.Element( "first" ).Value;
            person.NickName = inputPerson.Element( "nickname" )?.Value;
            person.MiddleName = inputPerson.Element( "middle" )?.Value;
            person.LastName = inputPerson.Element( "last" )?.Value;
            person.Salutation = inputPerson.Element( "title" )?.Value;

            // gender
            var gender = inputPerson.Element( "gender" )?.Value;
            if ( gender.IsNotNullOrWhitespace() )
            {
                switch ( gender )
                {
                    case "Male":
                        person.Gender = Gender.Male;
                        break;
                    case "Female":
                        person.Gender = Gender.Female;
                        break;
                    default:
                        person.Gender = Gender.Unknown;
                        break;
                }
            }

            // email
            person.Email = inputPerson.Element( "email" )?.Value;

            // primary phone
            var primaryPhone = inputPerson.Element( "primary_phone" )?.Value;
            if ( primaryPhone.IsNotNullOrWhitespace() )
            {
                person.PhoneNumbers.Add( new PersonPhone
                {
                    PersonId = person.Id,
                    PhoneType = inputPerson.Element( "primary_phone_type" )?.Value,
                    PhoneNumber = primaryPhone
                } );
            }

            // secondary phone
            var secondaryPhone = inputPerson.Element( "secondary_phone_type" )?.Value;
            if ( secondaryPhone.IsNotNullOrWhitespace() )
            {
                person.PhoneNumbers.Add( new PersonPhone
                {
                    PersonId = person.Id,
                    PhoneType = inputPerson.Element( "secondary_phone_type" )?.Value,
                    PhoneNumber = secondaryPhone
                } );
            }


            // addresses

            // family 
            var familyId = inputFamily.Element( "id" )?.Value.AsIntegerOrNull();


            if ( familyId > 0 )
            {
                person.FamilyId = familyId;
                person.FamilyName = person.LastName + " Family";

                var familyMembers = inputFamily.Element( "family_members" );
                foreach ( var familyMember in familyMembers.Elements() )
                {
                    var personId = familyMember.Element( "user_id" ).Value.AsInteger();
                    if ( person.Id == personId )
                    {
                        var familyRole = familyMember.Element( "family_role" ).Value;
                        switch ( familyRole )
                        {
                            case "Spouse":
                                person.FamilyRole = FamilyRole.Adult;
                                break;
                            case "Child":
                                person.FamilyRole = FamilyRole.Child;
                                break;
                            default:
                                person.FamilyRole = FamilyRole.Child;
                                person.Note += "FamilyRole: " + familyRole;
                                break;
                        }
                    }
                } 
            }

            // campus
            var campusId = inputPerson.Element( "primary_campus_id" )?.Value.AsIntegerOrNull();
            if ( campusId.HasValue )
            {
                person.Campus = new Campus
                {
                    CampusId = campusId.Value,
                    CampusName = inputPerson.Element( "primary_campus_name" ).Value
                };
            }

            // dates
            person.Birthdate = inputPerson.Element( "birthdate" )?.Value.AsDateTime();
            person.CreatedDateTime = inputPerson.Element( "created_at" )?.Value.AsDateTime();
            person.ModifiedDateTime = inputPerson.Element( "updated_at" )?.Value.AsDateTime();

            // debug
            person.Note = inputPerson.Element( "type" )?.Value;

            return person;
        }
    }
}
