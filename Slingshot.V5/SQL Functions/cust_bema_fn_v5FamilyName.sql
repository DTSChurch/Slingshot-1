USE [ShelbyDB]
GO

/****** Object:  UserDefinedFunction [Shelby].[cust_bema_fn_v5FamilyName]    Script Date: 5/9/2017 9:17:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [Shelby].[cust_bema_fn_v5FamilyName] (@FamNu INT)
RETURNS VARCHAR(100)
AS
BEGIN
	DECLARE @FamilyName VARCHAR(100)

	SET @FamilyName = (
			SELECT TOP 1 LEFT(dbo.KeepSafeCharacters(LastName, 1), 100) AS FamilyName
			FROM Shelby.NANames
			WHERE FamNu = @FamNu
			ORDER BY UnitNu
				,Gender DESC
				,HH DESC
			)

	RETURN @FamilyName
END


GO


