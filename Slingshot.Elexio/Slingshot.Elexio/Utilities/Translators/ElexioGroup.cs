using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            group.GroupTypeId = row.Field<int>( "GroupTypeId" );

            return group;
        }
    }
}
