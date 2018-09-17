SELECT
	C.Household_ID + 1000000000 AS PersonId,
	'Work' AS PhoneType,
	CC.Communication_Value AS PhoneNumber,
	'' AS IsMessagingEnabled,
	'' AS IsUnlisted
FROM Communication CC 
INNER JOIN Company C ON C.Household_ID = CC.Household_ID
WHERE CC.Communication_Type LIKE '%phone%'