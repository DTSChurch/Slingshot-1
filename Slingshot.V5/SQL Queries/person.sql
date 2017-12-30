SELECT
	-- Id --
	n.NameCounter AS [Id]
	-- FamilyId --
	,n.FamNu AS [FamilyId]
	-- FamilyName
	,LEFT(ISNULL(Shelby.cust_bema_fn_v5FamilyName(n.FamNu),''),50) AS [FamilyName]
	-- FamilyImageURL --
	,'' AS [FamilyImageURL]
	-- FamilyRole
	,CASE
		WHEN n.UnitNu = '0' THEN 'Adult'
		WHEN n.UnitNu = '1' THEN 'Adult'
		WHEN n.UnitNu = '2' THEN 'Child'
		WHEN n.UnitNu = '3' THEN 'Child' -- 'Other' Had to change this to Child so that the Import will work.
		ELSE 'AdultOther'
		END AS [FamilyRole]
	-- FirstName --
	,LEFT(ISNULL(dbo.KeepSafeCharacters(n.FirstMiddle, 1), '_'), 50) AS [FirstName]
	-- NickName --
	,LEFT(ISNULL(dbo.KeepSafeCharacters(n.Salutation, 1), ''), 50) AS [NickName]
	 -- LastName
	,LEFT(ISNULL(NULLIF(LTRIM(ISNULL(NULLIF(LTRIM(dbo.KeepSafeCharacters(n.LastName, 1)), ''), n.FirstMiddle)), ''), 'UNKNOWN'), 50) AS [LastName]
	-- MiddleName --
	,SUBSTRING(n.FirstMiddle,CHARINDEX(' ',n.FirstMiddle + ' ')+1,LEN(n.FirstMiddle)) AS [MiddleName]
	-- Salutation --
	,'' AS [Salutation]
	-- Suffix -- 
	,ISNULL(dbo.KeepSafeCharacters(s.Descr, 1), '') AS [Suffix]
	-- Email --
	,CASE 
		WHEN n.EMailAddress LIKE '%;%' THEN LEFT(n.EMailAddress, CHARINDEX(';', n.EMailAddress) - 1)
		ELSE ISNULL(n.EMailAddress, '')
		END AS [Email]
	-- Gender --
	,CASE 
		WHEN n.Gender = 'F'
			THEN 'Female'
		WHEN n.Gender = 'M'
			THEN 'Male'
		ELSE 'Unknown'
		END AS [Gender]
	-- MarritalStatus
	,CASE
		WHEN n.MaritalStatus = '' THEN 'Unknown'
		WHEN n.MaritalStatus = 'D' THEN 'Divorced'
		WHEN n.MaritalStatus = 'S' THEN 'Single'
		WHEN n.MaritalStatus = 'W' THEN 'Unknown' -- Changed this from Widowed for import to work.
		WHEN n.MaritalStatus = 'M' THEN 'Married'
		ELSE 'Unknown' END AS [MaritalStatus]
	-- Birthdate --
	,ISNULL(CONVERT(VARCHAR(10), CAST(N.Birthdate AS DATE), 101), '') AS [Birthdate]
	-- Anniversary -- 
	,'' AS [Anniversary]
	-- RecordStatus
	,CASE M.RecordStatus
		WHEN 0 THEN 'Active'
		WHEN 1 THEN 'Inactive'
		ELSE 'Inactive'
		END AS [RecordStatus]
	-- InactiveReason --
	,'' AS [InactiveReason]
	-- ConnectionStatus -- 
	,'Attendee' AS [ConnectionStatus]
	-- EmailPreference -- 
	,CASE 
		WHEN email.NameCounter IS NOT NULL THEN 'NoMassEmails'
		ELSE 'EmailAllowed'
	END AS [EmailPreference]
	-- CreatedDateTime --
	,CONVERT(VARCHAR(10),ISNULL(n.WhenSetup, GETDATE()), 101) AS [CreatedDateTime]
	-- ModifiedDateTime --
	,CONVERT(VARCHAR(10),ISNULL(n.WhenUpdated, GETDATE()), 101) AS [ModifiedDateTime]
	-- PersonPhotoUrl --
	,'' AS [PersonPhotoUrl]
	-- CampusId --
	,'0' AS [Campus]
	---- CampusName --
	--,'' AS [CampusName]
	-- Note --
	,REPLACE(REPLACE(n.[Memo], CHAR(13), ''), CHAR(10), '') AS [Note]
	-- Grade --
	,CASE
		WHEN gradek.[NameCounter] IS NOT NULL THEN 'K'
		WHEN grade1.[NameCounter] IS NOT NULL THEN '1st'
		WHEN grade2.[NameCounter] IS NOT NULL THEN '2nd'
		WHEN grade3.[NameCounter] IS NOT NULL THEN '3rd'
		WHEN grade4.[NameCounter] IS NOT NULL THEN '4th'
		WHEN grade5.[NameCounter] IS NOT NULL THEN '5th'
		WHEN grade6.[NameCounter] IS NOT NULL THEN '6th'
		WHEN grade7.[NameCounter] IS NOT NULL THEN '7th'
		WHEN grade8.[NameCounter] IS NOT NULL THEN '8th'
		WHEN grade9.[NameCounter] IS NOT NULL THEN '9th'
		WHEN grade10.[NameCounter] IS NOT NULL THEN '10th'
		WHEN grade11.[NameCounter] IS NOT NULL THEN '11th'
		WHEN grade12.[NameCounter] IS NOT NULL THEN '12th'
		ELSE ''
	END AS [Grade]
	,'' AS [Attributes]
	,'' AS [PhoneNumbers]
	,'' AS [Addresses]
	,CASE
		WHEN n.UnitNu = 0 AND m.PledgeSpouse = -1  THEN 'FALSE'
		WHEN n.UnitNu = 1 AND m.PledgeSpouse = 0 THEN 'FALSE'
		WHEN n.UnitNu = 2 THEN 'TRUE'
		ELSE 'TRUE' 
	END AS [GiveIndividually]
	-- IsDeceased --
	,'FALSE' AS [IsDeceased]

