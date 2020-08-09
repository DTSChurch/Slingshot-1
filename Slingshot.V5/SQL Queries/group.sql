-- This is the offset we use to build parent groups with
DECLARE @ParentOffset int = '60000';
DECLARE @GroupOffset int = '30000';
DECLARE @StartDate DateTime = '2019-01-01';

--
-- Getting all Connect Groups
--
SELECT
	-- Id --
	grp.[Counter] + @GroupOffset AS [Id]
	-- Name --
	,grp.[IdGrp] + ' - ' + grp.[Descr] AS [Name]
	-- Description --
	,grp.Descr + ' ' + grp.RoomNu + ' ' + grp.RollHeading AS [Description]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,grp.[SGOrgLvlCounter] + @ParentOffset AS [ParentGroupId]
	-- GroupTypeId --
	,101 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	-- Capacity --
	,'' AS Capacity
	-- MeetingDay --
	,'' AS MeetingDay
	-- MeetingTime --
	,'' AS MeetingTime
	-- IsActive --
	,1 AS IsActive
	-- IsPublic --
	,1 AS IsPublic
FROM [Shelby].[SGOrgGrp] grp
WHERE grp.[SGOrgCounter] = 3

UNION ALL


--
-- Getting all groups
--
SELECT
	-- Id --
	grp.[Counter] + @GroupOffset AS [Id]
	-- Name --
	,CASE WHEN grp.[Descr] ='' THEN 'No Name'
		ELSE grp.[Descr]
		END AS [Name]
	-- Description --
	,grp.Descr + ' ' + grp.RoomNu + ' ' + grp.RollHeading AS [Description]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,grp.[SGOrgLvlCounter] + @ParentOffset AS [ParentGroupId]
	-- GroupTypeId --
	,100 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	-- Capacity --
	,'' AS Capacity
	-- MeetingDay --
	,'' AS MeetingDay
	-- MeetingTime --
	,'' AS MeetingTime
	-- IsActive --
	,1 AS IsActive
	-- IsPublic --
	,1 AS IsPublic
FROM [Shelby].[SGOrgGrp] grp
LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
WHERE grp.WhenSetup >= @StartDate AND grp.[Descr] <> ''
	AND grp.[SGOrgCounter] != 3
Group By grp.[Counter],grp.[Descr],grp.[SGOrgLvlCounter],lvl.SGOrgCounter,org.Descr,grp.RoomNu,grp.RollHeading


UNION ALL
--
-- Making all parent groups
--
SELECT 
	-- Id --
	lvl.[Counter] + @ParentOffset AS [Id]
	-- Name --
	,lvl.[Descr] AS [Name]
	-- Description --
	,'' AS [Description]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,org.[Counter] + @ParentOffset + @ParentOffset AS [ParentGroupId]
	-- GroupTypeId --
	,100 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	,'' AS Capacity
	,'' AS MeetingDay
	,'' AS MeetingTime
	,1 AS IsActive
	,1 AS IsPublic
FROM [Shelby].[SGOrgLvl] lvl
LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
WHERE Lvl = 1 
	AND lvl.[Counter] In (
			SELECT
			-- Id --
			grp.[SGOrgLvlCounter]
		FROM [Shelby].[SGOrgGrp] grp
		LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
		LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
		WHERE grp.WhenSetup >= @StartDate)
		GROUP BY lvl.[Counter],lvl.[Descr],org.[Counter],lvl.SGOrgCounter,org.Descr

UNION ALL
--
-- Making all Grandparent groups
--
SELECT 
	-- Id --
	org.[Counter] + @ParentOffset + @ParentOffset AS [Id]
	-- Name --
	,org.[Descr] AS [Name]
	-- Description --
	,'' AS [Description]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,'0' AS [ParentGroupId]
	-- GroupTypeId --
	,100 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	,'' AS Capacity
	,'' AS MeetingDay
	,'' AS MeetingTime
	,1 AS IsActive
	,1 AS IsPublic
FROM [Shelby].[SGOrg] org
	LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON lvl.[SGOrgCounter] = org.[Counter]
WHERE org.[Counter] != 27 -- Exclude empty SECURITY ALERT group
	AND org.[Counter] In (
			SELECT
			-- Id --
			grp.[SGOrgCounter]
		FROM [Shelby].[SGOrgGrp] grp
		LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
		LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
		WHERE grp.WhenSetup >= @StartDate)
		GROUP BY org.[Counter],org.[Descr],lvl.SGOrgCounter,org.Descr
ORDER BY [Id] DESC