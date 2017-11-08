using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.F1.Utilities.Translators
{
    public static class F1Person
    {
        public static Person Translate( XElement inputPerson, List<PersonAttribute> personAttributes )
        {
            var person = new Person();
            var notes = new List<string>();

            if ( inputPerson.Attribute( "id" ) != null && inputPerson.Attribute( "id" ).Value.AsIntegerOrNull().HasValue )
            {
                person.Id = inputPerson.Attribute( "id" ).Value.AsInteger();

                // names
                person.FirstName = inputPerson.Element( "firstName" ).Value;
                person.NickName = inputPerson.Element( "goesByName" )?.Value;
                person.MiddleName = inputPerson.Element( "middleName" )?.Value;
                person.LastName = inputPerson.Element( "lastName" )?.Value;

                person.Salutation = inputPerson.Element( "salutation" )?.Value;

                var suffix = inputPerson.Element( "suffix" )?.Value;
                if ( suffix.Equals( "Sr.", StringComparison.OrdinalIgnoreCase ) )
                {
                    person.Suffix = "Sr.";
                }
                else if ( suffix.Equals( "Jr.", StringComparison.OrdinalIgnoreCase ) )
                {
                    person.Suffix = "Jr.";
                }
                else if ( suffix.Equals( "Ph.D.", StringComparison.OrdinalIgnoreCase ) )
                {
                    person.Suffix = "Ph.D.";
                }
                else
                {
                    person.Suffix = suffix;
                }

                // communcations (phone & email)
                var communicationsList = inputPerson.Element( "communications" ).Elements( "communication" );
                foreach ( var comm in communicationsList )
                {
                    if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Home Phone" )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PersonId = person.Id,
                            PhoneType = "Home",
                            PhoneNumber = comm.Element( "communicationValue" ).Value
                        } );
                    }
                    else if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Work Phone" )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PersonId = person.Id,
                            PhoneType = "Work",
                            PhoneNumber = comm.Element( "communicationValue" ).Value
                        } );
                    }
                    else if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Mobile" )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PersonId = person.Id,
                            PhoneType = "Mobile",
                            PhoneNumber = comm.Element( "communicationValue" ).Value
                        } );
                    }
                    else if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Email" )
                    {
                        person.Email = comm.Element( "communicationValue" ).Value;
                    }
                }

                // addresses
                var addressList = inputPerson.Element( "addresses" ).Elements( "address" );
                foreach ( var address in addressList )
                {
                    if ( address.Element( "address1" ) != null && address.Element( "address1" ).Value.IsNotNullOrWhitespace() )
                    {
                        var importAddress = new PersonAddress();
                        importAddress.PersonId = person.Id;
                        importAddress.Street1 = address.Element( "address1" ).Value;
                        importAddress.Street2 = address.Element( "address2" )?.Value;
                        importAddress.City = address.Element( "city" ).Value;
                        importAddress.State = address.Element( "stProvince" ).Value;
                        importAddress.PostalCode = address.Element( "postalCode" ).Value;
                        importAddress.Country = address.Element( "country" )?.Value;

                        var addressType = address.Element( "addressType" ).Element( "name" ).Value;

                        switch ( addressType )
                        {
                            case "Primary":
                                importAddress.AddressType = AddressType.Home;
                                break;
                            case "Previous":
                                importAddress.AddressType = AddressType.Previous;
                                break;
                            case "Businiess":
                                importAddress.AddressType = AddressType.Work;
                                break;
                        }

                        // only add the address if we have a valid address
                        if ( importAddress.Street1.IsNotNullOrWhitespace() && importAddress.City.IsNotNullOrWhitespace() && importAddress.PostalCode.IsNotNullOrWhitespace() )
                        {
                            person.Addresses.Add( importAddress );
                        }
                    }
                }

                // gender
                var gender = inputPerson.Element( "gender" )?.Value;

                if ( gender == "Male" )
                {
                    person.Gender = Gender.Male;
                }
                else if ( gender == "Female" )
                {
                    person.Gender = Gender.Female;
                }
                else
                {
                    person.Gender = Gender.Unknown;
                }

                // marital status
                var maritalStatus = inputPerson.Element( "maritalStatus" )?.Value;
                switch ( maritalStatus )
                {
                    case "Married":
                        person.MaritalStatus = MaritalStatus.Married;
                        break;
                    case "Single":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    default:
                        person.MaritalStatus = MaritalStatus.Unknown;
                        if ( maritalStatus.IsNotNullOrWhitespace() )
                        {
                            notes.Add( "maritalStatus: " + maritalStatus );
                        }
                        break;
                }

                // connection status
                person.ConnectionStatus = inputPerson.Element( "status" ).Element( "name" )?.Value;


                // record status
                if ( inputPerson.Element( "status" ).Element( "name" )?.Value == "Inactive Member" )
                {
                    person.RecordStatus = RecordStatus.Inactive;
                }
                else
                {
                    person.RecordStatus = RecordStatus.Active;
                }

                // dates
                person.Birthdate = inputPerson.Element( "dateOfBirth" )?.Value.AsDateTime();
                person.CreatedDateTime = inputPerson.Element( "createdDate" )?.Value.AsDateTime();
                person.ModifiedDateTime = inputPerson.Element( "lastUpdatedDate" )?.Value.AsDateTime();

                // family
                person.FamilyId = inputPerson.Attribute( "householdID" )?.Value.AsInteger();

                if ( inputPerson.Element( "householdMemberType" ).Element( "name" )?.Value == "Head" ||
                     inputPerson.Element( "householdMemberType" ).Element( "name" )?.Value == "Spouse" )
                {
                    person.FamilyRole = FamilyRole.Adult;
                }
                else if ( inputPerson.Element( "householdMemberType" ).Element( "name" )?.Value == "Child" )
                {
                    person.FamilyRole = FamilyRole.Child;
                }
                else
                {
                    // likely the person is a visitor and should belong to their own family
                    person.FamilyRole = FamilyRole.Child;
                    person.FamilyId = null;
                }

                // campus

                // Note: Most F1 Churches use sub status to track the campus.
                Campus campus = new Campus();
                person.Campus = campus;

                string campusName = inputPerson.Element( "status" ).Element( "subStatus" ).Element( "name" )?.Value;
                if ( campusName.IsNotNullOrWhitespace() )
                {
                    campus.CampusName = campusName;

                    // generate a unique campus id
                    MD5 md5Hasher = MD5.Create();
                    var hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( campusName ) );
                    var campusId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
                    if ( campusId > 0 )
                    {
                        campus.CampusId = campusId;
                    }
                }

                // person attributes

                // Note: People from F1 can contain orphaned person attributes that could cause
                //  the slingshot import to fail.  To prevent that, we'll check each attribute
                //  and make sure it exists first before importing.
                //
                // There is also the possibility that someone can have two person attributes with
                //  the same key. 
                var attributes = inputPerson.Element( "attributes" );
                var usedAttributeKeys = new List<string>();

                foreach ( var attribute in attributes.Elements() )
                {
                    if ( personAttributes.Any() )
                    {
                        var attributeKey = attribute.Element( "attributeGroup" ).Element( "attribute" ).Element( "name" ).Value.RemoveSpaces().RemoveSpecialCharacters();
                        
                        if ( personAttributes.Where( p => attributeKey.Equals( p.Key ) ).Any() )
                        {
                            usedAttributeKeys.Add( attributeKey );

                            if ( usedAttributeKeys.Where( a => attributeKey.Equals( a ) ).Count() <= 1 )
                            {
                                StringBuilder attribValue = new StringBuilder();

                                DateTime? startDate = attribute.Element( "startDate" )?.Value.AsDateTime();
                                DateTime? endDate = attribute.Element( "endDate" )?.Value.AsDateTime();
                                string comment = attribute.Element( "comment" ).Value;

                                if ( startDate.HasValue )
                                {
                                    attribValue.Append( "Start Date: " + startDate.Value.ToShortDateString() );
                                }

                                if ( startDate.HasValue && endDate.HasValue )
                                {
                                    attribValue.Append( " " );
                                }

                                if ( endDate.HasValue )
                                {
                                    attribValue.Append( "End Date: " + endDate.Value.ToShortDateString() );
                                }

                                if ( comment.IsNotNullOrWhitespace() && ( startDate.HasValue || endDate.HasValue ) )
                                {
                                    attribValue.Append( " " );
                                }

                                if ( comment.IsNotNullOrWhitespace() )
                                {
                                    attribValue.Append( "Comment: " + comment );
                                }

                                // People in F1 can have an attribute assigned without a value.
                                //  For these cases, the value will be set to Yes.
                                if ( !startDate.HasValue && !endDate.HasValue && comment.IsNullOrWhiteSpace() )
                                {
                                    attribValue.Append( "Yes" );
                                }

                                person.Attributes.Add( new PersonAttributeValue
                                {
                                    AttributeKey = attributeKey,
                                    AttributeValue = attribValue.ToString(),
                                    PersonId = person.Id
                                } );
                            }
                        }                       
                    }
                }

                // person requirements
                var requirements = inputPerson.Element( "peopleRequirements" );
                foreach ( var requirement in requirements.Elements() )
                {
                    string requirementId = requirement.Element( "requirement" ).Attribute( "id" ).Value;

                    var requirementStatus = requirement.Element( "requirementStatus" ).Element( "name" ).Value;
                    var requirementStatusKey = requirementId + "_" + requirement.Element( "requirement" ).Element( "name" ).Value
                                                    .RemoveSpaces().RemoveSpecialCharacters() + "Status";

                    if ( personAttributes.Where( p => requirementStatusKey.Equals( p.Key ) ).Any() )
                    {
                        usedAttributeKeys.Add( requirementStatusKey );

                        if ( usedAttributeKeys.Where( a => requirementStatusKey.Equals( a ) ).Count() <= 1 )
                        {
                            person.Attributes.Add( new PersonAttributeValue
                            {
                                AttributeKey = requirementStatusKey,
                                AttributeValue = requirementStatus,
                                PersonId = person.Id
                            } );
                        }
                    }

                    DateTime? requirementDate = requirement.Element( "requirementDate" ).Value.AsDateTime();
                    var requirementDateKey = requirementId + "_" + requirement.Element( "requirement" ).Element( "name" ).Value
                                                    .RemoveSpaces().RemoveSpecialCharacters() + "Date";

                    if ( requirementDate != null )
                    {
                        if ( personAttributes.Where( p => requirementDateKey.Equals( p.Key ) ).Any() )
                        {
                            usedAttributeKeys.Add( requirementDateKey );

                            if ( usedAttributeKeys.Where( a => requirementDateKey.Equals( a ) ).Count() <= 1 )
                            {
                                person.Attributes.Add( new PersonAttributeValue
                                {
                                    AttributeKey = requirementDateKey,
                                    AttributeValue = requirementDate.Value.ToString( "o" ),
                                    PersonId = person.Id
                                } );
                            }
                        }
                    }
                }

                // person fields

                // occupation
                string occupation = inputPerson.Element( "occupation" ).Element( "name" ).Value;
                if ( occupation.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Position",
                        AttributeValue = occupation,
                        PersonId = person.Id
                    } );
                }

                // employer
                string employer = inputPerson.Element( "employer" ).Value;
                if ( employer.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Employer",
                        AttributeValue = employer,
                        PersonId = person.Id
                    } );
                }

                // school
                string school = inputPerson.Element( "school" ).Element( "name" ).Value;
                if ( school.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "School",
                        AttributeValue = school,
                        PersonId = person.Id
                    } );
                }

                // denomination
                string denomination = inputPerson.Element( "denomination" ).Element( "name" ).Value;
                if ( denomination.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Denomination",
                        AttributeValue = denomination,
                        PersonId = person.Id
                    } );
                }

                // former Church
                string formerChurch = inputPerson.Element( "formerChurch" ).Value;
                if ( formerChurch.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "PreviousChurch",
                        AttributeValue = formerChurch,
                        PersonId = person.Id
                    } );
                }


                // write out person notes
                if ( notes.Count() > 0 )
                {
                    person.Note = string.Join( ",", notes );
                }

            }

            return person;
        }
    }
}
