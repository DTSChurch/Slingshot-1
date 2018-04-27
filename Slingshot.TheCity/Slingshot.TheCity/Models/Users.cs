using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Slingshot.TheCity.Models
{
    public class UsersModel
    {
        [JsonProperty( PropertyName = "total_entries" )]
        public int TotalEntries { get; set; }

        [JsonProperty( PropertyName = "total_pages" )]
        public int TotalPages { get; set; }

        [JsonProperty( PropertyName = "per_page" )]
        public int PerPage { get; set; }

        [JsonProperty( PropertyName = "current_page" )]
        public int CurrentPage { get; set; }

        [JsonProperty( PropertyName = "users" )]
        public List<User> Users { get; set; }
    }

    public class User
    {
        [JsonProperty( PropertyName = "admin_url" )]
        public string AdminURL { get; set; }

        [JsonProperty( PropertyName = "api_url" )]
        public string APIURL { get; set; }

        [JsonProperty( PropertyName = "internal_url" )]
        public string InternalURL { get; set; }

        [JsonProperty( PropertyName = "updated_at" )]
        public string UpdatedAt { get; set; }

        [JsonProperty( PropertyName = "last_logged_in" )]
        public string LastLoggedIn { get; set; }

        [JsonProperty( PropertyName = "secondary_phone" )]
        public string SecondaryPhone { get; set; }

        [JsonProperty( PropertyName = "last_engaged" )]
        public string LastEngaged { get;set; }

        [JsonProperty( PropertyName = "title" )]
        public string Title { get; set; }

        [JsonProperty( PropertyName = "id" )]
        public int Id { get; set; }

        [JsonProperty( PropertyName = "first" )]
        public string First { get; set; }

        [JsonProperty( PropertyName = "primary_campus_name" )]
        public string PrimaryCampusName { get; set; }

        [JsonProperty( PropertyName = "last" )]
        public string Last { get; set; }

        [JsonProperty( PropertyName = "head_of_household" )]
        public bool HeadOfHousehold { get; set; }

        [JsonProperty( PropertyName = "nickname" )]
        public string NickName { get; set; }

        [JsonProperty( PropertyName = "active" )]
        public bool Active { get; set; }

        [JsonProperty( PropertyName = "primary_phone_type" )]
        public string PrimaryPhoneType { get; set; }

        [JsonProperty( PropertyName = "primary_phone" )]
        public string PrimaryPhone { get; set; }

        [JsonProperty( PropertyName = "member_since" )]
        public string MemberSince { get;set; }

        [JsonProperty( PropertyName = "birthdate" )]
        public string BirthDate { get;set; }

        [JsonProperty( PropertyName = "email_bouncing" )]
        public bool EmailBouncing { get; set; }

        [JsonProperty( PropertyName = "secondary_phone_type" )]
        public string SecondaryPhoneType { get; set; }

        [JsonProperty( PropertyName = "primary_campus_id" )]
        public int? PrimaryCampusId { get; set; }

        [JsonProperty( PropertyName = "contact_updated_at" )]
        public string ContactUpdatedAt { get;set; }

        [JsonProperty( PropertyName = "type" )]
        public string Type { get; set; }

        [JsonProperty( PropertyName = "staff" )]
        public bool? Staff { get; set; }

        [JsonProperty( PropertyName = "created_at" )]
        public string CreatedAt { get;set; }

        [JsonProperty( PropertyName = "gender" )]
        public string Gender { get; set; }

        [JsonProperty( PropertyName = "external_id_1" )]
        public string ExternalId1 { get; set; }

        [JsonProperty( PropertyName = "external_id_2" )]
        public string ExternalId2 { get; set; }

        [JsonProperty( PropertyName = "external_id_3" )]
        public string ExternalId3 { get; set; }

        [JsonProperty( PropertyName = "external_chms_id" )]
        public string ExternalChMSId { get; set; }

        [JsonProperty( PropertyName = "middle" )]
        public string Middle { get; set; }

        [JsonProperty( PropertyName = "email" )]
        public string Email { get; set; }
    }
}
