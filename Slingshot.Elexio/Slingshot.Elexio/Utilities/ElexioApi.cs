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
    /// API Elexio Status
    /// </summary>
    public static class ElexioApi
    {
        private static SqlConnection _dbConnection;
        private static DateTime _modifiedSince;

        #region Properties

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
        /// The hostname.
        /// </value>
        public static string HostName { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance is using SQL authentication.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is using SQL authentication; otherwise, <c>false</c>.
        /// </value>
        public static bool IsSQLAuthentication { get; set; } = false;

        #endregion

        #region SQL Queries

        private static string SQL_PEOPLE = $@"
SELECT
	C.[ContactID] AS [Id]
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
	,LS.[Description] AS [ConnectionStatus]
	,CASE WHEN C.EmailOptOutDate IS NOT NULL THEN 'False' ELSE 'True' END AS [EmailOptOut]
	,C.[DateCreated] AS [CreatedDateTime]
	,C.[DateUpdated] AS [ModifiedDateTime]
	,HOH.Campus
	,HOH.CampusId
	,AG.[Description] AS [AgeGroup] -- Grade
	,C.TrackContributionsIndividually AS [GiveIndividually]
	,C.MembershipDate AS [MembershipDate]
	
	-- Phones
	,HP.[Value] AS [HomePhone]
	,CASE WHEN HP.Private = 1 THEN 'True' ELSE 'False' END AS [HomePhoneUnlisted]
	,CP.[Value] AS [MobilePhone]
	,CASE WHEN CP.Private = 1 THEN 'True' ELSE 'False' END AS [MobilePhoneUnlisted]
	,WP.[Value] AS [WorkPhone]
	,CASE WHEN WP.Private = 1 THEN 'True' ELSE 'False' END AS [WorkPhoneUnlisted]
	,CASE WHEN C.SMSOptOutDate IS NOT NULL THEN 'False' ELSE 'True' END AS [IsMessagingEnabled]
	
	---- Attributes
	,ALLERGY.Allergy
	,MEDICAL.Medical

	-- Social Media attributes
	,FB.[Value] AS [Facebook]
	,IG.[Value] AS [Instagram]
	,TW.[Value] AS [Twitter]
	,LI.[Value] AS [LinkedIn]

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

    -- Background Checks
    ,BC.[BackgroundCheckDate]
    ,BC.[BackgroundCheckResult]
FROM [dbo].[tblContacts] C
INNER JOIN [dbo].[qryLookupFamilyPositions] FP ON FP.CodeId = C.FamilyPosition
INNER JOIN [dbo].[qryLookupStatus] LS ON LS.CodeID = C.[Status]
LEFT OUTER JOIN [dbo].[qryLookupMaritalStatus] MS ON MS.CodeID = C.MaritalStatus
LEFT OUTER JOIN [dbo].[qryLookupRace] LR ON LR.CodeID = C.Race
LEFT OUTER JOIN [dbo].[qryLookupOccupations] LO ON LO.CodeID = C.Occupation
LEFT OUTER JOIN [dbo].[qryLookupEducation] LE ON LE.CodeID = C.Education
LEFT OUTER JOIN [dbo].[qryAgeGroups] AG ON AG.CodeID = C.AgeGroup
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
) HOH -- Head of Household
OUTER APPLY (
	SELECT TOP 1CC.[Value] AS [Email]
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeId = CC.ValueType
	WHERE CC.ContactID = C.ContactID
		AND CCC.CodeValue = 'email'
) E
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE 'Home'
		AND CC.ContactID = C.ContactId
) HP -- Home Phone
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE 'Mobile'
		AND CC.ContactID = C.ContactId
) CP -- Mobile Phone
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE 'Work'
		AND CC.ContactID = C.ContactId
) WP -- Work Phone
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE 'Facebook'
		AND CC.ContactID = C.ContactId
) FB -- Facebook
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE '%Instagram%'
		AND CC.ContactID = C.ContactId
) IG -- Instagram
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE 'Twitter'
		AND CC.ContactID = C.ContactId
) TW -- Twitter
OUTER APPLY (
	SELECT TOP 1 CC.*
	FROM tblContactCommunications CC
	INNER JOIN tblCodes CCC ON CCC.CodeID = CC.ValueType
	WHERE CCC.[Description] LIKE 'Linked-In'
		AND CC.ContactID = C.ContactId
) LI -- Linked-In
OUTER APPLY (
	SELECT TOP 1  S.StatusAsOf
	FROM [tblStatus] S
	INNER JOIN tblCodes CCC ON CCC.CodeID = S.Status
	INNER JOIN tblContacts CC ON CC.ContactID = S.ContactID
	WHERE CCC.[Description] LIKE 'Regular Attender'
		AND CC.ContactID = C.ContactId
	ORDER BY S.ContactID, S.StatusAsOf
) RA
OUTER APPLY (
	SELECT TOP 1
		BGC.StageDate AS [BackgroundCheckDate]
		,CASE 
			WHEN BGC.StageStatus = 593 THEN 'Pass' 
			WHEN BGC.StageStatus = 594 THEN 'Fail' 
		 END AS [BackgroundCheckResult]
	FROM tblBackgroundChecks BGC
	WHERE BGC.ContactID = C.ContactID
	ORDER BY BGC.StageDate DESC
) BC
OUTER APPLY (
	SELECT TOP 1 CN.[Notes] AS [Allergy]
	FROM tblContactNotes CN
	WHERE CN.NoteType = 583 -- Allergy Notes
		AND CN.ContactID = C.ContactID
) ALLERGY
OUTER APPLY (
	SELECT TOP 1 CN.[Notes] AS [Medical]
	FROM tblContactNotes CN
	WHERE CN.NoteType = 585 -- Medical Notes
		AND CN.ContactID = C.ContactID
) MEDICAL
WHERE C.[DateUpdated] >= { _modifiedSince.ToShortDateString() }
    AND LS.[Description] != 'Inactivated by Mass Update' -- TODO: Remove since this is specific to Grace Church
