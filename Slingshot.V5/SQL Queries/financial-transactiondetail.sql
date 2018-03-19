SELECT
	-- Id --
	d.[Counter] AS [Id],
	-- TransactionId --
	ch.[Counter] AS [TransactionId],
	-- AccountId --
	a.[Counter] AS [AccountId],
	-- Amount --
	d.[Amount] AS [Amount],
	-- Summary --
	'' AS [Summary],
	-- CreatedByPersonId --
	'' AS [CreatedByPersonId],
	-- CreatedDateTime --
	CONVERT(VARCHAR(10),ch.WhenSetup,101) AS [CreatedDateTime],
	-- ModifiedByPersonId -- 
	'' AS [ModifiedByPersonId],
	-- ModifiedDateTime --
	CONVERT(VARCHAR(10),ch.WhenUpdated,101) AS [ModifiedDateTime]
	
FROM Shelby.CNHst ch
JOIN Shelby.CNHstDet d ON ch.[Counter] = d.HstCounter
JOIN Shelby.CNPur a ON d.PurCounter = a.[Counter]
