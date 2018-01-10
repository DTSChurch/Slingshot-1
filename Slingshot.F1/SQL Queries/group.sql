
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
	,'' AS [GroupMembers]

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
	,'' AS [GroupMembers]

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
	,'' AS [GroupMembers]

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
	,'' AS [GroupMembers]

FROM Activity_Group G3

--WHERE G3.Activity_ID IS NULL

UNION ALL

-- This is the forth level of the Group structure in Rock -- 
-- This is pulling from the RLC id and name in F1

SELECT DISTINCT
	-- Id --
	RLC.RLC_ID AS [Id]
	-- Name --
	,RLC.RLC_Name [Name]
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
	,'' AS [GroupMembers]

FROM RLC

--WHERE RLC.Activity_Group_ID IS NULL