--ORDER BY A.AddressID, C.ContactID
";

        private static string SQL_PEOPLE_PHOTOS_COUNT = $@"
SELECT
    COUNT(*) AS [Count]
FROM [dbo].[tblContacts] C
INNER JOIN tblPhotos P ON 
	C.AddressID IS NULL OR P.AddressID = C.AddressID AND
	C.ContactID IS NULL OR P.ContactID = C.ContactID
INNER JOIN tblImages I ON I.UniqueID = P.[FileName]
WHERE C.[DateUpdated] >= { _modifiedSince.ToShortDateString() }
";

        private static string SQL_PEOPLE_PHOTOS = $@"
WITH CTEImages AS (
	SELECT
		 C.ContactID AS [Id]
		,I.ImageBinary AS [ImageData]
		,ROW_NUMBER() OVER (ORDER BY C.ContactID) AS RowNumber
	FROM [dbo].[tblContacts] C
	INNER JOIN tblPhotos P ON 
		C.AddressID IS NULL OR P.AddressID = C.AddressID AND
		C.ContactID IS NULL OR P.ContactID = C.ContactID
	INNER JOIN tblImages I ON I.UniqueID = P.[FileName]
	WHERE C.[DateUpdated] >= { _modifiedSince.ToShortDateString() }
)

SELECT 
	 [Id]
	,[ImageData]
FROM CTEImages
WHERE RowNumber BETWEEN {{0}} AND {{1}}
";

        private static string SQL_PEOPLE_NOTES = $@"
SELECT
	 CN.[ContactNotesID] AS [Id]
	,CN.[ContactID] AS [PersonId]
	,NT.[Description] AS [NoteType]
	,'' AS [Caption]
	,CASE WHEN [Private] = 1 THEN 'True' ELSE 'False' END AS [IsPrivateNote]
	,[Notes] AS [Text]
	,[NoteDate] AS [DateTime]
FROM [dbo].[tblContactNotes] CN
INNER JOIN [dbo].[qryLookupNoteTypes] NT ON NT.CodeID = CN.NoteType
WHERE [Notes] IS NOT NULL
    AND CN.[DateUpdated] >= { _modifiedSince.ToShortDateString() }

UNION

SELECT
	S.StatusID + 999000000 AS [Id] -- Offset the status id to prevent id collisions with notes
	,S.ContactID AS [PersonId]
	,'Status History' AS [NoteType]
	,'' AS [Caption]
	,'False' AS [IsPrivateNote]
	,LS.[Description] AS [Text]
	,S.StatusAsOf AS [Date]
