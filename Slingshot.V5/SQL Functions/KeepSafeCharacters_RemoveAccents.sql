USE [ShelbyDB]
GO

/****** Object:  UserDefinedFunction [dbo].[KeepSafeCharacters_RemoveAccents]    Script Date: 5/9/2017 9:15:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[KeepSafeCharacters_RemoveAccents] (@Temp VARCHAR(1000))
RETURNS VARCHAR(1000)
AS
BEGIN
	-- GRAVE
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'À', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'È', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ì', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ò', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ù', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'à', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'è', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ì', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ò', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ù', 'u')
	-- ACUTE
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Á', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'É', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Í', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ó', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ú', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ý', 'Y')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'á', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'é', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'í', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ó', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ú', 'u')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ý', 'y')
	-- CIRCUMFLEX
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Â', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ê', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Î', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ô', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Û', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'â', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ê', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'î', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ô', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'û', 'u')
	-- TILDE
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ã', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ñ', 'N')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Õ', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ã', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ñ', 'n')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'õ', 'o')
	-- UMLAUT
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ä', 'A')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ë', 'E')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ï', 'I')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ö', 'O')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ü', 'U')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'Ÿ', 'Y')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ä', 'a')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ë', 'e')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ï', 'i')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ö', 'o')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ü', 'u')
	SET @Temp = replace(@Temp COLLATE Latin1_General_CS_AS, 'ÿ', 'y')

	RETURN Rtrim(Ltrim(@Temp))
END


GO


