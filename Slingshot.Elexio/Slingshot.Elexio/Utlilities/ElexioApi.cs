using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;
using Slingshot.Core.Utilities;
using Slingshot.Elexio.Utilities.Translators;

namespace Slingshot.Elexio.Utilities
{
    /// <summary>
    /// API CCB Status
    /// </summary>
    public static class ElexioApi
    {
        private static SqlConnection _dbConnection;

        /// <summary>
        /// Gets or sets the counter.
        /// </summary>
        /// <value>
        /// The counter.
        /// </value>
        public static int Counter { get; set; } = 0;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public static string ApiHostname { get; set; }

        public static string ApiDatabase { get; set; }

        /// <summary>
        /// Gets or sets the API username.
        /// </summary>
        /// <value>
        /// The API username.
        /// </value>
        public static string ApiUsername { get; set; }

        /// <summary>
        /// Gets or sets the API password.
        /// </summary>
        /// <value>
        /// The API password.
        /// </value>
        public static string ApiPassword { get; set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public static string ConnectionString
        {
            get
            {
                //return $"Server={ApiHostname};Database={ApiDatabase};User Id={ApiUsername};Password ={ApiPassword};";
                return $"Server={ApiHostname};Database={ApiDatabase};Integrated Security=SSPI";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public static bool IsConnected { get; private set; } = false;

        #region SQL Queries

        private const string SQL_PEOPLE = @"
SELECT C.[ContactID]
	,A.AddressID
	,A.AddressName
	,FP.[Description] AS [FamilyPosition]
	,C.[FirstName]
	,C.[NickName]
	,C.[MiddleName]
	,C.[LastName]
	,C.[Suffix]
	,E.Email
	,C.[JobTitle]
	,C.[Company]
	,C.[Birthdate]
	,C.[Gender]
	,MS.[Description] AS [MaritalStatus]
	,[School]
	,[Race]
	,[Occupation]
	,[Education]
	,[BaptismDate]
	,[BaptizedHere]
	,LS.[Description] AS [Status]
	,C.[DateCreated]
	,C.[DateUpdated]
	,P.Phone
	,CP.Mobile
	,A.Street
	,A.City
	,A.[State]
	,A.ZipCode
	,A.Country
FROM [dbo].[tblContacts] C
INNER JOIN [dbo].[qryLookupFamilyPositions] FP ON FP.CodeId = C.FamilyPosition
INNER JOIN [dbo].[qryLookupStatus] LS ON LS.CodeID = C.[Status]
LEFT OUTER JOIN [dbo].[qryLookupMaritalStatus] MS ON MS.CodeID = C.MaritalStatus
LEFT OUTER JOIN [dbo].[qryEmailAddressTopOne] E ON E.ContactID = C.ContactID
LEFT OUTER JOIN [dbo].[qryPhoneTopOne] P ON P.ContactID = C.ContactID
LEFT OUTER JOIN [dbo].[qryPhoneMobileTopOne] CP ON CP.ContactID = C.ContactID
INNER JOIN [dbo].[tblAddresses] A ON A.AddressID = C.AddressID
ORDER BY C.AddressID, C.ContactID
";

        private const string SQL_FINANCIAL_ACCOUNTS = @"
SELECT [CodeID]
  ,[Description]
FROM [qryLookupFunds]
ORDER BY SortOrder
";

        #endregion

        /// <summary>
        /// Initializes the export.
        /// </summary>
        public static void InitializeExport()
        {
            ImportPackage.InitalizePackageFolder();
        }

        /// <summary>
        /// Connects the specified host name.
        /// </summary>
        /// <param name="apiHostname">Name of the host.</param>
        /// <param name="apiUsername">The API username.</param>
        /// <param name="apiPassword">The API password.</param>
        public static void Connect( string apiHostname, string apiDatabase, string apiUsername, string apiPassword )
        {
            ApiHostname = apiHostname;
            ApiDatabase = apiDatabase;
            ApiUsername = apiUsername;
            ApiPassword = apiPassword;

            _dbConnection = new SqlConnection { ConnectionString = ConnectionString };

            using ( SqlConnection con = new SqlConnection( ConnectionString ) )
            {
                try
                {
                    con.Open();

                    if ( con.State == ConnectionState.Open )
                    {
                        IsConnected = true;
                    }
                }
                catch ( Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            
            }
            
        }

        /// <summary>
        /// Gets the group types.
        /// </summary>
        /// <returns></returns>
        //public static List<GroupType> GetGroupTypes()
        //{
        //    List<GroupType> groupTypes = new List<GroupType>();

        //    try
        //    {
        //        var request = new RestRequest( API_GROUP_TYPES, Method.GET );
        //        var response = _client.Execute( request );

        //        XDocument xdocCustomFields = XDocument.Parse( response.Content );


        //        var sourceGroupTypes = xdocCustomFields.Element( "ccb_api" )?.Element( "response" )?.Element( "items" ).Elements( "item" );

        //        foreach ( var sourceGroupType in sourceGroupTypes )
        //        {
        //            var groupType = new GroupType();

        //            groupType.Id = sourceGroupType.Element( "id" ).Value.AsInteger();
        //            groupType.Name = sourceGroupType.Element( "name" )?.Value;

        //            groupTypes.Add( groupType );

        //        }
        //    }
        //    catch ( Exception ex )
        //    {
        //        ErrorMessage = ex.Message;
        //    }

        //    return groupTypes;
        //}

        /// <summary>
        /// Exports the groups.
        /// </summary>
        /// <param name="selectedGroupTypes">The selected group types.</param>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="perPage">The people per page.</param>
        //public static void ExportGroups( List<int> selectedGroupTypes, DateTime modifiedSince, int perPage = 500 )
        //{
        //    // write out the group types 
        //    WriteGroupTypes( selectedGroupTypes );

        //    // write departments
        //    ExportDeparments();

        //    // get groups
        //    try
        //    {
        //        int currentPage = 1;
        //        int loopCounter = 0;
        //        bool moreExist = true;
        //        while ( moreExist )
        //        {
        //            var request = new RestRequest( API_GROUPS, Method.GET );
        //            request.AddUrlSegment( "modifiedSince", modifiedSince.ToString( "yyyy-MM-dd" ) );
        //            request.AddUrlSegment( "currentPage", currentPage.ToString() );
        //            request.AddUrlSegment( "perPage", perPage.ToString() );

        //            var response = _client.Execute( request );

        //            XDocument xdoc = XDocument.Parse( response.Content );

        //            var groups = xdoc.Element( "ccb_api" )?.Element( "response" )?.Element( "groups" );

        //            if ( groups != null )
        //            {
        //                var returnCount = groups.Attribute( "count" )?.Value.AsIntegerOrNull();

        //                if ( returnCount.HasValue )
        //                {
        //                    foreach ( var groupNode in groups.Elements() )
        //                    {
        //                        // write out the group if its type was selected for export
        //                        var groupTypeId = groupNode.Element( "group_type" ).Attribute( "id" ).Value.AsInteger();
        //                        if ( selectedGroupTypes.Contains( groupTypeId ) )
        //                        {
        //                            var importGroups = CcbGroup.Translate( groupNode );

        //                            if ( importGroups != null )
        //                            {
        //                                foreach ( var group in importGroups )
        //                                {
        //                                    ImportPackage.WriteToPackage( group );
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if ( returnCount != perPage )
        //                    {
        //                        moreExist = false;
        //                    }
        //                    else
        //                    {
        //                        currentPage++;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                moreExist = false;
        //            }

        //            // developer safety blanket (prevents eating all the api calls for the day) 
        //            if ( loopCounter > loopThreshold )
        //            {
        //                break;
        //            }
        //            loopCounter++;
        //        }

        //    }
        //    catch ( Exception ex )
        //    {
        //        ErrorMessage = ex.Message;
        //    }

        //}

        /// <summary>
        /// Exports the deparments.
        /// </summary>
        //private static void ExportDeparments()
        //{
        //    try
        //    {
        //        var request = new RestRequest( API_DEPARTMENTS, Method.GET );
        //        var response = _client.Execute( request );

        //        XDocument xdocCustomFields = XDocument.Parse( response.Content );

        //        var sourceDepartments = xdocCustomFields.Element( "ccb_api" )?.Element( "response" )?.Elements( "items" );

        //        foreach ( var sourceDepartment in sourceDepartments.Elements( "item" ) )
        //        {
        //            var group = new Group();
        //            group.Id = ( "9999" + sourceDepartment.Element( "id" ).Value ).AsInteger();
        //            group.Name = sourceDepartment.Element( "name" )?.Value;
        //            group.Order = sourceDepartment.Element( "order" ).Value.AsInteger();
        //            group.GroupTypeId = 9999;

        //            ImportPackage.WriteToPackage( group );
        //        }
        //    }
        //    catch ( Exception ex )
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //}

        /// <summary>
        /// Exports the individuals.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="peoplePerPage">The people per page.</param>
        public static void ExportIndividuals( DateTime modifiedSince, int peoplePerPage = 500 )
        {
            // write out the person attributes
            WritePersonAttributes();

            // export people
            using ( var dtPeople = GetTableData( SQL_PEOPLE ) )
            {
                foreach ( DataRow row in dtPeople.Rows )
                {
                    var importPerson = ElexioPerson.Translate( row );

                    if ( importPerson != null )
                    {
                        ImportPackage.WriteToPackage( importPerson );
                    }
                }
            }
        }

        /// <summary>
        /// Exports the contributions.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        //public static void ExportContributions( DateTime modifiedSince )
        //{
        //    // we'll make an api call for each month until the modifiedSince date 
        //    var today = DateTime.Now;
        //    var numberOfMonths = ( ( ( today.Year - modifiedSince.Year ) * 12 ) + today.Month - modifiedSince.Month ) + 1;
        //    int loopCounter = 0;
        //    try
        //    {
        //        for ( int i = 0; i < numberOfMonths; i++ )
        //        {
        //            DateTime referenceDate = today.AddMonths( ( ( numberOfMonths - i ) - 1 ) * -1 );
        //            DateTime startDate = new DateTime( referenceDate.Year, referenceDate.Month, 1 );
        //            DateTime endDate = new DateTime( referenceDate.Year, referenceDate.Month, DateTime.DaysInMonth( referenceDate.Year, referenceDate.Month ) );

        //            // if it's the first instance set start date to the modifiedSince date
        //            if ( i == 0 )
        //            {
        //                startDate = modifiedSince;
        //            }

        //            // if it's the last time through set the end dat to today's date
        //            if ( i == numberOfMonths - 1 )
        //            {
        //                endDate = today;
        //            }

        //            var request = new RestRequest( API_FINANCIAL_BATCHES, Method.GET );
        //            request.AddUrlSegment( "startDate", startDate.ToString( "yyyy-MM-dd" ) );
        //            request.AddUrlSegment( "endDate", endDate.ToString( "yyyy-MM-dd" ) );

        //            var response = _client.Execute( request );

        //            XDocument xdoc = XDocument.Parse( response.Content );

        //            var sourceBatches = xdoc.Element( "ccb_api" )?.Element( "response" )?.Element( "batches" ).Elements( "batch" );

        //            foreach ( var sourceBatch in sourceBatches )
        //            {
        //                var importBatch = CcbFinancialBatch.Translate( sourceBatch );

        //                var sourceTransactions = sourceBatch.Element( "transactions" ).Elements( "transaction" );

        //                foreach ( var sourceTransaction in sourceTransactions )
        //                {
        //                    var importTransaction = CcbFinancialTransaction.Translate( sourceTransaction, sourceBatch.Attribute( "id" ).Value.AsInteger() );

        //                    if ( importTransaction != null )
        //                    {
        //                        importBatch.FinancialTransactions.Add( importTransaction );

        //                        var sourceTransactionDetails = sourceTransaction.Element( "transaction_details" ).Elements( "transaction_detail" );
        //                        foreach ( var sourceTransactionDetail in sourceTransactionDetails )
        //                        {
        //                            var importTransactionDetail = CcbFinancialTransactionDetail.Translate( sourceTransactionDetail, importTransaction.Id );

        //                            if ( importTransactionDetail != null )
        //                            {
        //                                importTransaction.FinancialTransactionDetails.Add( importTransactionDetail );
        //                            }
        //                        }
        //                    }
        //                }

        //                if ( importBatch != null )
        //                {
        //                    ImportPackage.WriteToPackage( importBatch );
        //                }
        //            }
        //        }
        //    }
        //    catch ( Exception ex )
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //}

        /// <summary>
        /// Exports the financial accounts.
        /// </summary>
        public static void ExportFinancialAccounts()
        {
            // export accounts
            using ( var dtAccounts = GetTableData( SQL_FINANCIAL_ACCOUNTS ) )
            {
                foreach ( DataRow row in dtAccounts.Rows )
                {
                    var importAccount = ElexioFinancialAccount.Translate( row );

                    if ( importAccount != null )
                    {
                        ImportPackage.WriteToPackage( importAccount );
                    }
                }
            }
        }

        /// <summary>
        /// Writes the person attributes.
        /// </summary>
        public static void WritePersonAttributes()
        {
            // export person attribute list
            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "School",
                Key = "IsBaptized",
                Category = "Membership",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Race",
                Key = "Race",
                Category = "Elexio",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Occupation",
                Key = "Occupation",
                Category = "Elexio",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Education",
                Key = "Education",
                Category = "Elexio",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Baptism Date",
                Key = "BaptismDate",
                Category = "Elexio",
                FieldType = "Rock.Field.Types.DateFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Baptized Here",
                Key = "BaptizedHere",
                Category = "Elexio",
                FieldType = "Rock.Field.Types.BooleanFieldType"
            } );
        }

        /// <summary>
        /// Exports the attendance.
        /// </summary>
        //public static void ExportAttendance( DateTime modifiedSince )
        //{
        //    // first we need to get the 'events' so we can get the group, location and schedule information
        //    // since the events have a different modification date than attendance we need to load all of the
        //    // events so we have the details we need for the attendance 
        //    var eventDetails = GetAttendanceEvents( new DateTime( 1900, 1, 1 ), 500 );

        //    // add location ids to the location fields (CCB doesn't have location ids) instead of randomly creating ids (that would not be consistant across exports)
        //    // we'll use a hash of the street name
        //    foreach ( var specificAddress in eventDetails.Select( e => new { e.LocationStreetAddress, e.LocationName } ).Distinct() )
        //    {
        //        int locationId = 1;

        //        if ( specificAddress.LocationName.IsNotNullOrWhitespace() || specificAddress.LocationStreetAddress.IsNotNullOrWhitespace() )
        //        {
        //            MD5 md5Hasher = MD5.Create();
        //            var hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( specificAddress.LocationName + specificAddress.LocationStreetAddress ) );
        //            locationId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
        //        }

        //        foreach ( var location in eventDetails.Where( e => e.LocationStreetAddress == specificAddress.LocationStreetAddress && e.LocationName == specificAddress.LocationName ) )
        //        {
        //            location.LocationId = locationId;
        //        }
        //    }

        //    // add schedule ids (CCB didn't have these either) instead of randomly creating ids (that would not be consistant across exports)
        //    // we'll use a hash of the schedule name
        //    foreach ( var specificSchedule in eventDetails.Select( e => e.ScheduleName ).Distinct() )
        //    {
        //        int scheduleId = 1;

        //        if ( specificSchedule.IsNotNullOrWhitespace() )
        //        {
        //            MD5 md5Hasher = MD5.Create();
        //            var hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( specificSchedule ) );
        //            scheduleId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
        //        }

        //        foreach ( var location in eventDetails.Where( e => e.ScheduleName == specificSchedule ) )
        //        {
        //            location.ScheduleId = scheduleId;
        //        }
        //    }

        //    // export locations, same thing with location ids we'll hash the street address
        //    foreach ( var location in eventDetails.Select( e => new { e.LocationName, e.LocationStreetAddress, e.LocationCity, e.LocationState, e.LocationZip, e.LocationId } ).Distinct() )
        //    {
        //        ImportPackage.WriteToPackage( new Location()
        //        {
        //            Id = location.LocationId,
        //            Name = location.LocationName,
        //            Street1 = location.LocationStreetAddress,
        //            City = location.LocationCity,
        //            State = location.LocationState,
        //            PostalCode = location.LocationZip
        //        } );
        //    }

        //    // export schedules
        //    foreach ( var schedule in eventDetails.Select( s => new { s.ScheduleId, s.ScheduleName } ).Distinct() )
        //    {
        //        ImportPackage.WriteToPackage( new Schedule()
        //        {
        //            Id = schedule.ScheduleId,
        //            Name = schedule.ScheduleName
        //        } );
        //    }

        //    // ok now that we have our events we can actually get the attendance data
        //    GetAttendance( modifiedSince, eventDetails );
        //}


        //private static void GetAttendance( DateTime modifiedSince, List<EventDetail> eventDetails )
        //{
        //    // we'll make an api call for each month until the modifiedSince date 
        //    var today = DateTime.Now;
        //    var numberOfMonths = ( ( ( today.Year - modifiedSince.Year ) * 12 ) + today.Month - modifiedSince.Month ) + 1;
        //    int loopCounter = 0;

        //    try
        //    {
        //        for ( int i = 0; i < numberOfMonths; i++ )
        //        {
        //            DateTime referenceDate = today.AddMonths( ( ( numberOfMonths - i ) - 1 ) * -1 );
        //            DateTime startDate = new DateTime( referenceDate.Year, referenceDate.Month, 1 );
        //            DateTime endDate = new DateTime( referenceDate.Year, referenceDate.Month, DateTime.DaysInMonth( referenceDate.Year, referenceDate.Month ) );

        //            // if it's the first instance set start date to the modifiedSince date
        //            if ( i == 0 )
        //            {
        //                startDate = modifiedSince;
        //            }

        //            // if it's the last time through set the end dat to today's date
        //            if ( i == numberOfMonths - 1 )
        //            {
        //                endDate = today;
        //            }

        //            var request = new RestRequest( API_ATTENDANCE, Method.GET );
        //            request.AddUrlSegment( "startDate", startDate.ToString( "yyyy-MM-dd" ) );
        //            request.AddUrlSegment( "endDate", endDate.ToString( "yyyy-MM-dd" ) );

        //            var response = _client.Execute( request );

        //            XDocument xdoc = XDocument.Parse( response.Content );

        //            var sourceEvents = xdoc.Element( "ccb_api" )?.Element( "response" )?.Element( "events" ).Elements( "event" );

        //            if ( sourceEvents != null )
        //            {
        //                foreach ( var sourceEvent in sourceEvents )
        //                {
        //                    var attendances = CcbAttendance.Translate( sourceEvent, eventDetails );

        //                    if ( attendances != null )
        //                    {
        //                        foreach ( var attendance in attendances )
        //                        {
        //                            ImportPackage.WriteToPackage( attendance );
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch ( Exception ex )
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //}

        /// <summary>
        /// Gets the attendance events.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns></returns>
        //private static List<EventDetail> GetAttendanceEvents( DateTime modifiedSince, int itemsPerPage )
        //{
        //    List<EventDetail> eventDetails = new List<EventDetail>();

        //    int currentPage = 1;
        //    int loopCounter = 0;
        //    bool moreItemsExist = true;

        //    try
        //    {
        //        while ( moreItemsExist )
        //        {
        //            var request = new RestRequest( API_EVENTS, Method.GET );
        //            request.AddUrlSegment( "modifiedSince", modifiedSince.ToString( "yyyy-MM-dd" ) );
        //            request.AddUrlSegment( "currentPage", currentPage.ToString() );
        //            request.AddUrlSegment( "itemsPerPage", itemsPerPage.ToString() );

        //            var response = _client.Execute( request );

        //            if ( response.StatusCode == System.Net.HttpStatusCode.OK )
        //            {
        //                XDocument xdoc = XDocument.Parse( response.Content );

        //                var returnCount = xdoc.Element( "ccb_api" )?.Element( "response" )?.Element( "events" )?.Attribute( "count" )?.Value.AsIntegerOrNull();

        //                var events = xdoc.Element( "ccb_api" )?.Element( "response" )?.Element( "events" )?.Elements( "event" );

        //                foreach ( var eventItem in events )
        //                {
        //                    var eventDetail = new EventDetail();
        //                    eventDetails.Add( eventDetail );

        //                    eventDetail.EventId = eventItem.Attribute( "id" ).Value.AsInteger();
        //                    eventDetail.GroupId = eventItem.Element( "group" ).Attribute( "id" ).Value.AsInteger();
        //                    eventDetail.ScheduleName = eventItem.Element( "recurrence_description" ).Value;

        //                    if ( eventItem.Element( "location" ) != null && eventItem.Element( "location" ).HasElements )
        //                    {
        //                        eventDetail.LocationName = eventItem.Element( "location" ).Element( "name" ).Value;
        //                        eventDetail.LocationStreetAddress = eventItem.Element( "location" ).Element( "street_address" ).Value;
        //                        eventDetail.LocationCity = eventItem.Element( "location" ).Element( "city" ).Value;
        //                        eventDetail.LocationState = eventItem.Element( "location" ).Element( "state" ).Value;
        //                        eventDetail.LocationZip = eventItem.Element( "location" ).Element( "zip" ).Value;
        //                    }
        //                }

        //                if ( returnCount != itemsPerPage )
        //                {
        //                    moreItemsExist = false;
        //                }
        //                else
        //                {
        //                    currentPage++;
        //                }
        //            }

        //            // developer safety blanket (prevents eating all the api calls for the day) 
        //            if ( loopCounter > loopThreshold )
        //            {
        //                break;
        //            }
        //            loopCounter++;
        //        }
        //    }
        //    catch ( Exception ex )
        //    {
        //        ErrorMessage = ex.Message;
        //    }


        //    return eventDetails;
        //}

        /// <summary>
        /// Writes the group types.
        /// </summary>
        /// <param name="selectedGroupTypes">The selected group types.</param>
        //public static void WriteGroupTypes( List<int> selectedGroupTypes )
        //{
        //    // hardcode the department and director group types as these are baked into the box
        //    ImportPackage.WriteToPackage( new GroupType()
        //    {
        //        Id = 9999,
        //        Name = "Department"
        //    } );

        //    ImportPackage.WriteToPackage( new GroupType()
        //    {
        //        Id = 9998,
        //        Name = "Director"
        //    } );

        //    // add custom defined group types 
        //    var groupTypes = GetGroupTypes();
        //    foreach ( var groupType in groupTypes.Where( t => selectedGroupTypes.Contains( t.Id ) ) )
        //    {
        //        ImportPackage.WriteToPackage( new GroupType()
        //        {
        //            Id = groupType.Id,
        //            Name = groupType.Name
        //        } );
        //    }
        //}

        /// <summary>
        /// Gets the table data.
        /// </summary>
        /// <param name="command">The SQL command to run.</param>
        /// <returns></returns>
        public static DataTable GetTableData( string command )
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            SqlCommand dbCommand = new SqlCommand( command, _dbConnection );
            SqlDataAdapter adapter = new SqlDataAdapter();

            adapter.SelectCommand = dbCommand;
            adapter.Fill( dataSet );
            dataTable = dataSet.Tables["Table"];

            return dataTable;
        }


        /// <summary>
        /// Temporary class for assembling attendance information
        /// </summary>
        public class EventDetail
        {
            /// <summary>
            /// Gets or sets the event identifier.
            /// </summary>
            /// <value>
            /// The event identifier.
            /// </value>
            public int EventId { get; set; }

            /// <summary>
            /// Gets or sets the schedule identifier.
            /// </summary>
            /// <value>
            /// The schedule identifier.
            /// </value>
            public int ScheduleId { get; set; }

            /// <summary>
            /// Gets or sets the name of the schedule.
            /// </summary>
            /// <value>
            /// The name of the schedule.
            /// </value>
            public string ScheduleName { get; set; }

            /// <summary>
            /// Gets or sets the group identifier.
            /// </summary>
            /// <value>
            /// The group identifier.
            /// </value>
            public int GroupId { get; set; }

            /// <summary>
            /// Gets or sets the location identifier.
            /// </summary>
            /// <value>
            /// The location identifier.
            /// </value>
            public int LocationId { get; set; }

            /// <summary>
            /// Gets or sets the name of the location.
            /// </summary>
            /// <value>
            /// The name of the location.
            /// </value>
            public string LocationName { get; set; }

            /// <summary>
            /// Gets or sets the location street address.
            /// </summary>
            /// <value>
            /// The location street address.
            /// </value>
            public string LocationStreetAddress { get; set; }

            /// <summary>
            /// Gets or sets the location city.
            /// </summary>
            /// <value>
            /// The location city.
            /// </value>
            public string LocationCity { get; set; }

            /// <summary>
            /// Gets or sets the state of the location.
            /// </summary>
            /// <value>
            /// The state of the location.
            /// </value>
            public string LocationState { get; set; }

            /// <summary>
            /// Gets or sets the location zip.
            /// </summary>
            /// <value>
            /// The location zip.
            /// </value>
            public string LocationZip { get; set; }
        }
    }
}
