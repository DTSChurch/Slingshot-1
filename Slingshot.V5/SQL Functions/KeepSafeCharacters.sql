USE [ShelbyDB]
GO

/****** Object:  UserDefinedFunction [dbo].[KeepSafeCharacters]    Script Date: 5/9/2017 9:14:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[KeepSafeCharacters] (
	@Temp VARCHAR(1000)
	,@Flag BIT = 1
	)
RETURNS VARCHAR(1000)
AS
BEGIN
	DECLARE @KeepValues AS VARCHAR(50)

	SET @KeepValues = '%[^a-zA-Z0-9_ ]%'

	WHILE PatIndex(@KeepValues, @Temp) > 0
		SET @Temp = Stuff(@Temp, PatIndex(@KeepValues, @Temp), 1, '')

	IF @Flag = 1
	BEGIN
		SET @Temp = [dbo].[KeepSafeCharacters_RemoveAccents](@Temp)
	END

	RETURN Rtrim(Ltrim(@Temp))
END


GO