FROM tblStatus S
INNER JOIN qryLookupStatus LS ON LS.CodeID = S.[Status]
WHERE S.StatusAsOf >= { _modifiedSince.ToShortDateString() }
";

        private const string SQL_FINANCIAL_ACCOUNTS = @"
SELECT LF.[CodeID] AS [Id]
  ,LF.[Description] AS [Name]
  ,CASE WHEN C.[RangeStart] = 1 THEN 'True' ELSE 'False' END AS [IsTaxDeductible]
FROM [qryLookupFunds] LF
INNER JOIN tblCodes C ON C.CodeID = LF.CodeID
ORDER BY LF.SortOrder
";

        private const string SQL_FINANCIAL_PLEDGES = @"
SELECT
	 P.PledgeID AS [Id]
	,CASE WHEN ContactID IS NULL THEN (SELECT TOP 1 CC.ContactID 
										FROM tblContacts CC
										LEFT OUTER JOIN qryLookupFamilyPositions LFP ON LFP.CodeID = CC.FamilyPosition
										WHERE CC.AddressID = P.AddressID 
										ORDER BY LFP.CodeValue) 
		ELSE ContactID 
	 END AS [PersonId]
	,P.[Fund] AS [AccountId]
	,P.StartDate
	,P.EndDate
	,P.Interval AS [PledgeFrequency]
	,P.Amount AS [TotalAmount]
	,P.DateCreated AS [CreatedDateTime]
	,P.DateUpdated AS [ModifiedDateTime]
FROM tblPledges P
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
	,CC.[Description] AS [GivingMethod]
	,C.Comment AS [Summary]
	,C.DateCreated AS [CreatedDateTime]
	,C.DateUpdated AS [ModifiedDateTime]
	,F.CodeID AS [AccountId]
    ,dbo.ConvertAmount(C.Amount, C.AddressID, C.ContactID) AS [Amount]
FROM tblContributions C
INNER JOIN tblCodes CC ON CC.CodeID = C.GivingMethod
INNER JOIN [qryLookupFunds] F ON F.CodeID = C.DesignatedFund
WHERE C.DateUpdated >= { _modifiedSince.ToShortDateString() }
";

        private static string SQL_FINANCIAL_TRANSACTIONS_IMAGES_COUNT = $@"
SELECT 
	COUNT(*) AS [Count]
FROM tblContributions C
INNER JOIN tblImages I ON I.UniqueID = C.ImageID
WHERE C.DateUpdated >= { _modifiedSince.ToShortDateString() }
";

        private static string SQL_FINANCIAL_TRANSACTIONS_IMAGES = $@"
WITH CTEImages AS (
	SELECT 
		C.ContributionID AS [Id]
		,I.ImageBinary AS [ImageData]
		,ROW_NUMBER() OVER (ORDER BY ContributionID) AS RowNumber
	FROM tblContributions C
	INNER JOIN tblImages I ON I.UniqueID = C.ImageID
    WHERE C.DateUpdated >= { _modifiedSince.ToShortDateString() }
)

SELECT 
	 [Id]
	,[ImageData]
FROM CTEImages
WHERE RowNumber BETWEEN {{0}} AND {{1}}
";

        private const string SQL_GROUP_TYPES = @"
SELECT 1 AS [Id], 'Small Groups' AS [Name]
UNION
SELECT 3 AS [Id], 'Ministry Teams' AS [Name]
UNION
SELECT 4 AS [Id], 'Classes/Seminars' AS [Name]
UNION
SELECT 5 AS [Id], 'Events' AS [Name]
UNION
SELECT 24 AS [Id], 'Services' AS [Name]
UNION
SELECT 9999 AS [Id], 'Mailing Lists' AS [Name]
";

        private const string SQL_GROUPS = @"
SELECT 
	 [MinistryID] AS [Id]
	,[Name]
	,[MinistryType] AS [GroupTypeId]
FROM tblMinistries M
WHERE MinistryType IN (1,3,4,5,24)

UNION

SELECT
	[MailingListID] + 999000000 AS [Id]
	,[Name]
	,9999 AS [GroupTypeId]
FROM [tblMailingLists]
";

        private const string SQL_GROUP_MEMBERS = @"
