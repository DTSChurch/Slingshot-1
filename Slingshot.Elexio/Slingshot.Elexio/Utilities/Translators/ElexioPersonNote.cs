using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slingshot.Core;
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

            // Currently all private notes in Elexio will be public since private notes in
            //  Rock require a created by person and that field is not stored as a person id.
            personNote.IsPrivateNote = false;

            personNote.Text = row.Field<string>( "Text" );
            personNote.DateTime = row.Field<DateTime>( "DateTime" );

            return personNote;
        }
    }
}
