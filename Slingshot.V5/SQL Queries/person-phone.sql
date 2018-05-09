SELECT	
	n.NameCounter AS [PersonId]
	,CASE pt.Descr
		WHEN 'Main/Home' THEN 'Home'
		WHEN 'Personal' THEN 'Home'
		WHEN 'Business' THEN 'Business'
		WHEN 'Pager' THEN 'Pager'
		WHEN 'Cell Phone' THEN 'Mobile'
		ELSE pt.Descr
		END AS [PhoneType]	
	,ISNULL(NULLIF(REPLACE(REPLACE(ph.PhoneNu, CHAR(13), ''), CHAR(10), ''), '0'), '') AS [PhoneNumber]	
	,'TRUE' AS [IsMessagingEnabled]	
	,CASE ph.Unlisted 
		WHEN '0' THEN 'FALSE'
		ELSE 'TRUE' -- IN V5, -1 and -2 both mean Unlisted
	END AS [IsUnlisted]	
FROM Shelby.NANames n
INNER JOIN Shelby.NAPhones AS ph ON n.NameCounter = ph.NameCounter
INNER JOIN Shelby.NAPhoneTypes pt ON ph.PhoneCounter = pt.Counter
WHERE N.NameCounter IS NOT NULL
ORDER BY PersonId DESC