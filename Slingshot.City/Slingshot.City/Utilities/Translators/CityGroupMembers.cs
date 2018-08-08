using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityGroupMembers
    {
        public static GroupMember Translate(DataRow row)
        {
            var groupMember = new GroupMember();
            groupMember.PersonId = row.Field<int>("id");
            groupMember.Role = row.Field<string>("role_in_group");
            groupMember.GroupId = row.Field<int>("group_id");
            return groupMember;
        }
    }
}
