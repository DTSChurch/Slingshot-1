﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioPerson
    {
        public static Person Translate( DataRow row )
        {
            var person = new Person();

            if ( row.Field<int?>( "Id" ) != null )
            {
                person.Id = row.Field<int>( "Id" );

                // names
                person.FirstName = row.Field<string>( "FirstName" );
                person.NickName = row.Field<string>( "NickName" );
                person.MiddleName = row.Field<string>( "MiddleName" );
                person.LastName = row.Field<string>( "LastName" );

                var suffix = row.Field<string>( "Suffix" );
                switch ( suffix )
                {
                    case "v":
                        person.Suffix = "V";
                        break;
                    case "mr":
                        break;
                    case "Jr":
                        person.Suffix = "Jr.";
                        break;
                    case "Sr":
                        person.Suffix = "Sr.";
                        break;
                }

                // family
                person.FamilyId = row.Field<int?>( "FamilyId" );
                person.FamilyName = row.Field<string>( "FamilyName" );

                var familyRole = row.Field<string>( "familyRole" );
                switch ( familyRole )
                {
                    case "Household Head":
                        person.FamilyRole = FamilyRole.Adult;
                        break;
                    case "Spouse of Head":
                        person.FamilyRole = FamilyRole.Adult;
                        break;
                    case "Child 1st":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 2nd":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 3rd":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 4th":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 5th":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 6th":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 7th":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    case "Child 8th":
                        person.FamilyRole = FamilyRole.Child;
                        break;
                    default:
                        person.FamilyRole = FamilyRole.Child;
                        break;
                }

                // email
                person.Email = row.Field<string>( "Email" );
                var emailPreference = row.Field<string>( "EmailOptOut" ).AsBoolean();
                if ( emailPreference )
                {
                    person.EmailPreference = EmailPreference.DoNotEmail;
                }
                else
                {
                    person.EmailPreference = EmailPreference.EmailAllowed;
                }


                // gender
                var gender = row.Field<bool>( "Gender" );

                if ( gender == true )
                {
                    person.Gender = Gender.Male;
                }
                else if ( gender == false )
                {
                    person.Gender = Gender.Female;
                }

                // marital status
                var maritalStatus = row.Field<string>( "MaritalStatus" );
                switch ( maritalStatus )
                {
                    case "Married":
                        person.MaritalStatus = MaritalStatus.Married;
                        break;
                    case "Single":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Divorce":
                        person.MaritalStatus = MaritalStatus.Divorced;
                        break;
                    default:
                        person.MaritalStatus = MaritalStatus.Unknown;
                        if ( maritalStatus.IsNotNullOrWhitespace() )
                        {
                            person.Note = "MaritalStatus:" + maritalStatus;
                        }
                        break;
                }

                // connection/record status
                string status = row.Field<string>( "ConnectionStatus" );
                person.ConnectionStatus = status;

                person.RecordStatus = RecordStatus.Active;

                switch ( status )
                {
                    case "Deceased":
                        person.RecordStatus = RecordStatus.Inactive;
                        person.InactiveReason = "Deceased";
                        person.IsDeceased = true;
                        break;
                    case "Moved Away":
                        person.RecordStatus = RecordStatus.Inactive;
                        break;
                    case "Drop Out Reason Unknown":
                        person.RecordStatus = RecordStatus.Inactive;
                        break;
                    case "Left to Another Church":
                        person.RecordStatus = RecordStatus.Inactive;
                        break;
                }

                // dates
                person.CreatedDateTime = row.Field<DateTime?>( "CreatedDateTime" );
                person.ModifiedDateTime = row.Field<DateTime?>( "ModifiedDateTime" );
                person.Birthdate = row.Field<DateTime?>( "Birthdate" );
                person.AnniversaryDate = row.Field<DateTime?>( "AnniversaryDate" );

                // gives individually
                var givesIndividually = row.Field<bool?>( "GiveIndividually" );
                if ( givesIndividually.HasValue )
                {
                    person.GiveIndividually = givesIndividually;
                }

                // campus
                var campusName = row.Field<string>( "Campus" );
                var CampusId = row.Field<int>( "CampusId" );
                if ( campusName.IsNotNullOrWhitespace() && CampusId > 0 )
                {
                    var campus = new Campus();
                    campus.CampusName = campusName;
                    campus.CampusId = CampusId;

                    person.Campus = campus;
                }

                // attributes
                var school = row.Field<string>( "School" );
                if ( school.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "School",
                        AttributeValue = school,
                        PersonId = person.Id
                    } );
                }

                var race = row.Field<string>( "Race" );
                if ( race.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Race",
                        AttributeValue = race,
                        PersonId = person.Id
                    } );
                }

                var occupation = row.Field<string>( "Occupation" );
                if ( occupation.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Occupation",
                        AttributeValue = occupation,
                        PersonId = person.Id
                    } );
                }

                var education = row.Field<string>( "Education" );
                if ( education.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Education",
                        AttributeValue = education,
                        PersonId = person.Id
                    } );
                }

                var baptismDate = row.Field<DateTime?>( "BaptismDate" );
                if ( baptismDate != null )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "BaptismDate",
                        AttributeValue = baptismDate.Value.ToString( "o" ),
                        PersonId = person.Id
                    } );
                }

                var baptizedDate = row.Field<string>( "BaptizedHere" );
                if ( baptizedDate.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "BaptizedHere",
                        AttributeValue = baptizedDate,
                        PersonId = person.Id
                    } );
                }

                // phone numbers
                // home
                var homePhone = row.Field<string>( "HomePhone" );
                if ( homePhone.IsNotNullOrWhitespace() )
                {
                    // since the phone number could have invalid information, all non digits will be removed
                    if ( homePhone.AsNumeric().IsNotNullOrWhitespace() && homePhone.AsNumeric().Count() <= 20 )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PhoneNumber = homePhone.AsNumeric(),
                            PersonId = person.Id,
                            PhoneType = "Home"
                        } );
                    }
                }

                // cell
                var cellPhone = row.Field<string>( "MobilePhone" );
                if ( cellPhone.IsNotNullOrWhitespace() )
                {
                    // since the phone number could have invalid information, all non digits will be removed
                    if( cellPhone.AsNumeric().IsNotNullOrWhitespace() && cellPhone.AsNumeric().Count() <= 20 )
                    {
                        var isMessagingEnabled = row.Field<string>( "IsMessagingEnabled" ).AsBoolean();

                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PhoneNumber = cellPhone.AsNumeric(),
                            PersonId = person.Id,
                            PhoneType = "Mobile",
                            IsMessagingEnabled = isMessagingEnabled
                        } );
                    }
                }

                // work
                var workPhone = row.Field<string>( "WorkPhone" );
                if ( workPhone.IsNotNullOrWhitespace() )
                {
                    // since the phone number could have invalid information, all non digits will be removed
                    if ( workPhone.AsNumeric().IsNotNullOrWhitespace() && workPhone.AsNumeric().Count() <= 20 )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PhoneNumber = workPhone.AsNumeric(),
                            PersonId = person.Id,
                            PhoneType = "Work"
                        } );
                    }
                }

                // household address
                var street = row.Field<string>( "Street" );
                var city = row.Field<string>( "City" );
                var state = row.Field<string>( "State" );
                var zip = row.Field<string>( "ZipCode" );
                var country = row.Field<string>( "Country" );

                if ( street.IsNotNullOrWhitespace() &&
                     city.IsNotNullOrWhitespace() &&
                     state.IsNotNullOrWhitespace() &&
                     zip.IsNotNullOrWhitespace() )
                {
                    person.Addresses.Add( new PersonAddress
                    {
                        PersonId = person.Id,
                        Street1 = street,
                        City = city,
                        State = state,
                        PostalCode = zip,
                        Country = country
                    } );
                }

                return person;
            }

            return null;
        }
    }
}
