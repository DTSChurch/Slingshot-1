using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityPerson
    {
        public static Person Translate(DataRow row)
        {
            var person = new Person();
            var notes = new List<string>();

            // person id
            int? personId = row.Field<int?>("id");
            if (personId != null)
            {
                person.Id = personId.Value;
            }
           
            
            // names
            string firstName = row.Field<string>("first");
            if (firstName.IsNotNullOrWhitespace())
            {
                person.FirstName = firstName;
            }

            string middleName = row.Field<string>("middle");
            if (middleName.IsNotNullOrWhitespace())
            {
                person.MiddleName = middleName;
            }

            string lastName = row.Field<string>("last");
            if (lastName.IsNotNullOrWhitespace())
            {
                person.LastName = lastName;
            }

            string nickName = row.Field<string>("preferred_name");
            if (nickName.IsNotNullOrWhitespace())
            {
                person.NickName = nickName;
            }

            // email
            string email = row.Field<string>("email");
            if (email.IsNotNullOrWhitespace())
            {
                person.Email = email;
            }
            
            //birthdate
            DateTime birthdate = row.Field<DateTime>("birthdate");
            if (birthdate != null)
            {
                person.Birthdate = birthdate;
            }

            //isActive
            string isActive = row.Field<string>("active");
            if (isActive.IsNotNullOrWhitespace())
            {
                switch (isActive.Trim().ToUpper())
                {
                    case "FALSE":
                        person.RecordStatus = RecordStatus.Inactive;
                        break;
                    case "TRUE":
                        person.RecordStatus = RecordStatus.Active;
                        break;
                    default:
                        person.RecordStatus = RecordStatus.Inactive;
                        break;
                }
            }
            else
            {
                person.RecordStatus = RecordStatus.Inactive;
            }

            //gender
            string gender = row.Field<string>("gender");
            if (gender.IsNotNullOrWhitespace())
            {
                if (gender == "Male")
                {
                    person.Gender = Gender.Male;
                }
                else if (gender == "Female")
                {
                    person.Gender = Gender.Female;
                }
            }

            // marital status
            string maritalStatus = row.Field<string>("family_role");
            if(maritalStatus == null)
            {
                person.MaritalStatus = MaritalStatus.Single;
            }
            else
            {
                switch (maritalStatus.Trim())
                {
                    case "Spouse":
                        person.MaritalStatus = MaritalStatus.Married;
                        break;
                    case "Child":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "TempChild":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "LegalChild":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Married":
                        person.MaritalStatus = MaritalStatus.Married;
                        break;
                    case "Re-Married":
                        person.MaritalStatus = MaritalStatus.Married;
                        break;
                    case "Divorced":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Never Married":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Widowed":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Engaged":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Separated":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Single":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    case "Cohabiting":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;

                    default:
                        person.MaritalStatus = MaritalStatus.Unknown;
                        notes.Add("Marital Status: " + maritalStatus);
                        break;
                }
            }
            
            // family
            int? familyId = row.Field<int?>("family_id");
            if (familyId != null)
            {
                person.FamilyId = familyId;
            }

            string familyRole = row.Field<string>("family_role");
            switch (familyRole)
            {
                case "Spouse":
                    person.FamilyRole = FamilyRole.Adult;
                    break;
                case "LegalChild":
                    person.FamilyRole = FamilyRole.Child;
                    break;
                case "Child":
                    person.FamilyRole = FamilyRole.Child;
                    break;
                default:
                    person.FamilyRole = FamilyRole.Child;
                    notes.Add("Family Postion: Other");
                    break;
            }

            // connection status
            /*string connectionStatus = row.Field<string>("member");
            string constat = "Visitor";
            if (connectionStatus.IsNotNullOrWhitespace())
            {
                switch (connectionStatus.ToUpper().Trim())
                {
                    case "YES":
                        constat = "Member";
                        break;
                    case "NO":
                        constat = "Visitor";
                        break;
                }
                person.ConnectionStatus = constat;
            }
           */
            DateTime connectionStatus = row.Field<DateTime>("Membership Date");
            string constat = "Visitor";
            if(connectionStatus > DateTime.MinValue)
            {
                constat = "Member";                       
            } 
            person.ConnectionStatus = constat;

            // record status
            /*
            string recordStatus = row.Field<string>("active");
            if (recordStatus.IsNotNullOrWhitespace())
            {
                switch (recordStatus)
                {
                    case "A":
                        person.RecordStatus = RecordStatus.Active;
                        break;
                    default:
                        person.RecordStatus = RecordStatus.Inactive;
                        break;
                }
            }
            */
            // write out import notes
            if (notes.Count > 0)
            {
                person.Note = string.Join(",", notes);
            }
           
            //primary phone
            var primaryPhone = row.Field<string>("primary_phone");
            var primaryPhoneType = row.Field<string>("primary_phone_type");
            if (!String.IsNullOrWhiteSpace(primaryPhone) && !String.IsNullOrWhiteSpace(primaryPhoneType))
            {
                person.PhoneNumbers.Add(new PersonPhone
                {
                    PersonId = person.Id,
                    PhoneType = primaryPhoneType,
                    PhoneNumber = primaryPhone
                });
            }
            //secondary phone 
            var secondaryPhone = row.Field<string>("secondary_phone");
            var secondaryPhoneType = row.Field<string>("secondary_phone_type");
            if(!String.IsNullOrWhiteSpace(secondaryPhone) && !String.IsNullOrWhiteSpace(secondaryPhoneType))
            {
                person.PhoneNumbers.Add(new PersonPhone
                {
                    PersonId = person.Id,
                    PhoneType = secondaryPhoneType,
                    PhoneNumber = secondaryPhone
                });
            }

            // addresses
            var importAddress = new PersonAddress();
            importAddress.PersonId = person.Id;
            importAddress.Street1 = row.Field<string>("street");
            importAddress.Street2 = row.Field<string>("street2");
            importAddress.City = row.Field<string>("city");
            importAddress.State = row.Field<string>("state");
            importAddress.PostalCode = row.Field<string>("zipcode");
            var addressType = row.Field<string>("location_type");
            switch (addressType)
            {
                case "Home":
                    {
                        importAddress.AddressType = AddressType.Home;
                        break;
                    }
            }

            // only add the address if we have a valid address
            if (importAddress.Street1.IsNotNullOrWhitespace() &&
                    importAddress.City.IsNotNullOrWhitespace() &&
                    importAddress.PostalCode.IsNotNullOrWhitespace())
            {
                person.Addresses.Add(importAddress);
            }

            //loop through attributes found. 
            foreach(var attrib in TheCityExport.PersonAttributes)
            {
                string value;

                if (attrib.Value == "String")
                {
                    value = row.Field<string>(attrib.Key);
                }
                else if (attrib.Value == "DateTime")
                {
                    var datetime = row.Field<DateTime?>(attrib.Key);
                    if (datetime.HasValue)
                    {
                        value = datetime.Value.ToString("o");
                    }
                    else
                    {
                        value = "";
                    }
                }
                else
                {
                    value = null;
                }

                if (value.IsNotNullOrWhitespace())
                {
                    person.Attributes.Add(new PersonAttributeValue
                    {
                        AttributeKey = attrib.Key,
                        AttributeValue = value.ToString(),
                        PersonId = person.Id
                    });
                }
            }
            return person;
        }
    }
}