SELECT DISTINCT
	 I.[ContactID] AS [PersonId]
	,I.[Activity] AS [GroupId]
	,CASE WHEN L.[Description] IS NULL THEN 'Unknown' ELSE L.[Description] END AS [Role]
FROM [tblInvolvement] I
INNER JOIN tblMinistries M ON M.MinistryID = I.Activity AND M.MinistryType = I.ActivityType
INNER JOIN tblContacts C ON C.ContactID = I.ContactID
LEFT OUTER JOIN [dbo].[qryLookup] L ON L.CodeID = I.CommitmentLevel

UNION

SELECT DISTINCT
	I.ContactID AS [PersonId]
	,M.MailingListID + 999000000 AS [GroupId]
	,'Member' AS [Role]
FROM tblInvolvement I
INNER JOIN tblMailingLists M ON M.MailingListId = I.Activity
";

        private const string SQL_ATTENDANCE_LOCATIONS = @"
SELECT
	[ID] AS [Id]
	,'' AS [ParentLocationId]
	,[Name]
	,1 AS [IsActive]
	,'' AS [LocationType]
	,'' AS [Street1]
	,'' AS [Street2]
	,'' AS [City]
	,'' AS [State]
	,'' AS [Country]
	,'' AS [Postal]
	,'' AS [County]
  FROM [tblEventRoomDefinitions]
";

        private static string SQL_ATTENDANCE = $@"
SELECT
	 EA.[AttendanceID] AS [AttendanceId]
    ,EA.[ContactID] AS [PersonId]
    ,EA.[EventID] AS [GroupId]
    ,EA.[RoomID] AS [LocationId] -- Maybe LocationId
    ,'' AS [ScheduleId]
    ,'' AS [DeviceId]
    ,CASE WHEN EA.[CheckInTime] IS NULL THEN EA.[Date] ELSE EA.[CheckInTime] END AS [StartDateTime] -- Substitute Date when CheckInTime is null
    ,EA.[CheckOutTime] AS [EndDateTime]
    ,'Imported From Elexio' AS [Note]
    ,ERD.CampusID AS [CampusId]
