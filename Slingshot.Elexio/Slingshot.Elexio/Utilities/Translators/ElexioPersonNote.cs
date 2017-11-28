using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Slingshot.Core.Model;

namespace Slingshot.Elexio.Utilities.Translators
{
    public static class ElexioPersonNote
    {
        public static PersonNote Translate( DataRow row )
        {
            var personNote = new PersonNote();

            personNote.Id = row.Field<int>( "Id" );
            personNote.PersonId = row.Field<int>( "PersonId" );
            personNote.NoteType = row.Field<string>( "NoteType" );
            personNote.IsPrivateNote = row.Field<bool>( "IsPrivateNote" );
            personNote.Text = row.Field<string>( "Text" );
            personNote.DateTime = row.Field<DateTime>( "DateTime" );

            return personNote;
        }
    }
}
