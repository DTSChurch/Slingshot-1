SELECT 
	-- Id --
	bat.[BatchNu] AS [Id]
	-- Name --
	,CONVERT(VARCHAR(10), ISNULL(bat.WhenSetup, ''), 111) + ' - ' + isnull(nullif(ltrim(replace(replace(replace(bat.WhoPosted, ',', ''), CHAR(13), ''), CHAR(10), '')), ''), 'UNKNOWN') AS [Name]
	-- CampusId --
	,'' AS [CampusId]
	-- StartDate --
	,'' AS [StartDate]
	-- EndDate --
	,'' AS [EndDate]
	-- Status --
	,CASE 
		WHEN bat.[Posted] = '-1' THEN 'Closed'
		WHEN bat.[Posted] = 0 AND bat.[WhenPosted] IS NULL THEN 'Pending'
		ELSE 'Open'
		END AS [Status]
	-- CreatedByPersonId --
	,'' AS [CreatedByPersonId]
	-- CreatedDateTime --
	,CONVERT(VARCHAR(10), ISNULL(bat.WhenSetup, ''), 101) AS [CreatedDateTime]
	-- ModifiedByPersonId --
	,'' AS [ModifiedByPersonId]
	-- ControlAmount --
	,'' AS [ModifiedDateTime]
	,[Total] AS [ControlAmount]
	-- List<FinancialTransactions> --
	,'' AS [FinancialTransactions]
FROM Shelby.CNBat bat