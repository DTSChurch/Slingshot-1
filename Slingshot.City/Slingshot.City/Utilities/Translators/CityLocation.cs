using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityLocation
    {
        public static Location Translate(DataRow row)
        {
            var location = new Location();
            int id = row.Field<int>("id");
            location.Id = id;
            string Street1 = row.Field<string>("street");
            location.Street1 = Street1;
            string Street2 = row.Field<string>("street2");
            location.Street2 = Street2;
            string City = row.Field<string>("City");
            location.City = City;
            string State = row.Field<string>("state");
            location.State = State;
            string ZipCode = row.Field<string>("zipcode");
            location.PostalCode = ZipCode;
            return location
        ;
        }
    }
}
