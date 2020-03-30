SELECT 
	-- Id --
	c.[Counter] AS [Id],
	-- BatchId --
	c.[BatchNu] AS [BatchId],
	-- AuthorizedPersonId --
	c.NameCounter AS [AuthorizedPersonId],
	-- TransactionDate, If no createdDate then use When Setup --
	CONVERT(VARCHAR(10),ISNULL(c.[CNDate],c.[WhenSetup]),101) AS [TransactionDate],
	-- TransactionType --
	'Contribution' AS [TransactionType],
	/* They currently have 220 Types.. WE are just going to classify everything as "Contribution"
	CASE
		WHEN a.[Purpose] LIKE '%CAMP' THEN 'EventRegistration' -- Covers Youth Camps
		WHEN a.[Purpose] LIKE '%EVENT' THEN 'EventRegistration' -- Covers Youth Events
		ELSE 'Contributuon'
	END AS [TransactionType],
	*/
	-- TransactionSource --
	CASE
		WHEN c.[CheckNu] LIKE 'T%' THEN 'Website'
		ELSE 'OnsiteCollection'
	END AS [TransactionSource],
	-- CurrencyType --
	CASE 
		WHEN c.[CheckNu] LIKE 'Non-Cash' THEN 'NonCash'
		WHEN c.[CheckNu] LIKE 'CASH%' THEN 'Cash'
		WHEN c.[CheckNu] LIKE 'ACH%' THEN 'ACH'
		WHEN c.[CheckNu] LIKE 'ONLINE%' THEN 'CreditCard'
		WHEN c.[CheckNu] LIKE '%CARD'
		   	 OR c.[CheckNu] LIKE 'CC%'
			 OR c.[CheckNu] LIKE 'Visa%'
			 OR c.[CheckNu] LIKE 'MC%'
			 THEN 'CreditCard'
		WHEN c.[CheckNu] LIKE 'T%' THEN 'CreditCard' -- Online Donations need to bulk update these later
		WHEN ISNUMERIC(c.CheckNu) = 1 THEN 'Check'
		WHEN c.[CheckNu] LIKE '%Payroll%' THEN 'Check'
		WHEN c.[CheckNu] LIKE '%REEF%' THEN 'Check'
		ELSE 'Unknown'
	END AS [CurrencyType],
	-- Summary --
			CASE 
				WHEN c.[CheckNu] LIKE 'Non-Cash' THEN 'NonCash'
				WHEN c.[CheckNu] LIKE 'CASH%' THEN 'Cash'
				WHEN c.[CheckNu] LIKE 'ACH%' THEN 'ACH'
				WHEN c.[CheckNu] LIKE 'ONLINE%' THEN 'CreditCard'
				WHEN c.[CheckNu] LIKE '%CARD'
		   			 OR c.[CheckNu] LIKE 'CC%'
					 OR c.[CheckNu] LIKE 'Visa%'
					 OR c.[CheckNu] LIKE 'MC%'
					 THEN 'CreditCard'
				WHEN c.[CheckNu] LIKE 'T%' THEN 'CreditCard' -- Online Donations
				WHEN ISNUMERIC(c.CheckNu) = 1 THEN 'Check'
				ELSE 'Unknown'
			END AS [Summary],
	-- TransactionCode --
	c.[CheckNu] AS [TransactionCode],
	-- CreatedByPersonId --
	'' AS [CreatedByPersonId],
	-- CreatedDateTime --
	CONVERT(VARCHAR(10),c.WhenSetup,101) AS [CreatedDateTime],
	-- ModifiedByPersonId --
	'' AS [ModifiedByPersonId],
	-- ModifiedDateTime --
	CONVERT(VARCHAR(10),c.WhenUpdated,101) AS [ModifiedDateTime],
	-- List<FinancialTransactionDetails>
	'' AS [FinancialTransactionDetails]

FROM Shelby.[CNHst] c