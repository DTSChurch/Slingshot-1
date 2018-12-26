SELECT DISTINCT
	-- PersonId --
	AA.Individual_ID AS [PersonId]
	-- GroupId --
	,AA.RLC_ID AS [GroupId]
	-- Role --
	,'Member' AS [Role]

FROM ActivityAssignment AA

WHERE
AA.RLC_ID IS NOT NULL

UNION ALL
SELECT Distinct
	 Individual_ID as PersonId
	, 990000000 + breakoutGroupId as GroupId
	, 'Member' as [Role]
FROM [ActivityAssignment]
Where BreakoutGroup is not null

Order By GroupId, PersonId