FROM Shelby.NANames n
LEFT OUTER JOIN Shelby.[NATitles] t ON n.TitleCounter = t.[Counter]
LEFT OUTER JOIN Shelby.[NASuffixes] s ON n.SuffixCounter = s.[Counter]
LEFT OUTER JOIN Shelby.[MBMst] m ON n.NameCounter = m.NameCounter
LEFT OUTER JOIN Shelby.[MBMstLif] wed ON n.NameCounter = wed.NameCounter AND wed.MBLifCounter = 4
LEFT OUTER JOIN Shelby.[MBTextPicks] ms ON m.RelationshipCounter = ms.[Counter]
LEFT OUTER JOIN Shelby.[NAProfiles] gradek ON n.[NameCounter] = gradek.[NameCounter] AND gradek.[Profile] = 'GRADEK'
LEFT OUTER JOIN Shelby.[NAProfiles] grade1 ON n.[NameCounter] = grade1.[NameCounter] AND grade1.[Profile] = 'GRADE1'
LEFT OUTER JOIN Shelby.[NAProfiles] grade2 ON n.[NameCounter] = grade2.[NameCounter] AND grade2.[Profile] = 'GRADE2'
LEFT OUTER JOIN Shelby.[NAProfiles] grade3 ON n.[NameCounter] = grade3.[NameCounter] AND grade3.[Profile] = 'GRADE3'
LEFT OUTER JOIN Shelby.[NAProfiles] grade4 ON n.[NameCounter] = grade4.[NameCounter] AND grade4.[Profile] = 'GRADE4'
LEFT OUTER JOIN Shelby.[NAProfiles] grade5 ON n.[NameCounter] = grade5.[NameCounter] AND grade5.[Profile] = 'GRADE5'
LEFT OUTER JOIN Shelby.[NAProfiles] grade6 ON n.[NameCounter] = grade6.[NameCounter] AND grade6.[Profile] = 'GRADE6'
LEFT OUTER JOIN Shelby.[NAProfiles] grade7 ON n.[NameCounter] = grade7.[NameCounter] AND grade7.[Profile] = 'GRADE7'
LEFT OUTER JOIN Shelby.[NAProfiles] grade8 ON n.[NameCounter] = grade8.[NameCounter] AND grade8.[Profile] = 'GRADE8'
LEFT OUTER JOIN Shelby.[NAProfiles] grade9 ON n.[NameCounter] = grade9.[NameCounter] AND grade9.[Profile] = 'GRADE9'
LEFT OUTER JOIN Shelby.[NAProfiles] grade10 ON n.[NameCounter] = grade10.[NameCounter] AND grade10.[Profile] = 'GRADE10'
LEFT OUTER JOIN Shelby.[NAProfiles] grade11 ON n.[NameCounter] = grade11.[NameCounter] AND grade11.[Profile] = 'GRADE11'
LEFT OUTER JOIN Shelby.[NAProfiles] grade12 ON n.[NameCounter] = grade12.[NameCounter] AND grade12.[Profile] = 'GRADE12'
LEFT OUTER JOIN Shelby.[NAProfiles] email ON n.[NameCounter] = email.[NameCounter] AND email.[Profile] = 'MAIL'
WHERE
M.NameCounter IS NOT NULL