using System.Data;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioGroup
    {
        public static Group Translate( DataRow row )
        {
            var group = new Group();

            group.Id = row.Field<int>( "Id" );
            group.Name = row.Field<string>( "Name" );
            var groupTypeId = row.Field<int>( "GroupTypeId" );

            group.GroupTypeId = groupTypeId;

            group.ParentGroupId = 2099999000 + groupTypeId;

            return group;
        }
    }
}
