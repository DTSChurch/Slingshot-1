SELECT
	-- Id --
	p.[Counter] AS [Id],
	-- PersonId --
	p.[NameCounter] AS [PersonId],
	-- AccountId --
	a.[Counter] AS [AccountId],
	-- StartTime --
	CONVERT(VARCHAR(10),p.WhenSetup,101) AS [StartDate],
	-- EndTime --
	CONVERT(VARCHAR(10),p.WhenUpdated,101) AS [EndDate],
	-- PledgeFrequency --
	CASE 
		WHEN p.PayPerYear = 52
			THEN 'Weekly'
		WHEN p.PayPerYear = 26
			THEN 'BiWeekly'
		WHEN p.PayPerYear = 24
			THEN 'TwiceAMonth'
		WHEN p.PayPerYear = 12
			THEN 'Monthly'
		WHEN p.PayPerYear = 6
			THEN 'Quarterly' --'Bi-Monthly' not supported
		WHEN p.PayPerYear = 4
			THEN 'Quarterly'
		WHEN p.PayPerYear = 2
			THEN 'TwiceAYear'
		WHEN p.PayPerYear = 1
			THEN 'Yearly'
		END AS [PledgeFrequency],
	-- Total Amount --
	p.Pledge AS [TotalAmount],
	-- CreateDateTime --
	CONVERT(VARCHAR(10),p.WhenSetup,101) AS [CreateDateTime],
	-- ModifiedDateTime --
	CONVERT(VARCHAR(10),p.WhenUpdated,101) AS [ModifiedDateTime]

FROM Shelby.CNPlg p
JOIN Shelby.CNPur a ON p.PurCounter = a.[Counter]