/*
	NOTE: Setting the first name to Company so that we can reference it when we convert the business.
*/
SELECT
	C.Household_ID + 1000000000 AS Id,
	C.Household_ID AS FamilyId,
	LEFT(C.Household_Name, 49) AS FamilyName,
	'' AS FamilyImageUrl,
	'Adult' AS FamilyRole,
	'Company' AS FirstName,
	'' AS NickName,
	LEFT(C.Household_Name, 49) AS LastName,
	'' AS MiddleName,
	'' AS Salutation,
	'' AS Suffix,
	'' AS Email,
	'Unknown' AS Gender,
	'Unknown' AS MaritalStatus,
	'' AS Birthdate,
	'' AS AnniversaryDate,
	'Active' AS RecordStatus,
	'' AS InactiveReason,
	'Attendee' AS ConnectionStatus,
	'EmailAllowed' AS EmailPreference,
	CASE WHEN C.Created_Date IS NULL THEN '' ELSE C.Created_Date END AS CreatedDateTime,
	CASE WHEN C.Last_Updated_Date IS NULL THEN '' ELSE C.Last_Updated_Date END AS ModifiedDateTime,
	'' AS PersonPhotoUrl,
	0 AS CampusId,
	'' AS CampusName,
	'' AS Note,
	'' AS Grade,
	'' AS GiveIndividually,
	'FALSE' AS IsDeceased
FROM Company C
ORDER BY C.Household_ID + 1000000000