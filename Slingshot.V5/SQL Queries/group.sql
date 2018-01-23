-- This is the offset we use to build parent groups with
DECLARE @ParentOffset int = '10000';

--
-- Getting all groups
--
SELECT
	-- Id --
	grp.[Counter] AS [Id]
	-- Name --
	,grp.[Descr] AS [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,grp.[SGOrgLvlCounter] + @ParentOffset AS [ParentGroupId]
	-- GroupTypeId --
	,'100' AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	,'' AS [GroupMembers]
FROM [Shelby].[SGOrgGrp] grp
LEFT OUTER JOIN [Shelby].[SGOrgLvl] lvl ON grp.[SGOrgLvlCounter] = lvl.[Counter]
LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]

UNION ALL
--
-- Making all parent groups
--
SELECT 
	-- Id --
	lvl.[Counter] + @ParentOffset AS [Id]
	-- Name --
	,lvl.[Descr] AS [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,org.[Counter] + @ParentOffset + @ParentOffset AS [ParentGroupId]
	-- GroupTypeId --
	,'100' AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	,'' AS [GroupMembers]
FROM [Shelby].[SGOrgLvl] lvl
LEFT OUTER JOIN [Shelby].[SGOrg] org ON lvl.[SGOrgCounter] = org.[Counter]
WHERE Lvl = 1

UNION ALL
--
-- Making all Grandparent groups
--
SELECT 
	-- Id --
	org.[Counter] + @ParentOffset + @ParentOffset AS [Id]
	-- Name --
	,org.[Descr] AS [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,'0' AS [ParentGroupId]
	-- GroupTypeId --
	,'100' AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	,'' AS [GroupMembers]
	
FROM [Shelby].[SGOrg] org
WHERE org.[Counter] != 27 -- Exclude empty SECURITY ALERT group
ORDER BY [Id] DESC