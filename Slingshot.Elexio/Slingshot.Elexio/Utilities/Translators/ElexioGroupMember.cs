using System.Data;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioGroupMember
    {
        public static GroupMember Translate( DataRow row )
        {
            var groupMember = new GroupMember();

            groupMember.PersonId = row.Field<int>( "PersonId" );
            groupMember.GroupId = row.Field<int>( "GroupId" );
            groupMember.Role = row.Field<string>( "Role" );

            return groupMember;
        }
    }
}
