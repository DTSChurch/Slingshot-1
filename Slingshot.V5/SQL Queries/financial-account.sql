SELECT
	-- Id --
	pur.[Counter] AS [Id]
	-- Name --
	,pur.[Descr] AS [Name]
	-- IsTaxDeductible --
	,CASE pur.[TaxDed]
		WHEN '-1' THEN 'true'
		WHEN '0' THEN 'false'
		END AS [IsTaxDeductible]
	-- CampusId --
	, '' AS [CampusId]
	-- ParentAccountId --
	,'' AS [ParentAccountId]
FROM [Shelby].[CNPur] pur