-- This is the date we use to grab only recent groups
DECLARE @StartDate DateTime = '2019-01-01';

SELECT DISTINCT
	-- PersonId --
	org.[NameCounter] AS [PersonId]
	-- GroupId --
	,org.SGOrgGrpCounter AS [GroupId]
	-- Role --
	,'Member' AS [Role]
FROM [Shelby].[SGMstOrg] org
INNER JOIN [Shelby].[SGOrgGrp] grp ON org.SGOrgGrpCounter = grp.[Counter]
INNER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
INNER JOIN [Shelby].[SGOrg] sorg ON lvl.[SGOrgCounter] = sorg.[Counter]
WHERE [org].[Counter] IS NOT NULL
	AND grp.[Counter] In (
		SELECT	grp.[Counter] AS [Id]
			FROM [Shelby].[SGOrgGrp] grp
			LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
			LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
			WHERE grp.WhenUpdated >= @StartDate
			Group By grp.[Counter],grp.[Descr],grp.[SGOrgLvlCounter],lvl.SGOrgCounter,org.Descr
		)

-- Uncomment the line below if you just want to select the "Active" groups
--AND [org].[Active] = '0'
ORDER BY PersonId, GroupId