FROM [tblEventAttendance] EA
LEFT OUTER JOIN tblEventRoomDefinitions ERD ON ERD.ID = EA.RoomID
LEFT OUTER JOIN tblEventDefinitions ED ON ED.ID = EA.EventID
    AND EA.CheckInTime >= { _modifiedSince.ToShortDateString() }
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
        public static void ExportIndividuals( DateTime modifiedSince )
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

            // export person photos
            using ( var dtPeopleImagesCount = GetTableData( SQL_PEOPLE_PHOTOS_COUNT ) )
            {
                if ( dtPeopleImagesCount != null )
                {
                    int counter = 0;
                    int rowsPerPage = 100;
                    bool moreImages = true;
                    var imageCount = dtPeopleImagesCount.Rows[0].Field<int>( "Count" );

                    while ( moreImages )
                    {
                        using ( var dtImages = GetTableData( String.Format( SQL_PEOPLE_PHOTOS, counter, counter + rowsPerPage ) ) )
                        {
                            foreach ( DataRow row in dtImages.Rows )
                            {
                                // save image
                                var imageData = row.Field<byte[]>( "ImageData" );
                                if ( imageData != null )
                                {
                                    var id = row.Field<int>( "Id" );
                                    var path = Path.Combine( ImportPackage.ImageDirectory, "Person_" + id + ".jpg" );
                                    File.WriteAllBytes( path, imageData );
                                }
                            }

                            counter += rowsPerPage;

                            if ( counter > imageCount )
                            {
                                moreImages = false;
                            }
                        }
                    }
                }
            }

            // export person notes
            using ( var dtPersonNotes = GetTableData( SQL_PEOPLE_NOTES ) )
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

            // export financial pledges
            using ( var dtPledges = GetTableData( SQL_FINANCIAL_PLEDGES ) )
            {
                foreach ( DataRow row in dtPledges.Rows )
                {
                    var importPledge = ElexioFinancialPledge.Translate( row );

                    if ( importPledge != null )
                    {
                        ImportPackage.WriteToPackage( importPledge );
                    }
                }
            }


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

            // export financial transactions & details
            using ( var dtTransactions = GetTableData( SQL_FINANCIAL_TRANSACTIONS ) )
            {
                foreach ( DataRow row in dtTransactions.Rows )
                {
                    // transaction
                    var importTransaction = ElexioFinancialTransaction.Translate( row );

                    if ( importTransaction != null )
                    {
                        ImportPackage.WriteToPackage( importTransaction );
                    }

                    // detail
                    var importTransactionDetail = ElexioFinancialTransactionDetail.Translate( row );

                    if ( importTransactionDetail != null )
                    {
                        ImportPackage.WriteToPackage( importTransactionDetail );
                    }
                }
            }

            // export check images
            using ( var dtCheckImagesCount = GetTableData( SQL_FINANCIAL_TRANSACTIONS_IMAGES_COUNT ) )
            {
                if ( dtCheckImagesCount != null )
                {
                    int counter = 0;
                    int rowsPerPage = 100;
                    bool moreImages = true;
                    var imageCount = dtCheckImagesCount.Rows[0].Field<int>( "Count" );
                    
                    while ( moreImages )
                    {
                        using ( var dtCheckImages = GetTableData( String.Format( SQL_FINANCIAL_TRANSACTIONS_IMAGES, counter, counter + rowsPerPage ) ) )
                        {
                            foreach ( DataRow row in dtCheckImages.Rows )
                            {
                                // save check image
                                var imageData = row.Field<byte[]>( "ImageData" );
                                if ( imageData != null )
                                {
                                    var id = row.Field<int>( "Id" );
                                    var path = Path.Combine( ImportPackage.ImageDirectory, "FinancialTransaction_" + id + "_0.jpg" );
                                    File.WriteAllBytes( path, imageData );
                                }
                            }

                            counter += rowsPerPage;

                            if ( counter > imageCount )
                            {
                                moreImages = false;
                            }
                        }
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
                Name = "Membership Date",
                Key = "MembershipDate",
                Category = "Membership",
                FieldType = "Rock.Field.Types.DateFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "School",
                Key = "School",
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
                Category = "Membership",
                FieldType = "Rock.Field.Types.DateFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Baptized Here",
                Key = "BaptizedHere",
                Category = "Membership",
                FieldType = "Rock.Field.Types.BooleanFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Age Group",
                Key = "AgeGroup",
                Category = "Elexio",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Background Check Date",
                Key = "BackgroundCheckDate",
                Category = "Safety & Security",
                FieldType = "Rock.Field.Types.DateFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Background Check Result",
                Key = "BackgroundCheckResult",
                Category = "Safety & Security",
                FieldType = "Rock.Field.Types.SelectSingleFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Allergy",
                Key = "Allergy",
                Category = "Childhood Information",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Medical",
                Key = "Medical",
                Category = "Childhood Information",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            // social media attributes
            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Facebook",
                Key = "Facebook",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.UrlLinkFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Instagram",
                Key = "Instagram",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.UrlLinkFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Twitter",
                Key = "Twitter",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.UrlLinkFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Linked-In",
                Key = "LinkedIn",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.UrlLinkFieldType"
            } );
        }

        /// <summary>
        /// Exports the attendance.
        /// </summary>
        public static void ExportAttendance( DateTime modifiedSince )
        {
            // export attendance locations
            using ( var dtLocations = GetTableData( SQL_ATTENDANCE_LOCATIONS ) )
            {
                foreach ( DataRow row in dtLocations.Rows )
                {
                    var importLocation = ElexioLocation.Translate( row );

                    if ( importLocation != null )
                    {
                        ImportPackage.WriteToPackage( importLocation );
                    }
                }
            }

            // export attendance
            using ( var dtAttendance = GetTableData( SQL_ATTENDANCE ) )
            {
                foreach ( DataRow row in dtAttendance.Rows )
                {
                    var importAttendance = ElexioAttendance.Translate( row );

                    if ( importAttendance != null )
                    {
                        ImportPackage.WriteToPackage( importAttendance );
                    }
                }
            }
        }

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

                // create a parent group for each group type
                ImportPackage.WriteToPackage( new Group()
                {
                    Id = 2099999000 + groupType.Id,
                    Name = groupType.Name,
                    GroupTypeId = groupType.Id
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
                dbCommand.CommandTimeout = 300;
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
