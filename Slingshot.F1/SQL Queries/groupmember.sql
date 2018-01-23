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