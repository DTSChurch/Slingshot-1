/*
Slingshot only supports using the 4 predefined Phone Type Sources on import

This script assumes you bring over any other non supported phone types in as the "Home" type

Import each other phone type into it's own custom table, join the PhoneNumber table to your custom table using this script

Create new PHone Types in rock and then grab the definedvalue id's and update the 701

*/


UPDATE PhoneNumber

SET NumberTypeValueId = 701

WHERE Id IN (
		SELECT DISTINCT
		pn.Id

		FROM PhoneNumber PN
		INNER JOIN Person P ON pn.PersonId = p.id
		INNER JOIN fcc_phone_other_3 ph3 on p.ForeignId = ph3.PersonId AND ph3.PhoneNumber = pn.NumberFormatted

		WHERE
		pn.NumberTypeValueId = 13)