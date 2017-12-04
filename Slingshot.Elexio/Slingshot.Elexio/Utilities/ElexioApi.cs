﻿using System;
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
    /// API Elexio Status
    /// </summary>
    public static class ElexioApi
    {
        private static SqlConnection _dbConnection;
        private static DateTime _modifiedSince;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public static string HostName { get; set; }

        public static string Database { get; set; }

        /// <summary>
        /// Gets or sets the API username.
        /// </summary>
        /// <value>
        /// The API username.
        /// </value>
        public static string Username { get; set; }

        /// <summary>
        /// Gets or sets the API password.
        /// </summary>
        /// <value>
        /// The API password.
        /// </value>
        public static string Password { get; set; }

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
                if ( IsSQLAuthentication )
                {
                    return $"Server={HostName};Database={Database};User Id={Username};Password={Password};";
                }
                else
                {
                    return $"Server={HostName};Database={Database};Integrated Security=SSPI";
                }
                
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public static bool IsConnected { get; private set; } = false;

        public static bool IsSQLAuthentication { get; set; } = false;

        #region SQL Queries

        private static string SQL_PEOPLE = $@"
SELECT C.[ContactID] AS [Id]
	,A.AddressID AS [FamilyId]
	,HOH.LastName + ' Family' AS [FamilyName]
	,FP.[Description] AS [FamilyRole]
	,LEFT(C.[FirstName], 50) AS [FirstName]
	,LEFT(C.[NickName], 50) AS [NickName]
	,C.[MiddleName]
	,LEFT(C.[LastName], 50) AS [LastName]
	,C.[Suffix]
	,E.Email
	,C.[Gender]
	,MS.[Description] AS [MaritalStatus]
	,C.[Birthdate]
	,A.[Anniversary] AS [AnniversaryDate]
	-- Record Status
	-- Inactive Reason
	,LS.[Description] AS [ConnectionStatus]
	-- Email Preference
	,C.[DateCreated] AS [CreatedDateTime]
	,C.[DateUpdated] AS [ModifiedDateTime]
	,HOH.Campus
	,HOH.CampusId
	-- Grade
	-- Give Individually
	
	-- Phones
	,P.Phone AS [HomePhone]
	,CP.Mobile AS [MobilePhone]
	
	-- Address
	,A.Street
	,A.City
	,A.[State]
	,A.ZipCode
	,A.Country
	
	-- Other Fields
	,C.[JobTitle]
	,C.[Company]
	,[School]
	,LR.[Description] AS [Race]
	,LO.[Description] AS [Occupation]
	,LE.[Description] AS [Education]
	,[BaptismDate]
    ,CASE WHEN BaptizedHere = 1 THEN 'True' ELSE 'False' END AS [BaptizedHere]
FROM [dbo].[tblContacts] C
INNER JOIN [dbo].[qryLookupFamilyPositions] FP ON FP.CodeId = C.FamilyPosition
INNER JOIN [dbo].[qryLookupStatus] LS ON LS.CodeID = C.[Status]
LEFT OUTER JOIN [dbo].[qryLookupMaritalStatus] MS ON MS.CodeID = C.MaritalStatus
LEFT OUTER JOIN [dbo].[qryEmailAddressTopOne] E ON E.ContactID = C.ContactID
LEFT OUTER JOIN [dbo].[qryPhoneTopOne] P ON P.ContactID = C.ContactID
LEFT OUTER JOIN [dbo].[qryPhoneMobileTopOne] CP ON CP.ContactID = C.ContactID
LEFT OUTER JOIN [dbo].[qryLookupRace] LR ON LR.CodeID = C.Race
LEFT OUTER JOIN [dbo].[qryLookupOccupations] LO ON LO.CodeID = C.Occupation
LEFT OUTER JOIN [dbo].[qryLookupEducation] LE ON LE.CodeID = C.Education
INNER JOIN [dbo].[tblAddresses] A ON A.AddressID = C.AddressID
OUTER APPLY (
	SELECT TOP 1 CC.ContactID
		,CC.LastName
		,LC.[Description] AS [Campus]
		,LC.CodeID AS [CampusId]
	FROM tblContacts CC
	LEFT OUTER JOIN qryLookupFamilyPositions LFP ON LFP.CodeID = CC.FamilyPosition
	LEFT OUTER JOIN qryLookupCampus LC ON LC.CodeID = CC.[Service]
	WHERE CC.AddressID = C.AddressID 
	ORDER BY LFP.CodeValue
) HOH
WHERE C.[DateUpdated] >= { _modifiedSince.ToShortDateString() }
ORDER BY A.AddressID, C.ContactID
";

        private static string SQL_PERSON_NOTES = $@"
SELECT
	 CN.[ContactNotesID] AS [Id]
	,CN.[ContactID] AS [PersonId]
	,NT.[Description] AS [NoteType]
	,'' AS [Caption]
	,[Private] AS [IsPrivateNote]
	,[Notes] AS [Text]
	,[NoteDate] AS [DateTime]
FROM [dbo].[tblContactNotes] CN
INNER JOIN [dbo].[qryLookupNoteTypes] NT ON NT.CodeID = CN.NoteType
WHERE [Notes] IS NOT NULL
    AND CN.[DateUpdated] >= { _modifiedSince.ToShortDateString() }
";

        private const string SQL_FINANCIAL_ACCOUNTS = @"
SELECT [CodeID] AS [Id]
  ,[Description] AS [Name]
FROM [qryLookupFunds]
ORDER BY SortOrder
";

        private const string SQL_FINANCIAL_BATCHES = @"
SELECT 
	[BatchID] AS [Id], 
	[Batch] AS [Name]
FROM tblContributions
WHERE (BatchID IS NOT NULL AND BatchID != 0) AND
	  (Batch IS NOT NULL AND Batch != '')
GROUP BY BatchID, Batch
";

        private static string SQL_FINANCIAL_TRANSACTIONS = $@"
SELECT 
	 C.ContributionID AS [Id]
	,C.BatchID
	,CASE WHEN ContactID IS NULL THEN (SELECT TOP 1 CC.ContactID 
										FROM tblContacts CC
										LEFT OUTER JOIN qryLookupFamilyPositions LFP ON LFP.CodeID = CC.FamilyPosition
										WHERE CC.AddressID = C.AddressID 
										ORDER BY LFP.CodeValue) 
		  ELSE ContactID 
	 END AS [AuthorizedPersonId]  -- if transaction is tied to an address, choose the head of household as the giver
	,C.DateGiven AS [TransactionDate]
	--,'' AS [TransactionType] 
	--,'' AS TransactionSource
	,CC.[Description] AS [CurrencyType]
	,C.Comment AS [Summary]
	--,'' AS TransactionCode
	,C.DateCreated AS [CreatedDateTime]
	,C.DateUpdated AS [ModifiedDateTime]
FROM tblContributions C
INNER JOIN tblCodes CC ON CC.CodeID = C.GivingMethod
WHERE C.DateUpdated >= { _modifiedSince.ToShortDateString() }
";

        private static string SQL_FINANCIAL_TRANSACTION_DETAILS = $@"
SELECT 
	-- Id
	ContributionID AS [TransactionId]
	,DesignatedFund AS [AccountId]
	,dbo.ConvertAmount(Amount, AddressID, ContactID) AS [Amount]
	,DateCreated AS [CreatedDateTime]
	,DateUpdated AS [ModifiedDateTime]
FROM [dbo].[tblContributions]
WHERE DateUpdated >= { _modifiedSince.ToShortDateString() }
";

        private const string SQL_GROUP_TYPES = @"
SELECT MinistryID AS [Id]
	,[Name]
FROM [dbo].[tblMinistries] M
WHERE MinistryType = 0
ORDER BY MinistryID
";

        private const string SQL_GROUPS = @"
SELECT 
	 [MinistryID] AS [Id] -- Might be an issue with duplicate group ids
	,[Name]
	,[MinistryType] AS [GroupTypeId]
FROM tblMinistries M
WHERE MinistryType != 0
ORDER BY MinistryID
";

        private const string SQL_GROUP_MEMBERS = @"
SELECT DISTINCT
	 I.[ContactID] AS [PersonId]
	,I.[Activity] AS [GroupId]
	,CASE WHEN L.[Description] IS NULL THEN 'Unknown' ELSE L.[Description] END AS [Role]
FROM [c2737595_data].[dbo].[tblInvolvement] I
INNER JOIN tblMinistries M ON M.MinistryID = I.Activity AND M.MinistryType = I.ActivityType
INNER JOIN tblContacts C ON C.ContactID = I.ContactID
LEFT OUTER JOIN [dbo].[qryLookup] L ON L.CodeID = I.CommitmentLevel
ORDER BY I.Activity, I.ContactID, CASE WHEN L.[Description] IS NULL THEN 'Unknown' ELSE L.[Description] END
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
        /// <param name="hostname">Name of the host.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public static void Connect( string hostname, string database, string username, string password, bool isSQLAuth )
        {
            HostName = hostname;
            Database = database;
            Username = username;
            Password = password;
            IsSQLAuthentication = isSQLAuth;

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
        public static List<GroupType> GetGroupTypes()
        {
            List<GroupType> groupTypes = new List<GroupType>();

            using ( var dtGroupTypes = GetTableData( SQL_GROUP_TYPES ) )
            {
                foreach ( DataRow row in dtGroupTypes.Rows )
                {
                    var groupType = new GroupType();

                    groupType.Id = row.Field<int>( "Id" );
                    groupType.Name = row.Field<string>( "Name" );

                    groupTypes.Add( groupType );
                }

                return groupTypes;
            }
        }

        /// <summary>
        /// Exports the groups.
        /// </summary>
        /// <param name="selectedGroupTypes">The selected group types.</param>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="perPage">The people per page.</param>
        public static void ExportGroups( List<int> selectedGroupTypes, DateTime modifiedSince, int perPage = 500 )
        {
            // write out the group types 
            WriteGroupTypes( selectedGroupTypes );

            // export groups
            using ( var dtGroups = GetTableData( SQL_GROUPS ) )
            {
                foreach ( DataRow row in dtGroups.Rows )
                {
                    var importGroup = ElexioGroup.Translate( row );

                    if ( importGroup != null )
                    {
                        ImportPackage.WriteToPackage( importGroup );
                    }
                }
            }

            // export group members
            using ( var dtGroupMembers = GetTableData( SQL_GROUP_MEMBERS ) )
            {
                foreach ( DataRow row in dtGroupMembers.Rows )
                {
                    var importGroupMember = ElexioGroupMember.Translate( row );

                    if ( importGroupMember != null )
                    {
                        ImportPackage.WriteToPackage( importGroupMember );
                    }
                }
            }

        }

        /// <summary>
        /// Exports the individuals.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="peoplePerPage">The people per page.</param>
        public static void ExportIndividuals( DateTime modifiedSince, int peoplePerPage = 500 )
        {
            _modifiedSince = modifiedSince;

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

            // export person notes
            using ( var dtPersonNotes = GetTableData( SQL_PERSON_NOTES ) )
            {
                foreach ( DataRow row in dtPersonNotes.Rows )
                {
                    var importPersonNote = ElexioPersonNote.Translate( row );

                    if ( importPersonNote != null )
                    {
                        ImportPackage.WriteToPackage( importPersonNote );
                    }
                }
            }
        }


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
        /// Exports the contributions.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        public static void ExportContributions( DateTime modifiedSince )
        {
            _modifiedSince = modifiedSince;

            // hardcode a generic financial batch for all transactions that do not have a batch
            ImportPackage.WriteToPackage( new FinancialBatch()
            {
                Id = 9999,
                Name = "Imported Elexio Transactions Without Batch",
                Status = BatchStatus.Closed,
                StartDate = DateTime.Now
            } );


            // export financial batches
            using ( var dtBatches = GetTableData( SQL_FINANCIAL_BATCHES ) )
            {
                foreach ( DataRow row in dtBatches.Rows )
                {
                    var importBatch = ElexioFinancialBatch.Translate( row );

                    if ( importBatch != null )
                    {
                        ImportPackage.WriteToPackage( importBatch );
                    }
                }
            }

            // export financial transactions
            using ( var dtTransactions = GetTableData( SQL_FINANCIAL_TRANSACTIONS ) )
            {
                foreach ( DataRow row in dtTransactions.Rows )
                {
                    var importTransactions = ElexioFinancialTransaction.Translate( row );

                    if ( importTransactions != null )
                    {
                        ImportPackage.WriteToPackage( importTransactions );
                    }
                }
            }

            // export financial transaction details
            using ( var dtTransactionDetails = GetTableData( SQL_FINANCIAL_TRANSACTION_DETAILS ) )
            {
                foreach ( DataRow row in dtTransactionDetails.Rows )
                {
                    var importTransactionDetails = ElexioFinancialTransactionDetail.Translate( row );

                    if ( importTransactionDetails != null )
                    {
                        ImportPackage.WriteToPackage( importTransactionDetails );
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
        //}


        //private static void GetAttendance( DateTime modifiedSince, List<EventDetail> eventDetails )
        //{
        //}

        /// <summary>
        /// Gets the attendance events.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns></returns>
        //private static List<EventDetail> GetAttendanceEvents( DateTime modifiedSince, int itemsPerPage )
        //{
        //}

        /// <summary>
        /// Writes the group types.
        /// </summary>
        /// <param name="selectedGroupTypes">The selected group types.</param>
        public static void WriteGroupTypes( List<int> selectedGroupTypes )
        {
            // add custom defined group types 
            var groupTypes = GetGroupTypes();
            foreach ( var groupType in groupTypes.Where( t => selectedGroupTypes.Contains( t.Id ) ) )
            {
                ImportPackage.WriteToPackage( new GroupType()
                {
                    Id = groupType.Id,
                    Name = groupType.Name
                } );
            }
        }

        /// <summary>
        /// Gets the table data.
        /// </summary>
        /// <param name="command">The SQL command to run.</param>
        /// <returns></returns>
        public static DataTable GetTableData( string command )
        {
            try
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
            catch ( Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return null;
        }
    }
}
