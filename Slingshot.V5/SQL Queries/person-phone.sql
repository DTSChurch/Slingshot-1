SELECT
	-- PersonId --
	n.NameCounter AS [PersonId]
	-- PhoneType --
	,CASE ph.PhoneCounter
		WHEN '1' THEN 'Home'
		WHEN '2' THEN 'Business'
		WHEN '7' THEN 'Business'
		WHEN '80' THEN 'Mobile'
		ELSE 'Home'
		END AS [PhoneType]
	-- PhoneNumber --
	,ISNULL(NULLIF(REPLACE(REPLACE(ph.PhoneNu, CHAR(13), ''), CHAR(10), ''), '0'), '') AS [PhoneNumber]
	-- IsMessageingEnabled --
	,CASE ph.PhoneCounter
		When 80 THEN 'TRUE'
		ELSE 'False'
	END AS [IsMessagingEnabled]
	-- IsUnlisted --
	,CASE ph.Unlisted 
		WHEN '0' THEN 'FALSE'
		ELSE 'TRUE' -- IN V5, -1 and -2 both mean Unlisted
	END AS [IsUnlisted]

FROM Shelby.NANames n
INNER JOIN Shelby.NAPhones AS ph ON n.NameCounter = ph.NameCounter

WHERE N.NameCounter IS NOT NULL
ORDER BY PersonId DESC