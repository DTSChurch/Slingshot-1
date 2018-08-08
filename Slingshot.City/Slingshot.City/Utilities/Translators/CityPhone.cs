using System;
using System.Data;

using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityPhone
    {
        public static PersonPhone Translate( DataRow row)
        {
            var phone = new PersonPhone();
            phone.PersonId = row.Field<int>("id");
            phone.PhoneNumber = row.Field<string>("Phone");
            phone.IsUnlisted = row.Field<bool?>("Unlisted").Value;
            var phoneType = row.Field<string>("Description");
            switch (phoneType)
            {
                case "MOBILE":
                    phone.PhoneType = "Mobile";
                    break;
                case "HOME":
                    phone.PhoneType = "Home";
                    break;
                case "WORK":
                    phone.PhoneType = "Work";
                    break;
                default:
                    phone.PhoneType = phoneType;
                    break;
            }
            return phone;
        }
    }
}
