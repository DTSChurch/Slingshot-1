using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Slingshot.Core.Model;

namespace Slingshot.City.Utilities.Translators
{
    public static class CityGroup
    {
        public static Group Translate(DataRow row)
        {
            var group = new Group();
            string groupName = row.Field<string>("name");
            group.Name = groupName;

            int groupId = row.Field<int>("id");
            group.Id = groupId;

            int parentGroupId = row.Field<int>("parent_group_id");
            group.Id = groupId;

            //group.description
            // generate a unique group id
            /*
            MD5 md5Hasher = MD5.Create();
            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(groupName));
            var groupId = Math.Abs(BitConverter.ToInt32(hashed, 0)); // used abs to ensure positive number
            if (groupId > 0)
            {
                group.Id = groupId;
            }
            */

            // using the "Imported Group" group type
            string groupTypeName = row.Field<string>("group_type");
            var varGroupTypeId = TheCityExport.GroupTypes.FirstOrDefault(kvp=>kvp.Key.Contains(groupTypeName)).Value;
            group.GroupTypeId = varGroupTypeId;

            int parGroupId = row.Field<int>("parent_group_id");
            group.ParentGroupId = parGroupId;
            return group;
        }
    }
}
