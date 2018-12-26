SELECT * FROM
(
-- These are the very top level groups -- 
-- They are the Ministry from F1 --
SELECT DISTINCT
	-- Id --
	G1.Ministry_ID AS [Id]
	-- Name --
	,G1.Ministry_Name [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,'' AS [ParentGroupId]
	-- GroupTypeId --
	,1000 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	, 1 as IsActive

FROM ActivityMinistry G1

UNION ALL
SELECT DISTINCT
	-- Id --
	'1234567' AS [Id]
	-- Name --
	,'Unknown' [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,'' AS [ParentGroupId]
	-- GroupTypeId --
	,1000 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	, 1 as IsActive

--WHERE G1.Ministry_Name IS NULL

UNION ALL

-- This is the second level in the Group structure --
-- These are the Activities from F1 --
SELECT DISTINCT
	-- Id --
	G2.Activity_ID AS [Id]
	-- Name --
	,G2.Activity_Name [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,G2.Ministry_ID AS [ParentGroupId]
	-- GroupTypeId --
	,1000 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	, Activity_Active as IsActive

FROM ActivityMinistry G2

--WHERE G2.Activity_Name IS NULL

UNION ALL

SELECT DISTINCT
	-- Id --
	G3.Activity_Group_ID AS [Id]
	-- Name --
	,G3.Activity_Group_Name [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,G3.Activity_ID AS [ParentGroupId]
	-- GroupTypeId --
	,1000 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	, 1 as IsActive
	

FROM Activity_Group G3

--WHERE G3.Activity_ID IS NULL

UNION ALL

-- This is the forth level of the Group structure in Rock -- 
-- This is pulling from the RLC id and name in F1

SELECT DISTINCT
	-- Id --
	RLC.RLC_ID AS [Id]
	-- Name --
	,CASE 
		WHEN RLC.RLC_Name = '' THEN 'Unknown'
		ELSE RLC.RLC_Name
		END AS [Name]
	-- Order --
	,'0' AS [Order]
	-- ParentGroupId --
	,CASE
		WHEN RLC.Activity_Group_ID IS NULL THEN 1234567
		ELSE RLC.Activity_Group_ID 
		END AS [ParentGroupId]
	-- GroupTypeId --
	,1000 AS [GroupTypeId]
	-- CampusId --
	,'' AS [CampusId]
	, Is_Active as IsActive
FROM RLC


--WHERE RLC.Activity_Group_ID IS NULL

UNION ALL

SELECT Distinct
	990000000 + breakoutGroupId as Id
	, 'Break Out Group: ' + BreakoutGroup as [Name]
	, 0 as [Order]
	, isnull(RLC_ID,Activity_ID) as ParentGroupId
	, 1001 as GroupTypeId
	,'' as CampusId
	, 1 as IsActive
FROM [ActivityAssignment]
Where BreakoutGroupId is not null

) tmp

Order by Id Desc