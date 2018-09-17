SELECT
	C.Household_ID + 1000000000 AS PersonId,
	ISNULL(HA.Address_1,'') AS Street1,
	ISNULL(HA.Address_2,'') AS Street2,
	ISNULL(HA.City,'') AS City,
	ISNULL(HA.[State],'') AS [State],
	ISNULL(HA.Postal_Code,'') AS PostalCode,
	'' AS Country,
	'' AS Latitude,
	'' AS Longitude,
	'Work' AS AddressType
FROM Household_Address HA 
INNER JOIN Company C ON C.Household_ID = HA.Household_ID