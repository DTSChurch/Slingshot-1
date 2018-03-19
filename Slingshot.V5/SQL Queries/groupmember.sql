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

-- Uncomment the line below if you just want to select the "Active" groups
AND [org].[Active] = '-1'
ORDER BY PersonId, GroupId