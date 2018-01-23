SELECT DISTINCT
	-- PersonId --
	n.NameCounter AS [PersonId]
	-- Street1 --
	,REPLACE(REPLACE(REPLACE(ad.Adr1, CHAR(13), ''), CHAR(10), ''),'"','') AS [Street1]
	-- Street2 --
	,REPLACE(REPLACE(REPLACE(ad.Adr2, CHAR(13), ''), CHAR(10), ''),'"','') AS [Street2]
	-- City --
	,ad.City AS [City]
	-- State --
	,ad.[State] AS [State]
	-- PostalCode --
	,ad.[PostalCode] AS [PostalCode]
	-- Country --
	,CASE
		WHEN fo.Descr IS NULL AND ad.[Adr1] IS NOT NULL AND ad.[Adr1] != '' THEN 'United States'
		ELSE fo.Descr
	END AS [Country]
	-- Latitude --
	,'' AS [Latitude]
	-- Longitude --
	,'' AS [Longitude]
	---- AddressType --
	,CASE adt.[Counter]
		WHEN '1' THEN 'Home' -- V5 Type: Main/Home Address
		WHEN '2' THEN 'Previous' -- V5 Type: Family Alternate 
		WHEN '4' THEN 'Previous' -- V5 Type: Alternate
		WHEN '5' THEN 'Previous' -- V5 Type: Alternate
		WHEN '8' THEN 'Previous' -- V5 Type: Alternate
		ELSE 'Previous'
	END AS [AddressType]

FROM Shelby.NANames n
LEFT OUTER JOIN Shelby.NAAddresses ad ON n.MainAddress = ad.AddressCounter
LEFT OUTER JOIN Shelby.NAForeign fo ON ad.ForeignCounter = fo.[Counter]
LEFT OUTER JOIN Shelby.NACrossRef cr ON ad.AddressCounter = cr.AddressCounter
LEFT OUTER JOIN Shelby.NAAddressTypes adt ON cr.TypeCounter = adt.[Counter]

WHERE
N.NameCounter IS NOT NULL AND
ad.[Adr1] IS NOT NULL AND ad.[Adr1] != ''

ORDER BY PersonId