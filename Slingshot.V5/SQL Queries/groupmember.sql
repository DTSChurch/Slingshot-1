-- This is the date we use to grab only recent groups
DECLARE @GroupOffset int = '30000';
DECLARE @StartDate DateTime = '2019-01-01';

SELECT DISTINCT
	-- PersonId --
	org.[NameCounter] AS [PersonId]
	-- GroupId --
	,org.SGOrgGrpCounter + @GroupOffset AS [GroupId]
	-- Role --
	,'Member' AS [Role]
FROM [Shelby].[SGMstOrg] org
LEFT OUTER JOIN [Shelby].[SGOrgGrp] grp ON org.SGOrgGrpCounter = grp.[Counter]
LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
LEFT OUTER JOIN [Shelby].[SGOrg] sorg ON lvl.[SGOrgCounter] = sorg.[Counter]
WHERE [org].[Counter] IS NOT NULL
	AND grp.[Counter] In (
		SELECT	grp.[Counter] AS [Id]
			FROM [Shelby].[SGOrgGrp] grp
			LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
			LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
			WHERE grp.WhenSetup >= @StartDate
			Group By grp.[Counter],grp.[Descr],grp.[SGOrgLvlCounter],lvl.SGOrgCounter,org.Descr
		)
--ORDER BY PersonId, GroupId

UNION ALL

-- Adding ALL wanted Connect Groups
SELECT DISTINCT
	-- PersonId --
	org.[NameCounter] AS [PersonId]
	-- GroupId --
	,org.SGOrgGrpCounter + @GroupOffset  AS [GroupId]
	-- Role --
	,'Member' AS [Role]
FROM [Shelby].[SGMstOrg] org
LEFT OUTER JOIN [Shelby].[SGOrgGrp] grp ON org.SGOrgGrpCounter = grp.[Counter]
--LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
--LEFT OUTER JOIN [Shelby].[SGOrg] sorg ON lvl.[SGOrgCounter] = sorg.[Counter]
WHERE grp.[Counter] In (
--manual list of desired Connect Groups wanted
		5,7,9,46,50,122,169,170,337,392,428,704,1101,1658,2042,2737,2763,3106,3708,3795,3811,3937,4052,4582,4799,4822,5213,5214,5286,5294,5496,5705,5766,5993,6052)
AND org.Enrolled <0
ORDER BY PersonId, GroupId